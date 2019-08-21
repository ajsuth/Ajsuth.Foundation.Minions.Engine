using Sitecore.Commerce.Core;

namespace Ajsuth.Foundation.Minions.Engine.Policies
{
	public class KnownEnvironmentsPolicy : Policy
	{
		public KnownEnvironmentsPolicy()
		{
			MinionsEnvironment = "HabitatMinions";
		}

		public string MinionsEnvironment { get; set; }
	}
}
