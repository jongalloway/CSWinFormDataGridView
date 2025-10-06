# CSWinFormDataGridView - Modernizing Windows Forms to .NET 9 with AI

This repository demonstrates modernizing a Windows Forms sample from 2012 to .NET 9 and adding AI features using GitHub Copilot.

## Overview

This project showcases the journey of taking a legacy Windows Forms application and bringing it into the modern .NET ecosystem. The sample application demonstrates various DataGridView capabilities including data paging, custom columns, editing controls, and more.

**Original Source:** [Windows Forms DataGridView Demo (CSWinFormDataGridView)](https://github.com/microsoftarchive/msdn-code-gallery-microsoft/tree/master/OneCodeTeam/Windows%20Forms%20DataGridView%20demo%20(CSWinFormDataGridView))

## Features

- **DataGridView Paging**: Demonstrates pagination of data in the DataGridView control
- **Custom DataGridView Columns**: Examples of custom column implementations
- **Editing Control Hosting**: Shows how to host custom editing controls
- **Just-In-Time Data Loading**: Efficient data loading patterns
- **Multiple Layered Column Headers**: Advanced column header configurations

## Prerequisites

- Visual Studio 2022 or later
- .NET 9 SDK
- SQL Server LocalDB (included with Visual Studio)

## Setup Instructions

### 1. Set up the Northwind Database

The application uses the Northwind sample database. To set it up in SQL Server LocalDB:

1. Open SQL Server Management Studio or use the command line
2. Connect to `(localdb)\MSSQLLocalDB`
3. Create a new database named `Northwind`
4. Run the SQL script located in the root of this repository:
   ```bash
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d Northwind -i instnwnd.sql
   ```

Alternatively, you can open the `instnwnd.sql` file in SQL Server Management Studio and execute it against the Northwind database.

### 2. Configure AI Connection String

To use the AI features, you need to set up an environment variable with your AI service connection string:

**Windows (PowerShell):**
```powershell
$env:AI_CONNECTION_STRING="your-connection-string-here"
```

**Windows (Command Prompt):**
```cmd
set AI_CONNECTION_STRING=your-connection-string-here
```

**Note:** For permanent configuration, set this as a system or user environment variable through System Properties.

## Demo Flow

This repository is designed to showcase a modernization journey in three stages:

### 1. Start with Original Code
The original Windows Forms application from 2012, targeting .NET Framework 3.5, demonstrating classic WinForms development patterns.

### 2. Modernize to .NET 9
Using Visual Studio and GitHub Copilot to:
- Upgrade the project to .NET 9
- Update connection strings for LocalDB usage
- Modernize code patterns and syntax
- Leverage new language features

### 3. Add AI Features
Integrate AI capabilities using GitHub Copilot to:
- Add intelligent data processing
- Implement AI-powered features
- Enhance user experience with AI suggestions

## Running the Application

1. Open `CSWinFormDataGridView.sln` in Visual Studio
2. Ensure the Northwind database is set up (see Setup Instructions)
3. In `Program.cs`, uncomment the desired demo to run:
   ```csharp
   Application.Run(new DataGridViewPaging.MainForm());
   ```
4. Press F5 to run the application

## Project Structure

```
CSWinFormDataGridView/
├── DataGridViewPaging/         # Data paging demonstration
│   ├── MainForm.cs
│   ├── MainForm.Designer.cs
│   └── ReadMe.txt
├── Properties/                 # Assembly and resource files
├── instnwnd.sql               # Northwind database setup script
├── Program.cs                 # Application entry point
└── README.md                  # This file
```

## Database Connection

The application connects to SQL Server LocalDB using the following connection string pattern:
```
Persist Security Info=False;
Integrated Security=SSPI;
Initial Catalog=Northwind;
Data Source=(localdb)\MSSQLLocalDB
```

## Contributing

This is a demonstration repository showing modernization patterns. Feel free to explore, learn, and adapt the patterns shown here for your own projects.

## License

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL

## Resources

- [Original MSDN Code Gallery Sample](https://github.com/microsoftarchive/msdn-code-gallery-microsoft/tree/master/OneCodeTeam/Windows%20Forms%20DataGridView%20demo%20(CSWinFormDataGridView))
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [Windows Forms Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
- [GitHub Copilot](https://github.com/features/copilot)
