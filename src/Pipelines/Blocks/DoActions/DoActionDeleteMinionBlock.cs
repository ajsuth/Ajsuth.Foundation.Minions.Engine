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
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.DoActionDeleteMinion)]
	public class DoActionDeleteMinionBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		private readonly CommerceCommander _commerceCommander;

		public DoActionDeleteMinionBlock(CommerceCommander commerceCommander)
		  : base(null)
		{
			_commerceCommander = commerceCommander;
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			if (string.IsNullOrEmpty(entityView?.Action) ||
					!entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().DeleteMinion, StringComparison.OrdinalIgnoreCase))
			{
				return entityView;
			}
			
			var entityId = context.CommerceContext.Environment.Id;
			context.CommerceContext.Environment = context.CommerceContext.GlobalEnvironment;
			var environmentName = context.CommerceContext.GlobalEnvironment.Name;

			await _commerceCommander.Command<RemovePolicyFromEntityCommand>().Process(context.CommerceContext,
																						entityId,
																						GetMinionPolicyFullyQualifiedName(),
																						entityView.ItemId,
																						environmentName);

			return entityView;
		}

		private string GetMinionPolicyFullyQualifiedName()
		{
			return $"{typeof(MinionPolicy).FullName}, {typeof(MinionPolicy).Namespace}";
		}
	}
}
