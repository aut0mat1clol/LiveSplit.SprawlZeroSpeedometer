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

        public string UpdateURL => "https://raw.githubusercontent.com/aut0mat1clol/LiveSplit.SprawlZeroSpeedometer/master/";

        public string XMLURL => "https://raw.githubusercontent.com/aut0mat1clol/LiveSplit.SprawlZeroSpeedometer/master/Components/update.xml";

        public Version Version => Version.Parse("1.0.0");
    }
}
