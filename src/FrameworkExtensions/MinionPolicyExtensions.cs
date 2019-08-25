using Ajsuth.Foundation.Minions.Engine.Models;
using Sitecore.Commerce.Core;
using System.Linq;

namespace Ajsuth.Foundation.Minions.Engine.FrameworkExtensions
{
	public static class MinionPolicyExtensions
	{
		public static MinionRunModel CurrentRunModel(this MinionPolicy minionPolicy)
		{
			return minionPolicy.Models.FirstOrDefault(m => m is MinionRunModel) as MinionRunModel;
		}

		public static void ClearRunModels(this MinionPolicy minionPolicy)
		{
			minionPolicy.Models.RemoveAll(m => m is MinionRunModel);
		}

		public static MinionRunModel CreateRunModel(this MinionPolicy minionPolicy)
		{
			var minionRunModel = new MinionRunModel();
			minionPolicy.Models.Add(minionRunModel);
			return minionRunModel;
		}
	}
}
