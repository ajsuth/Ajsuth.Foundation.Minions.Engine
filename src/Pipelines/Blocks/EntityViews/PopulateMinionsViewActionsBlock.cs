using Ajsuth.Foundation.Minions.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.PopulateMinionsViewActions)]
	public class PopulateMinionsViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			var actionsPolicy = context.GetPolicy<KnownMinionsActionsPolicy>();
			var viewsPolicy = context.GetPolicy<KnownMinionsViewsPolicy>();

			if (string.IsNullOrEmpty(entityView?.Name) ||
					!entityView.Name.Equals(viewsPolicy.Minions, StringComparison.OrdinalIgnoreCase))
			{
				return Task.FromResult(entityView);
			}

			var actions = entityView.GetPolicy<ActionsPolicy>().Actions;
			actions.Add(new EntityActionView()
			{
				Name = actionsPolicy.RunMinion,
				DisplayName = "Run Minion",
				Description = "Runs a Minion",
				IsEnabled = true,
				ConfirmationMessage = "Run Minion?",
				RequiresConfirmation = true,
				Icon = "clock_forward"
			});
			actions.Add(new EntityActionView()
			{
				Name = actionsPolicy.AddMinion,
				DisplayName = "Add Minion",
				Description = "Adds a Minion",
				IsEnabled = true,
				EntityView = viewsPolicy.Details,
				Icon = "add"
			});
			actions.Add(new EntityActionView()
			{
				Name = actionsPolicy.EditMinion,
				DisplayName = "Edit Minion",
				Description = "Edits a Minion",
				IsEnabled = true,
				EntityView = viewsPolicy.Details,
				Icon = "edit"
			});
			actions.Add(new EntityActionView()
			{
				Name = actionsPolicy.DeleteMinion,
				DisplayName = "Delete Minion",
				Description = "Deletes a Minion",
				IsEnabled = true,
				ConfirmationMessage = "Delete Minion?",
				RequiresConfirmation = true,
				Icon = "delete"
			});

			return Task.FromResult(entityView);
		}

		public PopulateMinionsViewActionsBlock()
		  : base(null)
		{
		}
	}
}
