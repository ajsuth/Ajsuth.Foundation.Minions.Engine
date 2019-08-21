# Extended Sitecore Commerce Minions
A custom plugin to extend minions functionality in Sitecore Experience Commerce.

** WARNING **
This solution has been adapted from a P.o.C. from XC 9.0.2. and is still a work in progress.

## Features

### Minions Dashboard
Provides an overview of minions. Last run time details will render for custom minions. See [Updating Minions to obtain run time data](updating-minions-to-obtain-run-time-data).

### Running Minions Entity View
Renders minions currently runnning.

### Extended Minion Run Time Properties
The ``MinionRunModel`` captures additional data for the last minion execution for display in the Running Minions and Minons Dashboard entity views.
**Note:** The ``MinionRunModel`` is intended to replace the ``MinionRunResultsModel``. _Early P.o.C. days._
**Note:** Currently only the last execution data is captured to avoid a memory leak.

## Installation Instructions

### Creating a BizFx website for Minions
The Business Tools (Bizfx) website that is installed with via SIF and ARM templates is intended for the Authoring environment for merchandisers. This doesn't prevent its usage as a GUI for Dev Ops operations and Minions management. In order to run BizFx under the Minions environment, a separate BizFx website is required. The following instructions are provided for local IIS setup under the XP0 installation.

1. In IIS, create a new the BizFx website, copying the original BizFx website configuration.
2. Configure the Bindings for the the new website as desired. e.g.

| Website | Bindings |
| ------- | -------- |
| BizFx for Authoring | https://localhost:4200 \| https://bizfx.XC91.local  |
| BizFx for Minions   | https://localhost:4300 \| https://bizfx.XC91m.local |

3. Update the Hosts file with the binding
4. Ensure the application pool is configured to match the Authoring BizFx application pool.
5. Either copy the BizFx website folder or deploy from the SDK.
6. Update the BizFx configuration in \Assets\config.json e.g.

| Setting | Example |
| ------- | ------- |
| EnvironmentName | HabitatMinions |
| EngineUri | https://commerceminions.XC91.local |
| BizFxUri | https://bizfx.XC91m.local |

### Update Identity Server Configuration
1. Update the Identity Server configuration in \Config\production\Sitecore.Commerce.IdentityServer.Host.xml with the site bindings e.g.
1.a. Append localhost bindings to AllowedCorsOriginsGroup1 e.g.

``<AllowedCorsOriginsGroup1>...|http://localhost:4300|https://localhost:4300</AllowedCorsOriginsGroup1>``

1.b. Append the minions commerce engine and BizFx Uris to engineAllowedCorsOriginsGroup2 e.g.

``<AllowedCorsOriginsGroup2>...|https://bizfx.XC91m.local|https://commerceminions.XC91.local</AllowedCorsOriginsGroup2>``

### Adding the plugin to your solution
1. Download the repository.
2. Add the **Ajsuth.Foundation.Minions.Engine.csproj** to the _**Sitecore Commerce Engine**_ solution.
3. In the _**Sitecore Commerce Engine**_ project, add a reference to the **Ajsuth.Foundation.Minions.Engine** project.
4. Add the following to the _**AccessByRole**_ policy set

``{
	"$type": "Sitecore.Commerce.EntityViews.ActionRoleModel, Sitecore.Commerce.Plugin.Views",
	"View": "MinionsDashboard",
	"Role": "sitecore\\Dev Ops Manager"
}``

5. Add the following to the _**Minions**_ environment policy set

``{
	"$type": "Sitecore.Commerce.EntityViews.ActionRoleModel, Sitecore.Commerce.Plugin.Views",
	"View": "MinionsDashboard",
	"Role": "sitecore\\Dev Ops Manager"
}``

6. Deploy the solution and run from IIS.
7. Run the **Bootstrap** command on the _**Sitecore Commerce Engine**_. 
8. Restart all commerce engine websites.

### Configuring Roles in Sitecore for Minions
The Minions dashboard is only intended to be visible within BizFx under the Minions environment. Although this configuration is handled via the access roles configured in the Commerce Engine solution, the roles need to be configured in Sitecore. A new Dev Ops Adminstrator role will be created.

In Sitecore, go to the Role Manager
1. Create Role **Dev Ops Administrator** in the _sitecore_ domain.
2. Set the role to be a member of **sitecore\Commerce Business User**.
3. Create or update a _User_ account with the **Dev Ops Administrator** role.

