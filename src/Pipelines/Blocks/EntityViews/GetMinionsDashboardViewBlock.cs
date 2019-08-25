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
	public class GetMinionsDashboardViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		public GetMinionsDashboardViewBlock()
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
				return await Task.FromResult(entityView).ConfigureAwait(false);
			}

			entityView.Icon = "windows";

			var minionsView = new EntityView()
			{
				Name = viewsPolicy.Minions,
				Icon = "calendar_clock",
				UiHint = "List"
			};
			entityView.ChildViews.Add(minionsView);
			
			var minionPolicies = context.CommerceContext.Environment.GetPolicies<MinionPolicy>();
			foreach (var minionPolicy in minionPolicies)
			{
				var minionView = CreateMinionEntityView(minionsView, minionPolicy, context);
				minionsView.ChildViews.Add(minionView);
				if (minionPolicy is MinionBossPolicy)
				{
					foreach (var childMinionPolicy in ((MinionBossPolicy)minionPolicy).Children)
					{
						// TODO: Add parent/children details for MinionBossPolicies
						var childMinionView = CreateMinionEntityView(minionsView, childMinionPolicy, context);
						minionsView.ChildViews.Add(childMinionView);
					}
				}
			}

			return await Task.FromResult(entityView).ConfigureAwait(false);
		}

		protected virtual EntityView CreateMinionEntityView(EntityView minionsView, MinionPolicy minionPolicy, CommercePipelineExecutionContext context)
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
				Name = nameof(minionPolicy.FullyQualifiedName),
				RawValue = minionPolicy.FullyQualifiedName,
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = nameof(minionPolicy.ListToWatch),
				RawValue = minionPolicy.ListToWatch,
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = "Minion Entities",
				RawValue = minionPolicy.Entities,
				IsReadOnly = true,
				UiType = "List"
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = nameof(minionPolicy.WakeupInterval),
				RawValue = minionPolicy.WakeupInterval != null ? ((TimeSpan)minionPolicy.WakeupInterval).ToString("c") : "",
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = nameof(minionPolicy.ItemsPerBatch),
				RawValue = minionPolicy.ItemsPerBatch,
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = nameof(minionPolicy.SleepBetweenBatches),
				RawValue = minionPolicy.SleepBetweenBatches,
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = "Last Run Time",
				RawValue = minionRunModel?.LastStartTime.ToString() ?? "N/A",
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = "Last Items Processed Count",
				RawValue = minionRunModel?.ItemsProcessed,
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = "Run Lapsed Time",
				RawValue = minionRunModel != null ? ((minionRunModel.CompletedTime ?? DateTime.UtcNow) - minionRunModel.LastStartTime).ToString() : "N/A",
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = "Status",
				RawValue = minionRunModel?.Status ?? "N/A",
				IsReadOnly = true
			});
			minionView.Properties.Add(new ViewProperty()
			{
				Name = "Completion Message",
				RawValue = minionRunModel?.Message ?? "N/A",
				IsReadOnly = true
			});

			return minionView;
		}
	}
}
