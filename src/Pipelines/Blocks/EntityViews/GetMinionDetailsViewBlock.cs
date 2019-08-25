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
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.GetMinionDetailsView)]
	public class GetMinionDetailsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		public GetMinionDetailsViewBlock()
			: base(null)
		{
		}
		
		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

			var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
			var viewsPolicy = context.GetPolicy<KnownMinionsViewsPolicy>();
			var actionsPolicy = context.GetPolicy<KnownMinionsActionsPolicy>();
			var isEditAction = entityViewArgument.ForAction.Equals(actionsPolicy.EditMinion, StringComparison.OrdinalIgnoreCase);
			if (string.IsNullOrEmpty(entityViewArgument?.ViewName) ||
					!entityViewArgument.ViewName.Equals(viewsPolicy.Details, StringComparison.OrdinalIgnoreCase) ||
					!(entityViewArgument.ForAction.Equals(actionsPolicy.AddMinion, StringComparison.OrdinalIgnoreCase) ||
					isEditAction))
			{
				return await Task.FromResult(entityView).ConfigureAwait(false);
			}
			
			var minionPolicy = isEditAction ? GetMinionPolicy(entityView.ItemId, context) : null;
			PopulateDetails(entityView, minionPolicy, context);

			return await Task.FromResult(entityView).ConfigureAwait(false);
		}

		protected virtual MinionPolicy GetMinionPolicy(string policyId, CommercePipelineExecutionContext context)
		{
			return context.CommerceContext.Environment.GetPolicies<MinionPolicy>().FirstOrDefault(p => p.PolicyId == policyId);
		}

		protected virtual void PopulateDetails(EntityView view, MinionPolicy minionPolicy, CommercePipelineExecutionContext context)
		{
			if (view == null)
			{
				return;
			}

			if (minionPolicy == null)
			{
				minionPolicy = new MinionPolicy(string.Empty, string.Empty);
			}

			view.Properties.Add(new ViewProperty()
			{
				Name = nameof(minionPolicy.PolicyId),
				RawValue = minionPolicy.PolicyId ?? Guid.NewGuid().ToString("N"),
				IsReadOnly = true
			});

			view.Properties.Add(new ViewProperty()
			{
				Name = "EntityId",
				RawValue = context.CommerceContext.Environment.Id,
				IsReadOnly = true
			});

			view.Properties.Add(new ViewProperty()
			{
				Name = "EnvironmentName",
				RawValue = context.CommerceContext.GlobalEnvironment.Name,
				IsReadOnly = true
			});

			view.Properties.Add(new ViewProperty() { Name = nameof(minionPolicy.FullyQualifiedName), RawValue = minionPolicy.FullyQualifiedName });
			view.Properties.Add(new ViewProperty() { Name = nameof(minionPolicy.ListToWatch), RawValue = minionPolicy.ListToWatch, IsRequired = false });
			view.Properties.Add(new ViewProperty() { Name = nameof(minionPolicy.WakeupInterval), RawValue = minionPolicy.WakeupInterval != null ? ((TimeSpan)minionPolicy.WakeupInterval).ToString("c") : "", IsRequired = false });
			view.Properties.Add(new ViewProperty() { Name = nameof(minionPolicy.ItemsPerBatch), RawValue = minionPolicy.ItemsPerBatch });
			view.Properties.Add(new ViewProperty() { Name = nameof(minionPolicy.SleepBetweenBatches), RawValue = minionPolicy.SleepBetweenBatches });

		}
	}
}