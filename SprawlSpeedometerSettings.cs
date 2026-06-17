using System;
using System.Xml;

namespace LiveSplit.SprawlSpeedometer
{
    public class SprawlSpeedometerSettings
    {
        public bool ShowLabel { get; set; } = true;
        public bool Show2D { get; set; } = true;
        public bool Show3D { get; set; } = false;
        public bool ShowZ { get; set; } = false;
        public int DecimalPlaces { get; set; } = 2;

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement parent = document.CreateElement("Settings");
            CreateSetting(document, parent, "ShowLabel", ShowLabel.ToString());
            CreateSetting(document, parent, "Show2D", Show2D.ToString());
            CreateSetting(document, parent, "Show3D", Show3D.ToString());
            CreateSetting(document, parent, "ShowZ", ShowZ.ToString());
            CreateSetting(document, parent, "DecimalPlaces", DecimalPlaces.ToString());
            return parent;
        }

        public void SetSettings(XmlNode settings)
        {
            if (settings == null) return;

            foreach (XmlNode node in settings.ChildNodes)
            {
                if (node.Name != "Setting") continue;

                string name = node.Attributes?["name"]?.Value;
                string value = node.Attributes?["value"]?.Value;
                if (name == null || value == null) continue;

                switch (name)
                {
                    case "ShowLabel":
                        if (bool.TryParse(value, out bool showLabel)) ShowLabel = showLabel;
                        break;
                    case "Show2D":
                        if (bool.TryParse(value, out bool show2D)) Show2D = show2D;
                        break;
                    case "Show3D":
                        if (bool.TryParse(value, out bool show3D)) Show3D = show3D;
                        break;
                    case "ShowZ":
                        if (bool.TryParse(value, out bool showZ)) ShowZ = showZ;
                        break;
                    case "DecimalPlaces":
                        if (int.TryParse(value, out int decimals)) DecimalPlaces = decimals;
                        break;
                }
            }
        }

        private static void CreateSetting(XmlDocument document, XmlElement parent, string name, string value)
        {
            XmlElement setting = document.CreateElement("Setting");
            setting.SetAttribute("name", name);
            setting.SetAttribute("value", value);
            parent.AppendChild(setting);
        }
    }
}
