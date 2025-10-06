# Modernization Demo Script

This script guides you through the three-stage modernization journey of the CSWinFormDataGridView application, from the original 2012 code to a modern .NET 9 application with AI features.

**Important:** This demo is performed entirely on the `main` branch. The `dotnet9` and `ai-queries` branches exist as reference points showing the completed state of each stage, and can be used as backup if you encounter issues or need to skip ahead due to time constraints.

## Prerequisites

Before starting, ensure you have:
- Visual Studio 2022 or later
- .NET 9 SDK installed
- SQL Server LocalDB
- Northwind database set up (see main README.md)
- AI_API_KEY environment variable configured (for Stage 3)
- Working on the `main` branch of the repository

## Stage 1: Examine the Original Code

### Starting Point
You should be on the `main` branch with the original 2012 code:
```bash
git checkout main
```

### Examine the Original Code
Open `DataGridViewPaging/MainForm.cs` and observe:
- .NET Framework 3.5 target
- Uses `System.Data.SqlClient`
- Traditional C# patterns from 2012

### Run the Application
1. Open `CSWinFormDataGridView.sln` in Visual Studio
2. In `Program.cs`, uncomment:
   ```csharp
   Application.Run(new DataGridViewPaging.MainForm());
   ```
3. Press F5 to run
4. Observe the basic pagination functionality

---

## Stage 2: Modernize to .NET 9

Now we'll modernize the application to .NET 9. All changes are made to the files in the `main` branch.

> **Reference:** If you need to see the completed state, the `dotnet9` branch shows all these changes already applied.

### Key Changes to Make

#### 1. Update the Project File

Update the `.csproj` file to target .NET 9 instead of .NET Framework 3.5. You can use Visual Studio's project upgrade wizard or manually edit the project file.

#### 2. Update Database Library in MainForm.cs

Replace `System.Data.SqlClient` with `Microsoft.Data.SqlClient`:

```csharp
// Change this line:
using System.Data.SqlClient;

// To this:
using Microsoft.Data.SqlClient;
```

#### 3. (Optional) Improve Connection String

You can optionally update the connection string to a more modern format:

```csharp
// Original connection string:
private string connstr =
    "Persist Security Info=False;" +
    "Integrated Security=SSPI;" +
    "Initial Catalog=Northwind;" +
    "Data Source=(localdb)\\MSSQLLocalDB";

// Modern format (optional):
private string connstr =
    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
```

#### 4. Add Modern Resource Disposal

Add a proper Dispose pattern at the end of the `MainForm` class:

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Clean up database resources
        command?.Dispose();
        adapter?.Dispose();
        conn?.Dispose();
        
        if (components != null)
        {
            components.Dispose();
        }
    }
    base.Dispose(disposing);
}
```

### Build and Test
1. Build the solution in Visual Studio
2. Run the application
3. Verify all pagination functionality still works

---

## Stage 3: Add AI Features

Now we'll add AI-powered semantic search to the application. All changes continue to be made on your working branch.

> **Reference:** If you need to see the completed state or skip ahead, the `ai-queries` branch shows all these changes already applied.

### Step 1: Add Required NuGet Packages

Add these packages to your project:
```bash
dotnet add package Microsoft.Extensions.AI
dotnet add package Microsoft.Extensions.AI.OpenAI
dotnet add package Azure.AI.OpenAI
```

### Step 2: Add Using Statements (SNIPPET 1)

At the top of `MainForm.cs`, add the AI-related using statements:

```csharp
// BEGIN SNIPPET 1: Microsoft.Extensions.AI using statements
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Core;
using System.ClientModel;
using OpenAI;
// END SNIPPET 1
```

### Step 3: Add AI Client Setup (SNIPPET 2)

Add the AI client initialization in the constructor:

```csharp
// BEGIN SNIPPET 2: AI client setup with environment variable
private OpenAI.Chat.ChatClient _chatClient;

