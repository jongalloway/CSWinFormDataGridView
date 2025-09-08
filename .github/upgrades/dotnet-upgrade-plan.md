# .NET 9.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 9.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9.0 upgrade.
3. Upgrade CSWinFormDataGridView.csproj

## Settings

This section contains settings and data used by execution steps.

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### CSWinFormDataGridView.csproj modifications

Project properties changes:
  - Target framework should be changed from `.NETFramework,Version=v3.5` to `net9.0-windows`
  - Project file needs to be converted to SDK-style format

Other changes:
  - Convert legacy project file format to modern SDK-style project format
  - Update project references and dependencies to be compatible with .NET 9.0