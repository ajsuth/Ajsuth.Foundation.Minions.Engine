using Sitecore.Commerce.Core;

namespace Ajsuth.Foundation.Minions.Engine.Policies
{
	public class KnownMinionsActionsPolicy : Policy
	{
		public KnownMinionsActionsPolicy()
		{
			RunMinion = nameof(RunMinion);
			AddMinion = nameof(AddMinion);
			EditMinion = nameof(EditMinion);
			DeleteMinion = nameof(DeleteMinion);
		}

		public string RunMinion { get; set; }

		public string AddMinion { get; set; }

		public string EditMinion { get; set; }

		public string DeleteMinion { get; set; }
	}
}
