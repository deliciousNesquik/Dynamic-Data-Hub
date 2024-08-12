using DynamicDataHub.Modules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicDataHub.Views
{
    /// <summary>
    /// Логика взаимодействия для QueryExecutionEnvironment.xaml
    /// </summary>
    public partial class QueryExecutionEnvironment : UserControl
    {
        private string query;

        private string tableName;
        private string dbName;

        private string serverName;
        private string nameDbFile;
        private string nameDBManagementSystem;


        private SQLServerConnector sqlServer;
        public QueryExecutionEnvironment()
        {
            InitializeComponent();
            queryTextBox.Focus();

            this.tableName = DatabaseConfiguration.tableName;
            this.dbName = DatabaseConfiguration.dbName;
            this.nameDBManagementSystem = DatabaseConfiguration.nameDbManagementSystem;
            this.serverName = DatabaseConfiguration.serverName;
            this.nameDbFile = DatabaseConfiguration.serverName;
        }

        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            sqlServer = new SQLServerConnector(this.serverName);
            query = queryTextBox.Text;
            DataTable dataTable = sqlServer.CreateQuery(query, dbName);
            foreach(DataRow row in dataTable.Rows)
            {
                Console.WriteLine(row[0].ToString());
            }
        }
    }
}
