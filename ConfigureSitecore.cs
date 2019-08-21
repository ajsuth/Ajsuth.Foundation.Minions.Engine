// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Policies
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
	using Sitecore.Commerce.EntityViews;
	using Sitecore.Commerce.Plugin.BusinessUsers;
	using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

			services.Sitecore().Pipelines(config => config
			
				.ConfigurePipeline<IBizFxNavigationPipeline>(pipeline => pipeline
					.Add<Pipelines.Blocks.GetMinionsNavigationViewBlock>().After<GetNavigationViewBlock>()
				)
				
				.ConfigurePipeline<IGetEntityViewPipeline>(pipeline => pipeline
					.Add<Pipelines.Blocks.GetMinionsDashboardViewBlock>().After<PopulateEntityVersionBlock>()
					.Add<Pipelines.Blocks.GetRunningMinionsViewBlock>().After<PopulateEntityVersionBlock>()
					.Add<Pipelines.Blocks.GetMinionDetailsViewBlock>().After<Pipelines.Blocks.GetMinionsDashboardViewBlock>()
				)
				
				.ConfigurePipeline<IPopulateEntityViewActionsPipeline>(pipeline => pipeline
					.Add<Pipelines.Blocks.PopulateMinionsViewActionsBlock>().After<InitializeEntityViewActionsBlock>()
				)

				.ConfigurePipeline<IDoActionPipeline>(pipeline => pipeline
					.Add<Pipelines.Blocks.DoActionRunMinionBlock>().After<ValidateEntityVersionBlock>()
					.Add<Pipelines.Blocks.DoActionAddOrEditMinionBlock>().After<ValidateEntityVersionBlock>()
					.Add<Pipelines.Blocks.DoActionDeleteMinionBlock>().After<ValidateEntityVersionBlock>()
				)
			);

			services.RegisterAllCommands(assembly);
        }
    }
}