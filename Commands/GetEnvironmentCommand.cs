using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ajsuth.Foundation.Minions.Engine.Commands
{
	public class GetEnvironmentCommand : CommerceCommand
	{
		private readonly IFindEntityPipeline _findEntityPipeline;
		private readonly IStartEnvironmentPipeline _startEnvironmentPipeline;
		private readonly NodeContext _nodeContext;

		public GetEnvironmentCommand(IFindEntityPipeline findEntityPipeline, IStartEnvironmentPipeline startEnvironmentPipeline, IServiceProvider serviceProvider, NodeContext nodeContext)
		  : base(serviceProvider)
		{
			_findEntityPipeline = findEntityPipeline;
			_nodeContext = nodeContext;
			_startEnvironmentPipeline = startEnvironmentPipeline;
		}

		public virtual async Task<CommerceEnvironment> Process(CommerceContext commerceContext, string environmentName)
		{
			using (CommandActivity.Start(commerceContext, this))
			{
				if (environmentName.Equals("GlobalEnvironment", StringComparison.OrdinalIgnoreCase))
				{
					return commerceContext.Environment.Name.Equals("GlobalEnvironment", StringComparison.OrdinalIgnoreCase) ? _nodeContext.Environment : _nodeContext.GlobalEnvironment;
				}

				string environmentKey = $"{CommerceEntity.IdPrefix<CommerceEnvironment>()}{environmentName}";
				var commerceEnvironment = _nodeContext.GetObjects<CommerceEnvironment>().FirstOrDefault(p => p.Id.Equals(environmentKey, StringComparison.OrdinalIgnoreCase));
				if (commerceEnvironment != null)
				{
					return commerceEnvironment;
				}

				commerceContext.Logger.LogInformation(string.Format("GetEnvironmentCommand.AccessUnloadedEnvironment: Environment={0}", environmentKey), Array.Empty<object>());
				commerceEnvironment = (await _findEntityPipeline.Run(new FindEntityArgument(typeof(CommerceEnvironment), environmentKey, false), _nodeContext.PipelineContextOptions)) as CommerceEnvironment;
				
				if (commerceEnvironment == null)
				{
					await commerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().Error,
													"EntityNotFound",
													new object[1] { environmentName },
													$"Entity '{environmentName}' was not found.");
					return null;
				}

				return await _startEnvironmentPipeline.Run(environmentName, _nodeContext.PipelineContextOptions);
			}
		}

		public virtual async Task<CommerceEnvironment> ById(CommerceContext commerceContext, string id)
		{
			using (CommandActivity.Start(commerceContext, this))
			{
				var commerceEnvironment1 = _nodeContext.GetObjects<CommerceEnvironment>().FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
				if (commerceEnvironment1 != null)
				{
					return commerceEnvironment1;
				}

				var findEntityArgument = new FindEntityArgument(typeof(CommerceEnvironment), id, false);
				var commerceEnvironment2 = await _findEntityPipeline.Run(findEntityArgument, commerceContext.PipelineContextOptions) as CommerceEnvironment;
				if (commerceEnvironment2 != null)
					return await _startEnvironmentPipeline.Run(commerceEnvironment2.Name, _nodeContext.PipelineContextOptions);
				await commerceContext.AddMessage(
					commerceContext.GetPolicy<KnownResultCodes>().Error,
					"EntityNotFound",
					new object[1] { id },
					$"Entity '{id}' was not found.");
				return null;
			}
		}
	}
}