# Sprawl Zero Speedometer — Standalone Component

A single LiveSplit component DLL for **Sprawl Zero** (`Silas-Win64-Test.exe`).

**No ASL required.** Just build the DLL, drop it into `LiveSplit/Components/`, add it to your layout, and forget about it.

## How it works

The component itself does everything:

1. Finds the `Silas-Win64-Test` process.
2. Scans for `GWorld` and `GNames`.
3. Walks the actor list to find `BP_PlayerPawn_C`.
4. Reads `PlayerMovementComponent` velocity.
5. Computes and displays 2D or 3D speed.

## Files

| File | Purpose |
|------|---------|
| `GameMemoryReader.cs` | All game-memory reading logic |
| `SprawlSpeedometerComponent.cs` | LiveSplit UI component that draws the speed |
| `SprawlSpeedometerFactory.cs` | Registers the component with LiveSplit |
| `SprawlSpeedometerSettings.cs` | Serializable settings |
| `SprawlSpeedometerSettingsUI.cs` | Layout Editor settings panel |
| `Properties/AssemblyInfo.cs` | `[ComponentFactory]` discovery attribute |
| `LiveSplit.SprawlSpeedometer.csproj` | Sample project file |

## Build and install

1. Open Visual Studio and create a **C# Class Library (.NET Framework)** project named `LiveSplit.SprawlSpeedometer`.
2. Target **.NET Framework 4.8.1** (or 4.6.1 if you use an older LiveSplit).
3. Add references to:
   - `LiveSplit.Core.dll`
   - `UpdateManager.dll`
   - `WinformsColor.dll`
   - `System`, `System.Core`, `System.Drawing`, `System.Windows.Forms`, `System.Xml`
4. Copy the `.cs` files from this folder into the project.
5. Build and copy `LiveSplit.SprawlSpeedometer.dll` to `LiveSplit/Components/`.
6. In LiveSplit, add the component to your layout: **Layout Editor → Add → Information → Sprawl Speedometer**.

## Settings

Right-click the component in the Layout Editor → **Settings**:

- **Show label** — show/hide the "Speed 2D" / "Speed 3D" prefix
- **Show 2D speed** — when checked, displays 2D speed; otherwise 3D speed
- **Decimal places** — default 2
- **Font** — display font

## Notes / troubleshooting

- The component scans for the game process every ~60 frames when not attached. Once attached, it re-finds the movement component every ~300 frames (same as the ASL).
- If the game process is closed, the component detaches and shows "Game not running".
- The `0xE8` offset for `vz` is assumed contiguous after `vy` at `0xE0`. If you only want 2D speed, leave **Show 2D speed** checked.
- LiveSplit must be able to read the 64-bit game process. If you get `AccessDenied` or `MainModule` errors, make sure LiveSplit is running as administrator and that the game is not protected by anti-cheat.
