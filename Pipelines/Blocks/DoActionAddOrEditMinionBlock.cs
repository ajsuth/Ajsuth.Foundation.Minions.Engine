using Ajsuth.Foundation.Minions.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.DoActionAddOrEditMinion)]
	public class DoActionAddOrEditMinionBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		private readonly CommerceCommander _commerceCommander;

		public DoActionAddOrEditMinionBlock(CommerceCommander commerceCommander)
		  : base(null)
		{
			_commerceCommander = commerceCommander;
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			if (string.IsNullOrEmpty(entityView?.Action) ||
					!(entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().AddMinion, StringComparison.OrdinalIgnoreCase) ||
					entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().EditMinion, StringComparison.OrdinalIgnoreCase)))
			{
				return entityView;
			}

			var entityId = entityView.Properties.FirstOrDefault(p => p.Name.Equals("EntityId", StringComparison.OrdinalIgnoreCase))?.Value;
			var environmentName = entityView.Properties.FirstOrDefault(p => p.Name.Equals("EnvironmentName", StringComparison.OrdinalIgnoreCase))?.Value;
			var policyId = entityView.Properties.FirstOrDefault(p => p.Name.Equals("PolicyId", StringComparison.OrdinalIgnoreCase))?.Value;
			var fullyQualifiedName = entityView.Properties.FirstOrDefault(p => p.Name.Equals("FullyQualifiedName", StringComparison.OrdinalIgnoreCase))?.Value;
			var listToWatch = entityView.Properties.FirstOrDefault(p => p.Name.Equals("ListToWatch", StringComparison.OrdinalIgnoreCase))?.Value;
			var wakeupInterval = entityView.Properties.FirstOrDefault(p => p.Name.Equals("WakeupInterval", StringComparison.OrdinalIgnoreCase))?.Value;
			var itemsPerBatch = entityView.Properties.FirstOrDefault(p => p.Name.Equals("ItemsPerBatch", StringComparison.OrdinalIgnoreCase))?.Value;
			var sleepBetweenBatches = entityView.Properties.FirstOrDefault(p => p.Name.Equals("SleepBetweenBatches", StringComparison.OrdinalIgnoreCase))?.Value;
			var minionPolicy = new MinionPolicy(fullyQualifiedName, listToWatch, TimeSpan.Parse(wakeupInterval));
			minionPolicy.PolicyId = policyId;
			minionPolicy.ItemsPerBatch = int.Parse(itemsPerBatch);
			minionPolicy.SleepBetweenBatches = int.Parse(sleepBetweenBatches);

			context.CommerceContext.Environment = context.CommerceContext.GlobalEnvironment;

			await _commerceCommander.Command<Commands.AddPolicyToEntityCommand>().Process(context.CommerceContext, entityId, minionPolicy, environmentName);

			return entityView;
		}
	}
}
