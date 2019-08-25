using Ajsuth.Foundation.Minions.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.DoActionAddOrEditMinion)]
	public class DoActionAddOrEditMinionBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		protected readonly CommerceCommander Commander;

		public DoActionAddOrEditMinionBlock(CommerceCommander commerceCommander)
		  : base(null)
		{
			Commander = commerceCommander;
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			if (string.IsNullOrEmpty(entityView?.Action) ||
					!(entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().AddMinion, StringComparison.OrdinalIgnoreCase) ||
					entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().EditMinion, StringComparison.OrdinalIgnoreCase)))
			{
				return entityView;
			}

			var entityId = entityView.GetPropertyValue("EntityId").ToString();
			var environmentName = entityView.GetPropertyValue("EnvironmentName").ToString();
			var policyId = entityView.GetPropertyValue("PolicyId").ToString();
			var fullyQualifiedName = entityView.GetPropertyValue("FullyQualifiedName").ToString();
			var listToWatch = entityView.GetPropertyValue("ListToWatch").ToString();
			var wakeupInterval = entityView.GetPropertyValue("WakeupInterval").ToString();
			var itemsPerBatch = entityView.GetPropertyValue("ItemsPerBatch").ToString();
			var sleepBetweenBatches = entityView.GetPropertyValue("SleepBetweenBatches").ToString();

			var minionPolicy = new MinionPolicy(fullyQualifiedName, listToWatch, TimeSpan.Parse(wakeupInterval))
			{
				PolicyId = policyId,
				ItemsPerBatch = int.Parse(itemsPerBatch),
				SleepBetweenBatches = int.Parse(sleepBetweenBatches)
			};

			context.CommerceContext.Environment = context.CommerceContext.GlobalEnvironment;

			await Commander.Command<AddPolicyToEntityCommand>().Process(context.CommerceContext, entityId, minionPolicy, environmentName).ConfigureAwait(false);

			return entityView;
		}
	}
}
