// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentMinionsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines a pipeline block
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Framework.Pipelines.PipelineBlock{System.String,
	///         System.Collections.Generic.IEnumerable{Sitecore.Commerce.Core.MinionPolicy}, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
	///     </cref>
	/// </seealso>
	[PipelineDisplayName(MinionsConstants.Pipelines.Blocks.EnvironmentMinions)]
    public class EnvironmentMinionsBlock : PipelineBlock<string, IEnumerable<MinionPolicy>, CommercePipelineExecutionContext>
    {
		/// <summary>
		/// Gets or sets the commander.
		/// </summary>
		/// <value>
		/// The commander.
		/// </value>
		protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public EnvironmentMinionsBlock(CommerceCommander commander)
		    : base(null)
		{
            this.Commander = commander;
        }

		/// <summary>
		/// The run.
		/// </summary>
		/// <param name="arg">
		/// The pipeline argument.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <returns>
		/// The list of <see cref="MinionPolicy"/>.
		/// </returns>
		public override Task<IEnumerable<MinionPolicy>> Run(string arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(context.CommerceContext.Environment.GetPolicies<MinionPolicy>().AsEnumerable());
        }
    }
}