**Note:** Until I customise the Business Tools to extend the **AccessByRole** with environment filters it is advised to not assign the primary **Administrator** role as a member of the **Dev Ops Administrator** role simply to avoid the Minions Dashboard from showing in the Authoring BizFx site.

### Updating Minions to obtain run time data
To track the last run time data of a minion, the minion will need to be overridden in your solution. The following instructions provides a guide for capturing this data.

1. Create a new minion in your solution, inheriting from the desired minion if it's not custom.
2. Add the following using statement to the minion class

``using Ajsuth.Foundation.Minions.Engine.Models;``

3. Override the ``Process()`` method, adding the following code to the beginning of the method.

```
public override async Task<MinionRunResultsModel> Process()
{
	var minionRunModel = new MinionRunModel();
	this.Policy.Models.RemoveAll(m => m is MinionRunModel);
	this.Policy.Models.Add(minionRunModel);
	if (!(await this.ShouldProcess().ConfigureAwait(false)))
	{
		return new MinionRunResultsModel();
	}

	this.Environment.AddRunningMinion(this);
	var result = await this.Execute().ConfigureAwait(false);
	this.Environment.RemoveRunningMinion(this);

	return result;
}
```

4. Override the ``ShouldProcess()`` method with the following

```
protected override async Task<bool> ShouldProcess()
{
	// minion with the same configuration
	if (this.Environment.RunningMinions.FirstOrDefault(p => p == this.Policy) != null)
	{
		var message = $"Minion '{this.Policy.FullyQualifiedName}' didn't run. There is already an instance of this minion running in the '{this.Environment.Name}' environment and watching the list '{this.Policy.ListToWatch}'.";
		await this.GlobalContext.AddMessage(
				this.Environment.GetPolicy<KnownResultCodes>().Warning,
				"MinionAlreadyRunning",
				new object[] { this.Policy.FullyQualifiedName, this.Environment.Name, this.Policy.ListToWatch },
				message)
			.ConfigureAwait(false);
		var minionRunModel = this.Policy.Models.FirstOrDefault(m => m is MinionRunModel) as MinionRunModel;
		minionRunModel.RunComplete("Complete", message);

		return false;
	}

	// minion that process entities of the same type
	var runningMinion = this.Environment.RunningMinions.FirstOrDefault(p => p.Entities.Intersect(this.Policy.Entities).Any());
	if (runningMinion != null)
	{
		var entities = string.Join(",", runningMinion.Entities.Intersect(this.Policy.Entities));
		var message = $"Minion '{this.Policy.FullyQualifiedName}' (ListToWatch:{this.Policy.ListToWatch}) didn't run. Minion '{runningMinion.FullyQualifiedName}' (ListToWatch:{runningMinion.ListToWatch}) is processing the same type of entities ({entities}) in the '{this.Environment.Name}' environment running.";
		await this.GlobalContext.AddMessage(
				this.Environment.GetPolicy<KnownResultCodes>().Warning,
				"MinionProcessingSameEntities",
				new object[] { this.Policy.FullyQualifiedName, this.Environment.Name },
				message)
			.ConfigureAwait(false);
		var minionRunModel = this.Policy.Models.FirstOrDefault(m => m is MinionRunModel) as MinionRunModel;
		minionRunModel.RunComplete("Complete", message);

		return false;
	}

	return true;
}
```

5. Override the ``Execute()`` method, inserting the following at the beginning of the method.

``var minionRunModel = this.Policy.Models.FirstOrDefault(m => m is MinionRunModel) as MinionRunModel;``

6. Add the ``minionRunModel.RunComplete()`` method wherever the minion returns the ``MinionRunResultsModel``, providing the appropriate status and message. e.g.

``minionRunModel.RunComplete("Complete", "Minion completed successfully");``

7. For minions running batches, update the ``ItemsProcessed`` property to align with the count for the ``MinionRunResultsModel``. e.g.

```
indexedItemsCount += entitiesToIndex.Count;
minionRunModel.ItemsProcessed += entitiesToIndex.Count;
```

8. Update the minion configuration in the _**Minions**_ policy set.
9. Deploy the solution
10. Run Bootstrap
11. Restart the Minions Commerce Engine.

## Known Issues
| Feature                 | Description | Issue |
| ----------------------- | ----------- | ----- |
| Add/Edit/Delete Minion Actions     | Original code migrated from 9.0.2, which hasn't been updated or verified. | N/A |

## Disclaimer
The code provided in this repository is sample code only. It is not intended for production usage and not endorsed by Sitecore.
Both Sitecore and the code author do not take responsibility for any issues caused as a result of using this code.
No guarantee or warranty is provided and code must be used at own risk.
