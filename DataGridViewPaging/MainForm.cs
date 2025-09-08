using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CSWinFormDataGridView.DataGridViewPaging
{
    public partial class MainForm : Form
    {
        private BindingSource bindingSource = new BindingSource();
        private DataTable dataTable = new DataTable();
        private int pageSize = 20;
        private int currentPage = 0;
        private int totalPages = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create sample data
            CreateSampleData();
            
            // Set up the binding source
            bindingSource.DataSource = dataTable;
            dataGridView1.DataSource = bindingSource;
            
            // Calculate total pages
            CalculateTotalPages();
            
            // Load first page
            LoadPage(0);
        }

        private void CreateSampleData()
        {
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Date", typeof(DateTime));

            // Add sample data (100 rows for demonstration)
            for (int i = 1; i <= 100; i++)
            {
                dataTable.Rows.Add(i, $"Name {i}", $"Description for item {i}", DateTime.Now.AddDays(-i));
            }
        }

        private void CalculateTotalPages()
        {
            if (dataTable.Rows.Count > 0)
            {
                totalPages = (int)Math.Ceiling((double)dataTable.Rows.Count / pageSize);
            }
            else
            {
                totalPages = 0;
            }
        }

        private void LoadPage(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber >= totalPages)
                return;

            currentPage = pageNumber;
            
            var pageData = dataTable.AsEnumerable()
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .CopyToDataTable();
            
            dataGridView1.DataSource = pageData;
            
            // Update button states
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            toolStripButtonFirst.Enabled = currentPage > 0;
            toolStripButtonPrev.Enabled = currentPage > 0;
            toolStripButtonNext.Enabled = currentPage < totalPages - 1;
            toolStripButtonLast.Enabled = currentPage < totalPages - 1;
        }

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            LoadPage(0);
        }

        private void toolStripButtonPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                LoadPage(currentPage - 1);
            }
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages - 1)
            {
                LoadPage(currentPage + 1);
            }
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            LoadPage(totalPages - 1);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}