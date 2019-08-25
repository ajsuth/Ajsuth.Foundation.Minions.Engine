// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
    using Microsoft.AspNetCore.OData.Builder;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a block which configures the OData model
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(MinionsConstants.Pipelines.Blocks.ConfigureOpsServiceApi)]
    public class ConfigureOpsServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

			// Add the entities

			// Add the entity sets

			// Add complex types

			// Add unbound functions

			var environmentMinionsFunction = modelBuilder.Function("EnvironmentMinions");
			environmentMinionsFunction.ReturnsCollection<MinionPolicy>();

			var runningMinionsFunction = modelBuilder.Function("RunningMinions");
			runningMinionsFunction.ReturnsCollection<MinionPolicy>();

			// Add unbound actions

			return Task.FromResult(modelBuilder);
        }
    }
}