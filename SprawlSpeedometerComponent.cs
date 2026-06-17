using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SprawlSpeedometer
{
    public class SprawlSpeedometerComponent : IComponent
    {
        public string ComponentName => "SPRAWL zero Speedometer";

        // Base row height. Total height scales with the number of enabled lines.
        public float HorizontalWidth => 200;
        public float MinimumWidth => 100;
        public float VerticalHeight => 30 * Math.Max(1, LineCount);
        public float MinimumHeight => 20 * Math.Max(1, LineCount);
        public float PaddingTop => 0;
        public float PaddingLeft => 0;
        public float PaddingBottom => 0;
        public float PaddingRight => 0;

        public IDictionary<string, Action> ContextMenuControls => null;

        public SprawlSpeedometerSettings Settings { get; set; }
        private SprawlSpeedometerSettingsUI _settingsUI;
        private GameMemoryReader _reader;

        private string _displayText = "0.00 u/s";

        private int LineCount =>
            (Settings.Show2D ? 1 : 0) +
            (Settings.Show3D ? 1 : 0) +
            (Settings.ShowZ ? 1 : 0);

        public SprawlSpeedometerComponent(LiveSplitState state)
        {
            Settings = new SprawlSpeedometerSettings();
            _settingsUI = new SprawlSpeedometerSettingsUI(Settings);
            _reader = new GameMemoryReader();
        }

        public void Dispose()
        {
            _reader?.Detach();
            _settingsUI?.Dispose();
        }

        public Control GetSettingsControl(LayoutMode mode) => _settingsUI;

        public XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

        public void SetSettings(XmlNode settings) => Settings.SetSettings(settings);

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            double[] vel = _reader.GetVelocity();

            double speed2D = Math.Sqrt(vel[0] * vel[0] + vel[1] * vel[1]);
            double speed3D = Math.Sqrt(vel[0] * vel[0] + vel[1] * vel[1] + vel[2] * vel[2]);
            double speedZ = Math.Abs(vel[2]);

            string format = "0." + new string('0', Math.Max(0, Settings.DecimalPlaces));

            List<string> lines = new List<string>();

            // Show a line for every enabled speed type. If not attached, show "-" instead.
            if (Settings.Show2D)
                lines.Add(Settings.ShowLabel
                    ? $"Speed 2D: {(_reader.IsAttached ? speed2D.ToString(format) : "-")} u/s"
                    : (_reader.IsAttached ? speed2D.ToString(format) : "-"));
            if (Settings.Show3D)
                lines.Add(Settings.ShowLabel
                    ? $"Speed 3D: {(_reader.IsAttached ? speed3D.ToString(format) : "-")} u/s"
                    : (_reader.IsAttached ? speed3D.ToString(format) : "-"));
            if (Settings.ShowZ)
                lines.Add(Settings.ShowLabel
                    ? $"Speed Z: {(_reader.IsAttached ? speedZ.ToString(format) : "-")} u/s"
                    : (_reader.IsAttached ? speedZ.ToString(format) : "-"));

            // Fallback: if nothing is selected, show 2D.
            if (lines.Count == 0)
                lines.Add(Settings.ShowLabel ? "Speed 2D: 0.00 u/s" : "0.00 u/s");

            string newText = string.Join("\n", lines);

            if (newText != _displayText)
            {
                _displayText = newText;
                invalidator?.Invalidate(0, 0, (int)width, (int)height);
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            Draw(g, state);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            Draw(g, state);
        }

        private void Draw(Graphics g, LiveSplitState state)
        {
            Color textColor = state.LayoutSettings.TextColor;
            Font font = state.LayoutSettings.TextFont ?? new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);

            float width = g.VisibleClipBounds.Width;
            float height = g.VisibleClipBounds.Height;
            RectangleF rect = new RectangleF(0, 0, width, height);

            // Semi-transparent background so the component is always visible.
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
            {
                g.FillRectangle(bgBrush, rect);
            }

            string[] lines = _displayText.Split('\n');
            int lineCount = Math.Max(1, lines.Length);
            float lineHeight = height / lineCount;

            using (StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            using (Brush textBrush = new SolidBrush(textColor))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    RectangleF lineRect = new RectangleF(0, i * lineHeight, width, lineHeight);
                    g.DrawString(lines[i], font, textBrush, lineRect, format);
                }
            }
        }

        public void RenameComparison(string oldName, string newName) { }
    }
}
