using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.SprawlSpeedometer
{
    /// <summary>
    /// Reads the player velocity directly from Silas-Win64-Test.exe.
    /// This replaces the ASL memory reading logic entirely.
    /// </summary>
    public class GameMemoryReader
    {
        private const string PROCESS_NAME = "Silas-Win64-Test";

        // How often (in Update calls) to look for the game process when not attached.
        private const int PROCESS_RESCAN_INTERVAL = 60;

        // How often (in Update calls) to re-find the movement component.
        private const int MC_REFRESH_INTERVAL = 300;

        private Process _process;
        private IntPtr _gworldPtr;
        private IntPtr _gnamesPtr;
        private IntPtr _cachedMC;
        private int _cacheTick;
        private int _processScanTimer;
        private Func<int, string> _decodeFName;

        public bool IsAttached => _process != null && !_process.HasExited;

        public string Status { get; private set; } = "Game not running";

        public double[] GetVelocity()
        {
            if (_processScanTimer-- <= 0)
            {
                _processScanTimer = PROCESS_RESCAN_INTERVAL;
                EnsureAttached();
            }

            if (!IsAttached)
                return new double[] { 0, 0, 0 };

            // During loading screens we don't trust the cached movement component
            // and just return zero velocity instead of reading garbage memory.
            if (IsLoading())
            {
                _cachedMC = IntPtr.Zero;
                return new double[] { 0, 0, 0 };
            }

            try
            {
                return ReadVelocity();
            }
            catch
            {
                Detach();
                return new double[] { 0, 0, 0 };
            }
        }

        /// <summary>
        /// Detects whether the game is currently loading.
        /// Matches the ASL logic: UWorld.GameInstance.LoadingWidget != null.
        /// </summary>
        public bool IsLoading()
        {
            if (!IsAttached || _gworldPtr == IntPtr.Zero)
                return true;

            try
            {
                IntPtr uworld = _process.ReadValue<IntPtr>(_gworldPtr);
                if (uworld == IntPtr.Zero) return true;

                IntPtr gi = _process.ReadValue<IntPtr>((IntPtr)((long)uworld + 0x228));
                if (gi == IntPtr.Zero) return true;

                IntPtr loadingWidget = _process.ReadValue<IntPtr>((IntPtr)((long)gi + 0x210));
                return loadingWidget != IntPtr.Zero;
            }
            catch
            {
                return true;
            }
        }

        private void EnsureAttached()
        {
            if (IsAttached)
                return;

            Detach();

            _process = Process.GetProcessesByName(PROCESS_NAME).FirstOrDefault();
            if (_process == null)
            {
                Status = "Game not running";
                return;
            }

            try
            {
                var module = _process.MainModule;
                if (module == null)
                {
                    Status = "No main module";
                    Detach();
                    return;
                }

                var scanner = new SignatureScanner(_process, module.BaseAddress, module.ModuleMemorySize);

                // GWorld
                var gworldSig = new SigScanTarget(3, "48 8B 1D ?? ?? ?? ?? 48 85 DB 74 ?? 41 B0 01 33 D2 48 8B CB");
                gworldSig.OnFound = (p, s, ptr) => ptr + 0x4 + p.ReadValue<int>(ptr);
                _gworldPtr = scanner.Scan(gworldSig);
                if (_gworldPtr == IntPtr.Zero)
                {
                    Status = "GWorld not found";
                    Detach();
                    return;
                }

                // GNames
                var gnamesSig = new SigScanTarget(3, "48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8B F8 C6");
                gnamesSig.OnFound = (p, s, ptr) => ptr + 0x4 + p.ReadValue<int>(ptr);
                _gnamesPtr = scanner.Scan(gnamesSig);
                if (_gnamesPtr == IntPtr.Zero)
                    _gnamesPtr = (IntPtr)((long)module.BaseAddress + 0x9152FC0L);

                if (_gnamesPtr == IntPtr.Zero)
                {
                    Status = "GNames not found";
                    Detach();
                    return;
                }

                IntPtr gnamesCapture = _gnamesPtr;
                _decodeFName = (id) =>
                {
                    if (id == 0) return "";
                    try
                    {
                        int block = id >> 16;
                        int offset = (id & 0xFFFF) * 2;
                        IntPtr blockPtr = _process.ReadValue<IntPtr>((IntPtr)((long)gnamesCapture + 0x10 + block * 8));
                        if (blockPtr == IntPtr.Zero) return "";

                        IntPtr entry = (IntPtr)((long)blockPtr + offset);
                        short header = _process.ReadValue<short>(entry);
                        int len = header >> 6;
                        if (len <= 0 || len > 1024) return "";

                        bool wide = (header & 1) == 1;
                        IntPtr str = (IntPtr)((long)entry + 2);
                        return wide
                            ? _process.ReadString(str, ReadStringType.UTF16, len * 2)
                            : _process.ReadString(str, ReadStringType.ASCII, len);
                    }
                    catch { return ""; }
                };

                Status = "Attached";
            }
            catch
            {
                Detach();
            }
        }

        private double[] ReadVelocity()
        {
            _cacheTick++;
            if (_cachedMC == IntPtr.Zero || _cacheTick > MC_REFRESH_INTERVAL)
            {
                _cacheTick = 0;
                _cachedMC = FindMovementComponent();
            }

            if (_cachedMC == IntPtr.Zero)
                return new double[] { 0, 0, 0 };

            long mcAddr = (long)_cachedMC;
            double vx = _process.ReadValue<double>((IntPtr)(mcAddr + 0xD8));
            double vy = _process.ReadValue<double>((IntPtr)(mcAddr + 0xE0));
            double vz = _process.ReadValue<double>((IntPtr)(mcAddr + 0xE8));

            if (double.IsNaN(vx) || double.IsInfinity(vx) ||
                double.IsNaN(vy) || double.IsInfinity(vy) ||
                double.IsNaN(vz) || double.IsInfinity(vz))
            {
                return new double[] { 0, 0, 0 };
            }

            return new double[] { vx, vy, vz };
        }

        private IntPtr FindMovementComponent()
        {
            IntPtr uworld = _process.ReadValue<IntPtr>(_gworldPtr);
            if (uworld == IntPtr.Zero) return IntPtr.Zero;

            IntPtr level = _process.ReadValue<IntPtr>((IntPtr)((long)uworld + 0x30));
            if (level == IntPtr.Zero) return IntPtr.Zero;

            int[] actorsOffs = { 0xA0, 0x98, 0xA8, 0xB0 };

            foreach (int aOff in actorsOffs)
            {
                IntPtr arr = _process.ReadValue<IntPtr>((IntPtr)((long)level + aOff));
                if ((long)arr < 0x10000) continue;

                int count = _process.ReadValue<int>((IntPtr)((long)level + aOff + 8));
                if (count < 1 || count > 50000) continue;

                for (int i = 0; i < Math.Min(count, 10000); i++)
                {
                    IntPtr actor = _process.ReadValue<IntPtr>((IntPtr)((long)arr + i * 8));
                    if ((long)actor < 0x10000) continue;

                    IntPtr classPtr = _process.ReadValue<IntPtr>((IntPtr)((long)actor + 0x10));
                    if ((long)classPtr < 0x10000) continue;

                    int nameId = _process.ReadValue<int>((IntPtr)((long)classPtr + 0x18));
                    if (_decodeFName(nameId) != "BP_PlayerPawn_C") continue;

                    IntPtr mc = _process.ReadValue<IntPtr>((IntPtr)((long)actor + 0x338));
                    if ((long)mc < 0x10000) return IntPtr.Zero;

                    IntPtr mcCls = _process.ReadValue<IntPtr>((IntPtr)((long)mc + 0x10));
                    int mcNameId = _process.ReadValue<int>((IntPtr)((long)mcCls + 0x18));
                    if (_decodeFName(mcNameId) != "PlayerMovementComponent")
                        return IntPtr.Zero;

                    return mc;
                }
            }

            return IntPtr.Zero;
        }

        public void Detach()
        {
            _process?.Dispose();
            _process = null;
            _gworldPtr = IntPtr.Zero;
            _gnamesPtr = IntPtr.Zero;
            _cachedMC = IntPtr.Zero;
            _cacheTick = 0;
            _decodeFName = null;
            Status = "Game not running";
        }
    }
}
