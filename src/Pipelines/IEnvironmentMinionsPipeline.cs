// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnvironmentMinionsPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Pipelines
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the IEnvironmentMinionsPipeline interface
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Framework.Pipelines.IPipeline{System.String,
	///         System.Collections.Generic.IEnumerable{Sitecore.Commerce.Core.MinionPolicy}, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
	///     </cref>
	/// </seealso>
	[PipelineDisplayName(MinionsConstants.Pipelines.EnvironmentMinions)]
    public interface IEnvironmentMinionsPipeline : IPipeline<string, IEnumerable<MinionPolicy>, CommercePipelineExecutionContext>
    {
    }
}
