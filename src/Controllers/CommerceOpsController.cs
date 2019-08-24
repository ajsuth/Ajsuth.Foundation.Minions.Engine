// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceOpsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Minions.Engine.Controllers
{
	using Ajsuth.Foundation.Minions.Engine.Commands;
	using Microsoft.AspNetCore.Mvc;
    using Sitecore.Commerce.Core;
    using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
    using System.Web.Http.OData;

    /// <inheritdoc />
    /// <summary>
    /// Defines an commerce ops controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    [Route("commerceops")]
    public class CommerceOpsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ajsuth.Foundation.Minions.Engine.Controllers.CommerceOpsController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommerceOpsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

		/// <summary>
		/// Running minions.
		/// </summary>
		/// <returns>A list of <see cref="MinionPolicy" /></returns>
		[HttpGet]
		[Route("RunningMinions")]
		public async Task<IActionResult> RunningMinions()
		{
			var result = await this.Command<RunningMinionsCommand>().Process(this.CurrentContext);
			return result == null
				? new ObjectResult(new List<MinionPolicy>())
				: new ObjectResult(result);
		}

	}
}