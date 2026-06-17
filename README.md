# SPRAWL zero Speedometer for LiveSplit

A standalone [LiveSplit](https://livesplit.org/) component that displays the player velocity in **SPRAWL zero** (`Silas-Win64-Test.exe`).

> **Note:** This is for **SPRAWL zero**, not the original **SPRAWL**.

## Features

- Displays **2D speed**, **3D speed**, and/or **Z (vertical) speed** simultaneously.
- No ASL required — the component reads game memory directly.
- Auto-detects loading screens and hides bogus values.
- Font and colors are inherited from LiveSplit's layout settings.
- Height automatically scales to fit the selected speed lines.

## Download & Install

1. Download the latest `LiveSplit.SprawlSpeedometer.dll` from [Releases](../../releases).
2. Copy it into your LiveSplit folder:
   ```
   LiveSplit/Components/LiveSplit.SprawlSpeedometer.dll
   ```
3. Restart LiveSplit.
4. Right-click LiveSplit → **Edit Layout** → **Add** → **Information** → **SPRAWL zero**.
5. Right-click the component → **Settings** to choose which speed values to show.

## Settings

| Setting | Description |
|---------|-------------|
| **Show labels** | Show/hide the `Speed 2D` / `Speed 3D` / `Speed Z` prefixes. |
| **Show 2D speed** | Horizontal speed: `sqrt(vx² + vy²)`. |
| **Show 3D speed** | Total speed: `sqrt(vx² + vy² + vz²)`. |
| **Show Z speed** | Absolute vertical speed: `|vz|`. |
| **Decimal places** | Number of digits after the decimal point. |

You can enable any combination of 2D, 3D, and Z at the same time. The component height automatically adjusts to fit the selected lines.

## How it works

The component reads memory directly from the game process:

1. Finds `Silas-Win64-Test.exe`.
2. Scans for `GWorld` and `GNames`.
3. Walks the actor list to locate `BP_PlayerPawn_C`.
4. Reads velocity from `PlayerMovementComponent`.
5. Detects loading screens by checking `UWorld.GameInstance.LoadingWidget` and returns `0` during loads.

## Building from source

1. Open Visual Studio and create a **C# Class Library (.NET Framework)** project.
2. Target **.NET Framework 4.8.1** (or 4.6.1 for older LiveSplit builds).
3. Add references to:
   - `LiveSplit.Core.dll`
   - `System.Drawing`
   - `System.Windows.Forms`
   - `System.Xml`
4. Copy all `.cs` files from this repository into the project.
5. Build in **Release** and copy the output DLL to `LiveSplit/Components/`.

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Component doesn't appear in LiveSplit | Make sure the DLL is in `LiveSplit/Components/` and LiveSplit was restarted. |
| Shows `0` or `-` all the time | Make sure the game is running and the process is named `Silas-Win64-Test.exe`. |
| Huge numbers during loading | Update to the latest release — loading detection is built-in. |
| Can't read process memory | Run LiveSplit as administrator. |

## Notes

- The `0xE8` offset for `vz` is assumed to be contiguous after `vy` at `0xE0`. If you only want 2D speed, leave **Show 2D speed** enabled.
- Game updates may change memory offsets or signatures, which can break the component.
- This component only reads memory; it does not write anything to the game process.

## License

This project is provided as-is for the speedrunning community. Use at your own risk.
