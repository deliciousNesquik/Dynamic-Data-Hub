﻿using DynamicDataHub.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        #region vars
        private string query;

        private string tableName;
        private string dbName;

        private string serverName;
        private string nameDbFile;
        private string nameDBManagementSystem;

        private SQLServerConnector sqlServer;
        #endregion

        #region builder
        public QueryExecutionEnvironment()
        {
            InitializeComponent();
            QueryTextBox.Focus();

            this.tableName = DatabaseConfiguration.tableName;
            this.dbName = DatabaseConfiguration.dbName;
            this.nameDBManagementSystem = DatabaseConfiguration.nameDbManagementSystem;
            this.serverName = DatabaseConfiguration.serverName;
            this.nameDbFile = DatabaseConfiguration.serverName;

            CustomNotificationBuilder.CreateNotification(MainGrid);

            if (this.nameDBManagementSystem == SQLServerConnector.nameDBManagementSystem) {

                ChoiseDatabaseComboBox.Visibility = Visibility.Visible;
                sqlServer = new SQLServerConnector(this.serverName);
                List<string> listDb = sqlServer.GetDBNames();

                foreach(string dbName in listDb)
                {
                    ChoiseDatabaseComboBox.Items.Add(dbName);
                }
            }

        }
        #endregion

        #region button click execution query 
        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            sqlServer = new SQLServerConnector(this.serverName);
            query = QueryTextBox.Text;
            DataTable dataTable = sqlServer.CreateQuery(query, dbName);
            UserControlDataTable content = new UserControlDataTable(dataTable);
            FrameResultExecutionQuery.Navigate(content);
        }
        #endregion

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                StreamReader reader = new StreamReader(fileInfo.Open(FileMode.Open, FileAccess.Read), Encoding.GetEncoding(1251));

                QueryTextBox.Text = reader.ReadToEnd();

                reader.Close();
                return;
            }
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "query (*.DDHQuery)|*.DDHQuery";

            if (saveFileDialog1.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.OpenFile(), System.Text.Encoding.Default))
                {
                    sw.Write(QueryTextBox.Text);
                    sw.Close();
                }
            }
        }



        private void ChoiseDatabaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.dbName = ChoiseDatabaseComboBox.SelectedItem.ToString();
        }
    }
}
