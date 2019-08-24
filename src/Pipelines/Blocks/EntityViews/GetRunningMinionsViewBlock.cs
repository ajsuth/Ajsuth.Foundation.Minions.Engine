using Ajsuth.Foundation.Minions.Engine.Models;
using Ajsuth.Foundation.Minions.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.GetMinionsDashboardView)]
	public class GetRunningMinionsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		public GetRunningMinionsViewBlock()
		  : base(null)
		{
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");

			var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
			var viewsPolicy = context.GetPolicy<KnownMinionsViewsPolicy>();
			if (string.IsNullOrEmpty(entityViewArgument?.ViewName) ||
					!entityViewArgument.ViewName.Equals(viewsPolicy.MinionsDashboard, StringComparison.OrdinalIgnoreCase))
			{
				return entityView;
			}

			var runningMinionsView = new EntityView()
			{
				Name = viewsPolicy.RunningMinions,
				Icon = "information",
				UiHint = "Table"
			};
			entityView.ChildViews.Add(runningMinionsView);

			var minionPolicies = context.CommerceContext.Environment.GetPolicies<MinionPolicy>();
			foreach (var minionPolicy in context.CommerceContext.Environment.RunningMinions)
			{
				var minionRunModel = (minionPolicy.Models.LastOrDefault(model => model is MinionRunModel) as MinionRunModel);
				var minionView = new EntityView()
				{
					ItemId = minionPolicy.PolicyId,
					Name = minionPolicy.PolicyId,
					Icon = "calendar_clock"
				};
				minionView.Properties.Add(new ViewProperty()
				{
					Name = nameof(minionPolicy.PolicyId),
					RawValue = minionPolicy.PolicyId,
					IsReadOnly = true
				});
				minionView.Properties.Add(new ViewProperty()
				{
					Name = "Minion Start Time",
					RawValue = minionRunModel?.LastStartTime,
					IsReadOnly = true
				});
				minionView.Properties.Add(new ViewProperty()
				{
					Name = "Items Processed",
					RawValue = minionRunModel?.ItemsProcessed,
					IsReadOnly = true
				});
				minionView.Properties.Add(new ViewProperty()
				{
					Name = "Minion Entities",
					RawValue = minionPolicy.Entities,
					IsReadOnly = true,
					UiType = "List"
				});
				runningMinionsView.ChildViews.Add(minionView);
			}

			return await Task.FromResult(entityView);
		}
	}
}
