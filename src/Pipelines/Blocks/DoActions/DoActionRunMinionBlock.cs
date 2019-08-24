using Microsoft.Extensions.Logging;
using Ajsuth.Foundation.Minions.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.DoActionRunMinion)]
	public class DoActionRunMinionBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		private readonly CommerceCommander _commerceCommander;

		public DoActionRunMinionBlock(CommerceCommander commerceCommander)
		  : base(null)
		{
			_commerceCommander = commerceCommander;
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			if (string.IsNullOrEmpty(entityView?.Action) ||
					!entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().RunMinion, StringComparison.OrdinalIgnoreCase))
			{
				return entityView;
			}

			try
			{
				var minionPolicy = context.CommerceContext.Environment.GetPolicies<MinionPolicy>().FirstOrDefault(p => p.PolicyId == entityView.ItemId);
				if (minionPolicy != null)
				{
					var environmentPolicy = context.CommerceContext.Environment.GetPolicy<KnownEnvironmentsPolicy>();
					var commandResult = await _commerceCommander.Command<RunMinionCommand>()
							.Process(context.CommerceContext,
										minionPolicy.FullyQualifiedName,
										environmentPolicy.MinionsEnvironment,
										new List<Policy> {
											new RunMinionPolicy { WithListToWatch = minionPolicy.ListToWatch }
										});
				}
				else
				{
					context.Logger.LogError($"Minions.DoActionRunMinionBlock.NoMinionPolicy: Name={entityView.ItemId}");
				}
			}
			catch (Exception ex)
			{
				context.Logger.LogError($"Search.DoActionRebuildScope.Exception: Message={ex.Message}");
			}

			return entityView;
		}
	}
}
