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
    /// Логика взаимодействия для FilterUserControl.xaml
    /// </summary>
    public partial class FilterUserControl : UserControl
    {
        private string tableName;
        private string dbName;

        private string serverName;
        private string nameDbFile;
        private string nameDBManagementSystem;

        private SQLServerConnector sqlServerDB;
        private SQLIteConnector sqliteDB;

        public FilterUserControl()
        {
            InitializeComponent();
            this.tableName = DatabaseConfiguration.tableName;
            this.dbName = DatabaseConfiguration.dbName;
            this.nameDBManagementSystem = DatabaseConfiguration.nameDbManagementSystem;
            this.serverName = DatabaseConfiguration.serverName;
            this.nameDbFile = DatabaseConfiguration.filePathDb;

            switch (this.nameDBManagementSystem)
            {
                case SQLServerConnector.nameDBManagementSystem:
                    sqlServerDB = new SQLServerConnector(this.serverName);
                    break;
                case SQLIteConnector.nameDBManagementSystem:
                    sqliteDB = new SQLIteConnector(this.nameDbFile);
                    break;
                default:
                    return;
            }
        }
    }
}
