# Extended Sitecore Commerce Minions
A custom plugin to extend minions functionality in Sitecore Experience Commerce.

- [Supported Sitecore Experience Commerce Versions](#supported-sitecore-experience-commerce-versions)
- [Features](#features)
- [Installation Instructions (API Only)](#installation-instructions-api-only)
- [Installation Instructions (Extended Run Time Properties)](#installation-instructions-extended-run-time-properties)
- [Installation Instructions (BizFx Module)](#installation-instructions-bizfx-module)
- [Known Issues](#known-issues)
- [Disclaimer](#disclaminer)

## Supported Sitecore Experience Commerce Versions
- XC 9.2
- XC 9.1 (Go here for XC 9.1 documentation)

## Features

- [Running Minions API](#running-minions-api)
- [Environment Minions API](#environment-minions-api)
- [Extended Minion Run Time Properties](#extended-minion-run-time-properties)
- [Improved Logging For Blocked Minions](#improved-logging-for-blocked-minions)
- [Minions Dashboard](#minions-dashboard)
- [Running Minions Entity View](#running-minions-entity-view)

### Running Minions API
The ``/commerceops/RunningMinions()`` returns a list of minion policies for minions that are currently running.
![Running Minions API](/images/running-minions-api.png)

### Environment Minions API
The ``/commerceops/EnvironmentMinions()`` returns a list of minion policies for the environment.
![Running Minions API](/images/environment-minions-api.png)

### Extended Minion Run Time Properties
The ``MinionRunModel`` captures additional data for the last minion execution for display in the Running Minions and Minons Dashboard entity views.

**Note:** Currently only the last execution data is captured to avoid a memory leak.

### Improved Logging For Blocked Minions
When a minion is blocked due to another minion utilising the same commerce entities, the logged message provides the name of the blocking minion. e.g.
> "Minion 'Sitecore.Commerce.Plugin.Search.IncrementalIndexMinion, Sitecore.Commerce.Plugin.Search' (ListToWatch: CatalogItemsIndex) didn't run. Minion 'Sitecore.Commerce.Plugin.Search.FullIndexMinion, Sitecore.Commerce.Plugin.Search' (ListToWatch: CatalogItems) is processing the same type of entities (Sitecore.Commerce.Plugin.Catalog.Catalogs, Sitecore.Commerce.Plugin.Catalog,Sitecore.Commerce.Plugin.Catalog.Category, Sitecore.Commerce.Plugin.Catalog,Sitecore.Commerce.Plugin.Catalog.SellableItem, Sitecore.Commerce.Plugin.Catalog) in the 'HabitatMinions' environment running."

### Minions Dashboard
Provides an overview of minions. Last run time details will render for custom minions. See [Updating Minions to obtain run time data](#updating-minions-to-obtain-run-time-data).
![Minions Dashboard](/images/minions-dashboard.png)

### Running Minions Entity View
Renders currently running minions in BizFx.
![Running Minions](/images/running-minions.png)

### Minion Actions
Added minion actions

**Note:** Add/Edit/Remove minions are not fully tested and not intended for production as it is recommended that policies be managed and bootstrapped from the policy configuration files for policy back ups and version control.

![Minion Dashboard Actions](/images/minion-dashboard-actions.png)

## Installation Instructions (API Only)

### Adding the plugin to your solution
1. Download the repository.
2. Add the **Ajsuth.Foundation.Minions.Engine.csproj** to the _**Sitecore Commerce Engine**_ solution.
3. In the _**Sitecore Commerce Engine**_ project, add a reference to the **Ajsuth.Foundation.Minions.Engine** project.
4. Deploy the solution and run from IIS.

## Installation Instructions (Extended Run Time Properties)

**Prerequisites:** [Installation Instructions (API Only)](#installation-instructions-api-only).

### Updating Minions to obtain run time data

To track the last run time data of a minion, the minion will need to be overridden in your solution. The following instructions provides a guide for capturing this data.

1. Create a new minion in your solution, inheriting from the desired minion if it's not custom.
2. Add the following using statement to the minion class

``using Ajsuth.Foundation.Minions.Engine.FrameworkExtensions;``

3. Override the ``Process()`` method, adding the following code to the beginning of the method.

```
public override async Task<MinionRunResultsModel> Process()
{
	this.Policy.ClearRunModels();
	this.Policy.CreateRunModel();
	if (!(await this.ShouldProcess().ConfigureAwait(false)))
	{
		return this.Policy.CurrentRunModel();
	}

	if (!this.Environment.AddRunningMinion(this))
	{
		return this.Policy.CurrentRunModel();
	}

	MinionRunModel runModel;
	try
	{
		var result = await this.Execute().ConfigureAwait(false);
		if (this.ShouldDispose)
		{
			this.Logger.LogInformation($"Disposing of minion '{this.Policy.FullyQualifiedName}|{this.Policy.ListToWatch}|{this.Environment.Name}' after finishing executing.");
			this.Dispose();
		}

		return result;
	}
	catch(MinionExecutionException ex)
	{
		runModel = this.Policy.CurrentRunModel();
		runModel.Abort(ex.Message);
		return runModel;
	}
	finally
	{
		this.Environment.RemoveRunningMinion(this);
	}
}
```

4. Override the ``ShouldProcess()`` method with the following

```
protected override async Task<bool> ShouldProcess()
{
	// minion with the same configuration
	if (this.Environment.RunningMinions.FirstOrDefault(p => p.FullyQualifiedName.Equals(this.Policy.FullyQualifiedName, StringComparison.OrdinalIgnoreCase) && p.ListToWatch.Equals(this.Policy.ListToWatch, StringComparison.OrdinalIgnoreCase)) != null)
	{
		var message = $"Minion '{this.Policy.FullyQualifiedName}' didn't run. There is already an instance of this minion running in the '{this.Environment.Name}' environment and watching the list '{this.Policy.ListToWatch}'.";
		await this.MinionContext.AddMessage(
				this.Environment.GetPolicy<KnownResultCodes>().Warning,
				"MinionAlreadyRunning",
				new object[] { this.Policy.FullyQualifiedName, this.Environment.Name, this.Policy.ListToWatch },
				message)
			.ConfigureAwait(false);
		this.Policy.CurrentRunModel().Abort(message);

		return false;
	}

	// minion that process entities of the same type
	var runningMinion = this.Environment.RunningMinions.FirstOrDefault(p => p.Entities.Intersect(this.Policy.Entities).Any());
	if (runningMinion != null)
	{
		var entities = string.Join(",", runningMinion.Entities.Intersect(this.Policy.Entities));
		var message = $"Minion '{this.Policy.FullyQualifiedName}' (ListToWatch:{this.Policy.ListToWatch}) didn't run. Minion '{runningMinion.FullyQualifiedName}' (ListToWatch:{runningMinion.ListToWatch}) is processing the same type of entities ({entities}) in the '{this.Environment.Name}' environment running.";
		await this.MinionContext.AddMessage(
				this.Environment.GetPolicy<KnownResultCodes>().Warning,
				"MinionProcessingSameEntities",
				new object[] { this.Policy.FullyQualifiedName, this.Environment.Name },
				message)
			.ConfigureAwait(false);
		this.Policy.CurrentRunModel().Abort(message);

		return false;
	}

	return true;
}
```

5. Override the ``Execute()`` method, inserting the following at the beginning of the method.

``var minionRunModel = this.Policy.CurrentRunModel();``

6. Wherever the minion returns the ``MinionRunResultsModel``, replace this with ``minionRunModel``, preceded with one of the following methods:

```
minionRunModel.Complete(message);
```

```
minionRunModel.Complete(message, itemsProcessed);
```

```
minionRunModel.Abort(message);
```

```
minionRunModel.Error(message, hasMoreItems, didRun (default = false));
```

7. For minions running batches, update the ``ItemsProcessed`` property to align with the count for the ``MinionRunResultsModel``. e.g.

```
indexedItemsCount += entitiesToIndex.Items.Count;
minionRunModel.ItemsProcessed += entitiesToIndex.Items.Count;
```

8. Update the minion configuration in the _**Minions**_ policy set to replace the original minion with the custom minion or insert the new minion.
9. Deploy the solution
10. Run Bootstrap
11. Restart the Minions Commerce Engine.

### Update the Commerce Term
1. In **Sitecore Client**, go to the **Content Editor**.
2. Navigate to ``/sitecore/Commerce/Commerce Control Panel/Commerce Engine Settings/Commerce Terms/System Messages/MinionProcessingSameEntities``.
3. Update the **Value** field to ``Minion '{0}' (ListToWatch: {1}) didn't run. Minion '{2}' (ListToWatch: {3}) is processing the same type of entities ({4}) in the '{5}' environment running.``


## Installation Instructions (BizFx Module)

**Prerequisites:** [Installation Instructions (API Only)](#installation-instructions-api-only) and [Installation Instructions (Extended Run Time Properties)](#installation-instructions-extended-run-time-properties).

### Creating a BizFx website for Minions
The Business Tools (Bizfx) website that is installed with via SIF and ARM templates is intended for the Authoring environment for merchandisers. This doesn't prevent its usage as a GUI for Dev Ops operations and Minions management. In order to run BizFx under the Minions environment, a separate BizFx website is required. The following instructions are provided for local IIS setup under the XP0 installation.

1. In IIS, create a new the BizFx website, copying the original BizFx website configuration.
2. Configure the Bindings for the the new website as desired. e.g.

| Website | Bindings |
| ------- | -------- |
| BizFx for Authoring | https://localhost:4200 \| https://bizfx.authoring.local  |
| BizFx for Minions   | https://localhost:4300 \| https://bizfx.minions.local |

3. Update the Hosts file with the binding
4. Ensure the application pool is configured to match the Authoring BizFx application pool.
5. Either copy the BizFx website folder or deploy from the SDK.
6. Update the BizFx configuration in ``\Assets\config.json`` e.g.

| Setting | Example |
| ------- | ------- |
| EnvironmentName | HabitatMinions |
| EngineUri | https://commerceminions.XC92.local |
| BizFxUri | https://bizfx.minions.local |

### Update Identity Server Configuration
1. Update the Identity Server configuration in ``\Config\production\Sitecore.Commerce.IdentityServer.Host.xml`` with the site bindings e.g.
1.a. Append localhost bindings to ``AllowedCorsOriginsGroup1`` e.g.

``<AllowedCorsOriginsGroup1>...|http://localhost:4300|https://localhost:4300</AllowedCorsOriginsGroup1>``

1.b. Append the minions commerce engine and BizFx Uris to ``AllowedCorsOriginsGroup2`` e.g.

``<AllowedCorsOriginsGroup2>...|https://bizfx.minions.local|https://commerceminions.XC92.local</AllowedCorsOriginsGroup2>``

### Configuring the solution
1. In the **Sitecore Commerce Engine** project, open ``\boostrap\Global.json`` and update the EnvironmentBusinessToolsPolicy to include the _Minions_ environment. e.g.

```
{
	"$type": "Sitecore.Commerce.Plugin.BusinessUsers.EnvironmentBusinessToolsPolicy,Sitecore.Commerce.Plugin.BusinessUsers",
	"EnvironmentList": {
		"$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
		"$values": [
			"HabitatAuthoring",
			"HabitatShops",
			"HabitatMinions"
		]
	}
}
```

2. In ``\data\config.json``, update the AllowedOrigins to include the minions BizFx URIs, e.g.

```
"AllowedOrigins": [
	"https://localhost:4200",
	"https://localhost:4300",
	"https://bizfx.authoring.local",
	"https://bizfx.minions.local",
	"https://sxa.storefront.com"
]
```

3. Add the following to the _**AccessByRole**_ policy set

```
{
	"$type": "Sitecore.Commerce.EntityViews.ActionRoleModel, Sitecore.Commerce.Plugin.Views",
	"View": "MinionsDashboard",
	"Role": "sitecore\\Dev Ops Administrator"
}
```

4. Add the following to the _**Minions**_ environment policy set

```
{
	"$type": "Sitecore.Commerce.Core.PolicySetPolicy, Sitecore.Commerce.Core",
	"PolicySetId": "Entity-PolicySet-AccessByRolesPolicySet"
}
```

5. Deploy the solution and run from IIS.
6. Run the **Bootstrap** command on the _**Sitecore Commerce Engine**_. 
7. Restart all commerce engine websites.

### Configuring Roles in Sitecore for Minions
The Minions dashboard is only intended to be visible within BizFx under the Minions environment. Although this configuration is handled via the access roles configured in the Commerce Engine solution, the roles need to be configured in Sitecore. A new Dev Ops Adminstrator role will be created.

In **Sitecore Client**, go to the **Role Manager**.
1. Create Role **Dev Ops Administrator** in the _sitecore_ domain.
2. Set the role to be a member of **sitecore\Commerce Business User**.
3. Create or update a _User_ account with the **Dev Ops Administrator** role.

**Note:** Until the Business Tools is customised to extend the **AccessByRole** with environment filters it is advised to not assign the primary **Administrator** role as a member of the **Dev Ops Administrator** role, simply to avoid the Minions Dashboard from showing in the Authoring BizFx site.

## Known Issues
| Feature                 | Description | Issue |
| ----------------------- | ----------- | ----- |
| Add/Edit/Delete Minion Actions     | Original code migrated from 9.0.2, which hasn't been updated or verified. | N/A |

## Disclaimer
The code provided in this repository is sample code only. It is not intended for production usage and not endorsed by Sitecore.
Both Sitecore and the code author do not take responsibility for any issues caused as a result of using this code.
No guarantee or warranty is provided and code must be used at own risk.
