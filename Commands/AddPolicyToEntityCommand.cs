using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using System;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Commands
{
	public class AddPolicyToEntityCommand : CommerceCommand
	{
		private readonly IAddPolicyToEntityPipeline _addPolicyPipeline;
		private readonly IFindEntityPipeline _findEntity;
		private readonly Commands.GetEnvironmentCommand _getEnvironment;

		public AddPolicyToEntityCommand(IAddPolicyToEntityPipeline addPolicyToEntityPipeline, IFindEntityPipeline findEntityPipeline, Commands.GetEnvironmentCommand getEnvironment, IServiceProvider serviceProvider)
		  : base(serviceProvider)
		{
			_addPolicyPipeline = addPolicyToEntityPipeline;
			_findEntity = findEntityPipeline;
			_getEnvironment = getEnvironment;
		}

		public virtual async Task<CommerceEntity> Process(CommerceContext commerceContext, string entityId, Policy policy, string environmentName = "")
		{
			commerceContext.Logger.LogInformation(string.Format("AddPolicyToEntityCommand:{0}:{1}", entityId, policy.GetType().FullName), Array.Empty<object>());
			using (CommandActivity.Start(commerceContext, this))
			{
				CommerceEnvironment commerceEnvironment = null;
				if (!string.IsNullOrEmpty(environmentName))
				{
					commerceEnvironment = await _getEnvironment.Process(commerceContext, environmentName);
				}

				if (commerceEnvironment == null)
				{
					commerceEnvironment = commerceContext.Environment;
				}

				CommercePipelineExecutionContextOptions context = commerceContext.PipelineContextOptions;
				context.CommerceContext.Environment = commerceEnvironment;
				CommerceEntity entity = await _findEntity.Run(new FindEntityArgument(typeof(CommerceEntity), entityId, false), context);
				if (entity != null)
				{
					return await _addPolicyPipeline.Run(new AddPolicyToEntityArgument(entity, policy), context);
				}

				await commerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().Error,
													"EntityNotFound",
													new object[1] { entityId },
													$"Entity {entityId} was not found");
				return null;
			}
		}
	}
}