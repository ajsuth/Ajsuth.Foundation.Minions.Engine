using Sitecore.Commerce.Core;

namespace Ajsuth.Foundation.Minions.Engine.Policies
{
	public class KnownMinionsViewsPolicy : Policy
	{
		public KnownMinionsViewsPolicy()
		{
			MinionsDashboard = nameof(MinionsDashboard);
			EnvironmentMinions = nameof(EnvironmentMinions);
			RunningMinions = nameof(RunningMinions);
			Details = nameof(Details);
		}

		public string MinionsDashboard { get; set; }

		public string EnvironmentMinions { get; set; }

		public string RunningMinions { get; set; }

		public string Details { get; set; }
	}
}
