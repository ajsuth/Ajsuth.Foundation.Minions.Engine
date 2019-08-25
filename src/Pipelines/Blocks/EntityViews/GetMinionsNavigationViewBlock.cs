using Ajsuth.Foundation.Minions.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.GetMinionsNavigationView)]
	public class GetMinionsNavigationViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		public GetMinionsNavigationViewBlock()
		  : base(null)
		{
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");

			if (entityView.Name != "ToolsNavigation")
			{
				return await Task.FromResult(entityView).ConfigureAwait(false);
			}

			var dashboardName = context.GetPolicy<KnownMinionsViewsPolicy>().MinionsDashboard;
			var minionsDashboardView = new EntityView()
			{
				Name = dashboardName,
				ItemId = dashboardName,
				Icon = "calendar_clock"
			};
			entityView.ChildViews.Add(minionsDashboardView);

			return await Task.FromResult(entityView).ConfigureAwait(false);
		}
    }
}
