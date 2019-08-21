using Sitecore.Commerce.Core;

namespace Ajsuth.Foundation.Minions.Engine.Policies
{
	public class KnownMinionsViewsPolicy : Policy
	{
		public KnownMinionsViewsPolicy()
		{
			MinionsDashboard = nameof(MinionsDashboard);
			Minions = nameof(Minions);
			RunningMinions = nameof(RunningMinions);
			Details = nameof(Details);
		}

		public string MinionsDashboard { get; set; }

		public string Minions { get; set; }

		public string RunningMinions { get; set; }

		public string Details { get; set; }
	}
}
