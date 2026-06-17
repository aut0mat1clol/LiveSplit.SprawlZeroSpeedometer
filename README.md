># SPRAWL zero Speedometer for LiveSplit

A standalone [LiveSplit](https://livesplit.org/) component that displays the player velocity in **SPRAWL zero** (`Silas-Win64-Test.exe`).

> **Note:** This is for **SPRAWL zero**, not the original **SPRAWL**.

- Displays **2D speed**, **3D speed**, and/or **Z (vertical) speed** simultaneously.
- No ASL required.
- Auto-detects loading screens and hides bogus values.
- Font and colors are inherited from LiveSplit's layout settings.

## Download & Install

1. Download the latest `LiveSplit.SprawlSpeedometer.dll` from the [Components](https://github.com/aut0mat1clol/LiveSplit.SprawlZeroSpeedometer/tree/master/Components) folder.
2. Copy it into your LiveSplit folder:
   ```
   LiveSplit/Components/LiveSplit.SprawlSpeedometer.dll
   ```
3. Restart LiveSplit.
4. Right-click LiveSplit → **Edit Layout** → **Add** → **Information** → **SPRAWL zero**.
5. Double-click the component → **Settings** to choose which speed values to show.

## Settings

| Setting | Description |
|---------|-------------|
| **Show labels** | Show/hide the `Speed 2D` / `Speed 3D` / `Speed Z` prefixes. |
| **Show 2D speed** | Horizontal speed. |
| **Show 3D speed** | Total speed. |
| **Show Z speed** | Absolute vertical speed. |
| **Decimal places** | Number of digits after the decimal point. |

You can enable any combination of 2D, 3D, and Z at the same time. The component height automatically adjusts to fit the selected lines.
