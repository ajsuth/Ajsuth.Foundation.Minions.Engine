namespace Ajsuth.Foundation.Minions.Engine
{
	/// <summary>
	/// The minions constants.
	/// </summary>
	public class MinionsConstants
    {
		/// <summary>
		/// The names of the minions pipelines.
		/// </summary>
		public static class Pipelines
		{
			/// <summary>
			/// The environment minions pipeline name.
			/// </summary>
			public const string EnvironmentMinions = "Minions.Pipeline.EnvironmentMinions";

			/// <summary>
			/// The running minions pipeline name.
			/// </summary>
			public const string RunningMinions = "Minions.Pipeline.RunningMinions";

			/// <summary>
			/// The names of the minions pipeline blocks.
			/// </summary>
			public static class Blocks
			{
				/// <summary>
				/// The configure ops service api block name.
				/// </summary>
				public const string ConfigureOpsServiceApi = "Minions.Block.ConfigureOpsServiceApiBlock";

				/// <summary>
				/// The do action add or edit minion block name.
				/// </summary>
				public const string DoActionAddOrEditMinion = "Minions.Block.DoActionAddOrEditMinion";

				/// <summary>
				/// The do action delete minion block name.
				/// </summary>
				public const string DoActionDeleteMinion = "Minions.Block.DoActionDeleteMinion";
				
				/// <summary>
				/// The do action run minion block name.
				/// </summary>
				public const string DoActionRunMinion = "Minions.Block.DoActionRunMinion";

				/// <summary>
				/// The environment minions block name.
				/// </summary>
				public const string EnvironmentMinions = "Minions.Block.EnvironmentMinions";

				/// <summary>
				/// The get minion details view block name.
				/// </summary>
				public const string GetMinionDetailsView = "Minions.Block.GetMinionDetailsView";

				/// <summary>
				/// The get minions dashboard view block name.
				/// </summary>
				public const string GetMinionsDashboardView = "Minions.Block.GetMinionsDashboardView";

				/// <summary>
				/// The get minions navigation view block name.
				/// </summary>
				public const string GetMinionsNavigationView = "Minions.Block.GetMinionsNavigationView";

				/// <summary>
				/// The populate minions view actions block name.
				/// </summary>
				public const string PopulateMinionsViewActions = "Minions.Block.PopulateMinionsViewActions";

				/// <summary>
				/// The running minions block name.
				/// </summary>
				public const string RunningMinions = "Minions.Block.RunningMinions";
			}
		}
	}
}
