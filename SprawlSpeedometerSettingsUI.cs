using System;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.SprawlSpeedometer
{
    public partial class SprawlSpeedometerSettingsUI : UserControl
    {
        private SprawlSpeedometerSettings _settings;

        private CheckBox chkShowLabel;
        private CheckBox chkShow2D;
        private CheckBox chkShow3D;
        private CheckBox chkShowZ;
        private NumericUpDown numDecimals;

        public SprawlSpeedometerSettingsUI(SprawlSpeedometerSettings settings)
        {
            _settings = settings;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            chkShowLabel = new CheckBox
            {
                Text = "Show labels",
                Location = new Point(10, 10),
                Width = 260,
                Checked = _settings.ShowLabel
            };
            chkShowLabel.CheckedChanged += (s, e) => _settings.ShowLabel = chkShowLabel.Checked;

            chkShow2D = new CheckBox
            {
                Text = "Show 2D speed",
                Location = new Point(10, 40),
                Width = 260,
                Checked = _settings.Show2D
            };
            chkShow2D.CheckedChanged += (s, e) => _settings.Show2D = chkShow2D.Checked;

            chkShow3D = new CheckBox
            {
                Text = "Show 3D speed",
                Location = new Point(10, 70),
                Width = 260,
                Checked = _settings.Show3D
            };
            chkShow3D.CheckedChanged += (s, e) => _settings.Show3D = chkShow3D.Checked;

            chkShowZ = new CheckBox
            {
                Text = "Show Z (vertical) speed",
                Location = new Point(10, 100),
                Width = 260,
                Checked = _settings.ShowZ
            };
            chkShowZ.CheckedChanged += (s, e) => _settings.ShowZ = chkShowZ.Checked;

            Label lblDecimals = new Label
            {
                Text = "Decimal places:",
                Location = new Point(10, 135),
                Width = 90
            };
            numDecimals = new NumericUpDown
            {
                Location = new Point(110, 133),
                Width = 50,
                Minimum = 0,
                Maximum = 6,
                Value = _settings.DecimalPlaces
            };
            numDecimals.ValueChanged += (s, e) => _settings.DecimalPlaces = (int)numDecimals.Value;

            this.Controls.Add(chkShowLabel);
            this.Controls.Add(chkShow2D);
            this.Controls.Add(chkShow3D);
            this.Controls.Add(chkShowZ);
            this.Controls.Add(lblDecimals);
            this.Controls.Add(numDecimals);

            this.Name = "SprawlSpeedometerSettingsUI";
            this.Size = new Size(300, 170);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
