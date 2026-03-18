# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NET 9.0.

## Table of Contents

- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [CSWinFormDataGridView.csproj](#cswinformdatagridviewcsproj)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)


## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>⚙️&nbsp;CSWinFormDataGridView.csproj</b><br/><small>.NETFramework,Version=v3.5</small>"]
    click P1 "#cswinformdatagridviewcsproj"

```

## Project Details

<a id="cswinformdatagridviewcsproj"></a>
### CSWinFormDataGridView.csproj

#### Project Info

- **Current Target Framework:** .NETFramework,Version=v3.5
- **Proposed Target Framework:** net10.0-windows
- **SDK-style**: False
- **Project Kind:** ClassicWinForms
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 9
- **Lines of Code**: 463

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["CSWinFormDataGridView.csproj"]
        MAIN["<b>⚙️&nbsp;CSWinFormDataGridView.csproj</b><br/><small>.NETFramework,Version=v3.5</small>"]
        click MAIN "#cswinformdatagridviewcsproj"
    end

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |

