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
		protected readonly CommerceCommander Commander;

		public DoActionRunMinionBlock(CommerceCommander commerceCommander)
		  : base(null)
		{
			Commander = commerceCommander;
		}

		public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			if (string.IsNullOrEmpty(entityView?.Action) ||
					!entityView.Action.Equals(context.GetPolicy<KnownMinionsActionsPolicy>().RunMinion, StringComparison.OrdinalIgnoreCase))
			{
				return entityView;
			}
			
			var minionPolicy = context.CommerceContext.Environment.GetPolicies<MinionPolicy>().FirstOrDefault(p => p.PolicyId == entityView.ItemId);
			if (minionPolicy == null)
			{
				await context.CommerceContext.AddMessage(
					context.GetPolicy<KnownResultCodes>().ValidationError,
					"InvalidOrMissingPropertyValue",
					new object[]
					{
						entityView.ItemId
					},
					$"Invalid or missing value for property 'ItemId'. ItemId = {entityView.ItemId}").ConfigureAwait(false);

				return entityView;
			}

			await Commander.Command<RunMinionCommand>().Process(
				context.CommerceContext,
				minionPolicy.FullyQualifiedName,
				context.CommerceContext.Environment.Name,
				new List<Policy> {
					new RunMinionPolicy { WithListToWatch = minionPolicy.ListToWatch }
				}).ConfigureAwait(false);

			return entityView;
		}
	}
}