public MainForm()
{
    InitializeComponent();

    // Simple API key setup - read from environment variable
    string apiKey = Environment.GetEnvironmentVariable("AI_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
    {
        MessageBox.Show("Please set the AI_API_KEY environment variable to use AI features.",
            "API Key Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }

    // Set up AI client in a single step
    var client = new OpenAIClient(
        new ApiKeyCredential(apiKey),
        new OpenAIClientOptions { Endpoint = new Uri("https://models.inference.ai.azure.com") }
    );
    _chatClient = client.GetChatClient("gpt-4o");
}
// END SNIPPET 2
```

**Note:** The original constructor code `InitializeComponent();` should be kept inside the new constructor.

### Step 4: Initialize Semantic Search UI (SNIPPET 3)

In the `MainForm_Load` method, add a call to set up the search controls:

```csharp
private void MainForm_Load(object sender, EventArgs e)
{
    this.conn = new SqlConnection(connstr);
    this.adapter = new SqlDataAdapter();
    this.command = conn.CreateCommand();

    // Get total count of the pages;
    this.GetTotalPageCount();

    this.dataGridView1.ReadOnly = true;
    // Load the first page of data;
    this.dataGridView1.DataSource = GetPageData(1);

    // BEGIN SNIPPET 3: Setup semantic search UI
    AddSemanticSearchControls();
    // END SNIPPET 3
}
```

### Step 5: Add Semantic Search Functionality (SNIPPET 4)

Add all the semantic search methods at the end of the class (before the closing brace):

```csharp
// BEGIN SNIPPET 4: Semantic search UI and functionality
private ComboBox txtSemanticSearch;
private Button btnSearch;

private void AddSemanticSearchControls()
{
    // Create search controls
    txtSemanticSearch = new ComboBox();
    // Position the search box to take most of the form width
    txtSemanticSearch.Location = new Point(12, 5);
    txtSemanticSearch.Width = this.ClientSize.Width - 120; // Leave room for button
    txtSemanticSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    txtSemanticSearch.DropDownStyle = ComboBoxStyle.DropDown;
    txtSemanticSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
    txtSemanticSearch.AutoCompleteSource = AutoCompleteSource.ListItems;

    // Add example queries to the dropdown
    txtSemanticSearch.Items.AddRange(new string[] {
        "Show me the top 5 employees by total sales revenue in 1998",
        "List all customers who haven't placed an order in the last 3 months of data",
        "Show monthly sales trends comparing 1997 vs 1998 in a summary format",
        "Find products with inventory below reorder level but no pending orders",
        "Calculate average shipping delay by country, sorted from longest to shortest",
        "Show me the most profitable product categories based on order details",
        "List customers who've ordered all categories of products we sell",
        "Find employees with declining monthly average sales from 1997 to 1998",
        "Show correlation summary between discount percentage and order quantities",
        "Create a supplier performance report showing average delivery time and out-of-stock incidents"
    });

    btnSearch = new Button
    {
        Text = "AI Search",
        Location = new Point(txtSemanticSearch.Right + 5, 5),
        Width = 100,
        Anchor = AnchorStyles.Top | AnchorStyles.Right
    };
    // Add tooltip for the button to show generated SQL
    ToolTip sqlTooltip = new ToolTip();
    sqlTooltip.AutoPopDelay = 10000; // Show tooltip for 10 seconds
    sqlTooltip.InitialDelay = 500;
    sqlTooltip.ReshowDelay = 200;
    sqlTooltip.ToolTipTitle = "Generated SQL Query";
    sqlTooltip.SetToolTip(btnSearch, "Click to translate natural language to SQL");

    // Create clear search button (initially hidden)
    Button btnClearSearch = new Button
    {
        Text = "Clear Search",
        Location = new Point(btnSearch.Right + 5, 5),
        Width = 100,
        Visible = false,
        Anchor = AnchorStyles.Top | AnchorStyles.Right
    };
    btnClearSearch.Click += (s, e) =>
    {
        // Restore original view
        toolStrip1.Visible = true;
        this.CurrentPageIndex = 1;
        this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
        btnClearSearch.Visible = false;
    };

    // Store both tooltip and clear button in a single tuple for easier access
    btnSearch.Tag = Tuple.Create(sqlTooltip, btnClearSearch);
    btnSearch.Click += BtnSearch_Click;

    // Add controls to form
    this.Controls.Add(txtSemanticSearch);
    this.Controls.Add(btnSearch);
    this.Controls.Add(btnClearSearch);

    // Adjust group box position to accommodate the search controls
    groupBox1.Location = new Point(groupBox1.Location.X, btnSearch.Bottom + 10);
    groupBox1.Height = this.ClientSize.Height - groupBox1.Location.Y - 10;
}

private async void BtnSearch_Click(object sender, EventArgs e)
{
    string query = txtSemanticSearch.Text;
    if (string.IsNullOrWhiteSpace(query) || _chatClient == null)
        return;

    // Update UI state
    Cursor = Cursors.WaitCursor;
    btnSearch.Enabled = false;
    btnSearch.Text = "Translating...";
    toolStrip1.Visible = false; // Hide pagination for search results

    try
    {
        var results = await PerformSemanticSearchAsync(query);

        if (results != null && results.Rows.Count > 0)
        {
            dataGridView1.DataSource = results;

            // Show the clear search button
            var tagItems = btnSearch.Tag as Tuple<ToolTip, Button>;
            if (tagItems != null)
                tagItems.Item2.Visible = true;
        }
        else
        {
            MessageBox.Show("No matching orders found.", "Search Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error performing semantic search: {ex.Message}",
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnSearch.Enabled = true;
        btnSearch.Text = "AI Search";
        Cursor = Cursors.Default;
    }
}

private async Task<DataTable> PerformSemanticSearchAsync(string query)
{
    try
    {
        if (_chatClient == null)
        {
            MessageBox.Show("AI is not configured properly. Please set the AI_API_KEY environment variable.",
                "AI Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return GetAllOrders();
        }

        // Generate SQL from natural language query
        string generatedSql = await GenerateSqlFromQueryAsync(query);
        
        // Display the generated SQL to the user
        DisplayGeneratedSql(generatedSql);

        // Validate and execute the query
        return await ExecuteSqlQueryAsync(generatedSql);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error generating or executing SQL: {ex.Message}",
            "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return GetAllOrders();
    }
}

private async Task<string> GenerateSqlFromQueryAsync(string query)
{
    var prompt = $@"
You are an SQL expert that converts natural language questions into valid SQL queries for a Northwind database.

Database schema information:
{DatabaseSchema}

Convert this question into a valid SQL Server query: ""{query}""

Return ONLY the SQL query without any explanation, comments or markdown formatting. The query should be valid for SQL Server 2016.
Limit results to 100 rows maximum.";

    var response = await _chatClient.CompleteChatAsync(prompt);
    string generatedSql = response.Value.Content[0].Text;

    if (string.IsNullOrEmpty(generatedSql))
    {
        throw new Exception("Failed to get a valid response from the AI service.");
    }

    return StripMarkdownCodeBlock(generatedSql);
}

private void DisplayGeneratedSql(string sql)
{
    // Update tooltip
    if (btnSearch.Tag is Tuple<ToolTip, Button> tagPair)
    {
        tagPair.Item1.SetToolTip(btnSearch, sql);
    }

    // Show message box with generated SQL
    MessageBox.Show($"Generated SQL Query:\n\n{sql}",
        "AI-Generated SQL", MessageBoxButtons.OK, MessageBoxIcon.Information);
}

private async Task<DataTable> ExecuteSqlQueryAsync(string sql)
{
    // Safety check to ensure query only performs SELECT or WITH (for CTEs) operations
    string sqlLower = sql.Trim().ToLower();
    if (!sqlLower.StartsWith("select") && !sqlLower.StartsWith("with "))
    {
        throw new InvalidOperationException("Only SELECT queries and Common Table Expressions (WITH) are allowed for safety reasons.");
    }

    // Execute the AI-generated query
    DataTable results = new DataTable();
    using (SqlCommand sqlCommand = new SqlCommand(sql, conn))
    {
        try
        {
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(results);
        }
        finally
        {
            conn.Close();
        }
    }

    return results;
}

/// <summary>
/// Strips markdown code block formatting from SQL queries.
/// Removes ```sql or ``` from the beginning and ``` from the end.
/// </summary>
private string StripMarkdownCodeBlock(string sql)
{
    if (string.IsNullOrWhiteSpace(sql))
        return sql;

    // Remove markdown code blocks: ```sql\n or ```\n at start and ``` at end
    return System.Text.RegularExpressions.Regex.Replace(sql.Trim(), @"^```(?:sql)?\s*\n?|\n?```$", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline).Trim();
}

// Hardcoded Northwind database schema as a property for performance and simplicity
private string DatabaseSchema => @"Tables in Northwind database:
- Table: Orders
  Columns: OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry
- Table: Customers
  Columns: CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax
- Table: Employees
  Columns: EmployeeID, LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo
- Table: Shippers
  Columns: ShipperID, CompanyName, Phone
- Table: [Order Details]
  Columns: OrderID, ProductID, UnitPrice, Quantity, Discount
- Table: Products
  Columns: ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued

Key relationships:
- Orders.CustomerID references Customers.CustomerID
- Orders.EmployeeID references Employees.EmployeeID
- Orders.ShipVia references Shippers.ShipperID
- [Order Details].OrderID references Orders.OrderID
- [Order Details].ProductID references Products.ProductID";

private DataTable GetAllOrders()
{
    DataTable dt = new DataTable();

    SqlCommand getAllCmd = conn.CreateCommand();
    getAllCmd.CommandText = "SELECT TOP 100 * FROM Orders";

    try
    {
        conn.Open();
        SqlDataAdapter tempAdapter = new SqlDataAdapter(getAllCmd);
        tempAdapter.Fill(dt);
    }
    finally
    {
        conn.Close();
    }
    return dt;
}
// END SNIPPET 4
```

### Build and Test

1. Ensure your `AI_API_KEY` environment variable is set (restart Visual Studio after setting it)
2. Build the solution
3. Run the application
4. Try the semantic search feature with natural language queries like:
   - "Show me the top 5 employees by total sales revenue in 1998"
   - "List all customers who haven't placed an order in the last 3 months of data"

### How It Works

The AI-powered semantic search:
1. Takes natural language input from the user
2. Sends it to an AI model (GPT-4) with the Northwind database schema
3. The AI generates a SQL query based on the natural language
4. Displays the generated SQL to the user for transparency
5. Executes the SQL query (with safety checks)
6. Shows the results in the DataGridView

---

## Summary

You've now completed the modernization journey by making all changes on the `main` branch:

1. **Stage 1**: Started with original 2012 Windows Forms code
2. **Stage 2**: Modernized to .NET 9 with updated libraries and patterns
3. **Stage 3**: Added AI-powered natural language query capabilities

The `dotnet9` and `ai-queries` branches exist as reference points showing the completed state at each stage, and can be used as backup if needed.

This demonstrates how legacy applications can be incrementally modernized while adding cutting-edge AI features.

## Tips for Presentation

- Work on the `main` branch for all demo steps
- The `dotnet9` and `ai-queries` branches are backup/reference only - mention them but don't check them out during the demo
- Run the application at each stage to show the progression
- Use the pre-populated example queries in the dropdown to demonstrate AI features
- Show how the AI translates natural language to SQL in real-time
- Highlight the safety features (only SELECT queries allowed)
- Emphasize how GitHub Copilot can assist in both the modernization and AI feature addition
- If short on time or encountering issues, you can reference or switch to the completed branches

## Troubleshooting

### Need to Skip Ahead or Recover?
If you encounter issues or are short on time:
- The `dotnet9` branch shows the completed Stage 2 modernization
- The `ai-queries` branch shows the completed Stage 3 with AI features
- You can check out these branches to continue from a working state

### AI Features Not Working
- Verify `AI_API_KEY` environment variable is set
- Restart Visual Studio after setting environment variables
- Check that you have internet connectivity
- Verify your API key is valid for GitHub Models or Azure AI Foundry

### Database Connection Issues
- Ensure SQL Server LocalDB is installed
- Verify the Northwind database is created and populated
- Check the connection string matches your LocalDB instance name

### Build Errors
- Ensure .NET 9 SDK is installed
- Verify all NuGet packages are restored
- Check that target framework is correctly set to .NET 9
