// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentMinionsPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Pipelines
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
	using System.Collections.Generic;

	/// <inheritdoc />
	/// <summary>
	///  Defines the EnvironmentMinionsPipeline pipeline.
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Commerce.Core.CommercePipeline{System.String,
	///         System.Collections.Generic.IEnumerable{Sitecore.Commerce.Core.MinionPolicy}}
	///     </cref>
	/// </seealso>
	/// <seealso cref="T:Ajsuth.Foundation.Minions.Engine.Pipelines.RunningMinionsPipeline" />
	public class EnvironmentMinionsPipeline : CommercePipeline<string, IEnumerable<MinionPolicy>>, IEnvironmentMinionsPipeline
	{
		/// <inheritdoc />
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ajsuth.Foundation.Minions.Engine.Pipelines.RunningMinionsPipeline" /> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="loggerFactory">The logger factory.</param>
		public EnvironmentMinionsPipeline(IPipelineConfiguration<IEnvironmentMinionsPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

