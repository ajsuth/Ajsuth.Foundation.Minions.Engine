# Extended Sitecore Commerce Minions
A custom plugin to extend minions functionality in Sitecore Experience Commerce.

** WARNING **
This solution has been adapted from a P.o.C. from XC 9.0.2. and is still a work in progress.

## Features
Minions Dashboard
Running Minions Entity View
Extended Minion Properties

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

## Known Issues
| Feature                 | Description | Issue |
| ----------------------- | ----------- | ----- |
| Add/Edit/Delete Minion Actions     | Original code migrated from 9.0.2, which hasn't been updated or verified. | N/A |

## Disclaimer
The code provided in this repository is sample code only. It is not intended for production usage and not endorsed by Sitecore.
Both Sitecore and the code author do not take responsibility for any issues caused as a result of using this code.
No guarantee or warranty is provided and code must be used at own risk.
