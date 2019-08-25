// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentMinionsCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Commands
{
	using Ajsuth.Foundation.Minions.Engine.Pipelines;
	using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <inheritdoc />
	/// <summary>
	/// Defines the EnvironmentMinionsCommand command.
	/// </summary>
	public class EnvironmentMinionsCommand : CommerceCommand
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected CommerceCommander Commander { get; set; }

		/// <inheritdoc />
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ajsuth.Foundation.Minions.Engine.Commands.EnvironmentMinionsCommand" /> class.
		/// </summary>
		/// <param name="pipeline">
		/// The pipeline.
		/// </param>
		/// <param name="serviceProvider">The service provider</param>
		public EnvironmentMinionsCommand(CommerceCommander commander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Commander = commander;
        }

		/// <summary>
		/// The process of the command
		/// </summary>
		/// <param name="commerceContext">
		/// The commerce context
		/// </param>
		/// <returns>
		/// The list of <see cref="MinionPolicy"/>.
		/// </returns>
		public async Task<IEnumerable<MinionPolicy>> Process(CommerceContext commerceContext)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
				return await Commander.Pipeline<IEnvironmentMinionsPipeline>().Run(string.Empty, commerceContext.PipelineContextOptions).ConfigureAwait(false);
            }
        }
    }
}