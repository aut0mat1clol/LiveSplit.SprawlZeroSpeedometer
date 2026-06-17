using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

namespace LiveSplit.SprawlSpeedometer
{
    public class SprawlSpeedometerFactory : IComponentFactory
    {
        public string ComponentName => "SPRAWL zero Speedometer";

        public string Description => "Speedometer for SPRAWL zero.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new SprawlSpeedometerComponent(state);

        public string UpdateName => ComponentName;

        public string UpdateURL => "";

        public string XMLURL => "";

        public Version Version => Version.Parse("1.0.0");
    }
}
