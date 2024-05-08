using DbAccessUtil;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string connectionString = "data source=LAPTOP-F00D91HG;initial catalog=master;trusted_connection=true";
            var data = new DataAccessor(connectionString);
            if (data.TestSqlServerConnection())
            {
                MessageBox.Show("Connected to SQL Server database successfully!");
            }
            else
            {
                MessageBox.Show($"Connecting to SQL Server database failed!\n{data.ErrorMsg}");
            } 
        }
    }
}
