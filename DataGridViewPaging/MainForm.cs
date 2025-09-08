/********************************* Module Header **********************************\
* Module Name:  DataGridViewPaging
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to page data in the DataGridView control;
\**********************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
// BEGIN SNIPPET 1: Microsoft.Extensions.AI using statements
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Core;
using System.ClientModel;
using OpenAI;
// END SNIPPET 1
#endregion

namespace CSWinFormDataGridView.DataGridViewPaging
{
    public partial class MainForm : Form
    {        // BEGIN SNIPPET 2: AI client setup with environment variable
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

        private int PageSize = 30; // 30 rows per page
        private int CurrentPageIndex = 1;
        private int TotalPage;

        private string connstr =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

        private SqlConnection conn;
        private SqlDataAdapter adapter;
        private SqlCommand command;

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

        private void GetTotalPageCount()
        {
            command.CommandText = "Select Count(OrderID) From Orders";

            try
            {
                conn.Open();
                int rowCount = (int)command.ExecuteScalar();

                this.TotalPage = rowCount / PageSize;

                if (rowCount % PageSize > 0)
                {
                    this.TotalPage += 1;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        private DataTable GetPageData(int page)
        {
            DataTable dt = new DataTable();

            if (page == 1)
            {
                command.CommandText =
                    "Select Top " + PageSize + " * From Orders Order By OrderID";
            }
            else
            {
                int lowerPageBoundary = (page - 1) * PageSize;

                command.CommandText = "Select Top " + PageSize +
                    " * From Orders " +
                    " WHERE OrderID NOT IN " +
                    " (SELECT TOP " + lowerPageBoundary + " OrderID From Orders Order By OrderID) " +
                    " Order By OrderID";
            }
            try
            {
                this.conn.Open();
                this.adapter.SelectCommand = command;
                this.adapter.Fill(dt);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            this.CurrentPageIndex = 1;
            this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
        }

        private void toolStripButtonPrev_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex > 1)
            {
                this.CurrentPageIndex--;
                this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
            }
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex < this.TotalPage)
            {
                this.CurrentPageIndex++;
                this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
            }
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            this.CurrentPageIndex = TotalPage;
            this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
        }        // BEGIN SNIPPET 4: Semantic search UI and functionality
        private ComboBox txtSemanticSearch;
        private Button btnSearch; private void AddSemanticSearchControls()
        {            // Create search controls
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
            }); btnSearch = new Button
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
            sqlTooltip.SetToolTip(btnSearch, "Click to translate natural language to SQL");            // Create clear search button (initially hidden)
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
                }                // Create a concise prompt with the database schema
                var prompt = $@"
You are an SQL expert that converts natural language questions into valid SQL queries for a Northwind database.

Database schema information:
{DatabaseSchema}

Convert this question into a valid SQL Server query: ""{query}""

Return ONLY the SQL query without any explanation, comments or markdown formatting. The query should be valid for SQL Server 2016.
Limit results to 100 rows maximum.";

                // Get SQL from AI
                var response = await _chatClient.CompleteChatAsync(prompt);
                string generatedSql = response.Value.Content[0].Text;

                if (string.IsNullOrEmpty(generatedSql))
                {
                    throw new Exception("Failed to get a valid response from the AI service.");
                }
                generatedSql = generatedSql.Trim();

                // Update tooltip and show message box with generated SQL
                if (btnSearch.Tag is Tuple<ToolTip, Button> tagPair)
                {
                    tagPair.Item1.SetToolTip(btnSearch, generatedSql);
                }
                MessageBox.Show($"Generated SQL Query:\n\n{generatedSql}",
                    "AI-Generated SQL", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Safety check to ensure query only performs SELECT or WITH (for CTEs) operations
                string sqlLower = generatedSql.Trim().ToLower();
                if (!sqlLower.StartsWith("select") && !sqlLower.StartsWith("with "))
                {
                    throw new InvalidOperationException("Only SELECT queries and Common Table Expressions (WITH) are allowed for safety reasons.");
                }

                // Execute the AI-generated query
                DataTable results = new DataTable();
                using (SqlCommand sqlCommand = new SqlCommand(generatedSql, conn))
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating or executing SQL: {ex.Message}",
                    "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Fall back to getting all orders on error
                return GetAllOrders();
            }
        }        // Hardcoded Northwind database schema as a property for performance and simplicity
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
    }
}