using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Xml.Linq;
using DynamicDataHub.Interfaces;
using DynamicDataHub.Modules;

namespace DynamicDataHub
{
    /// <summary>
    /// Логика взаимодействия для UserControlDataTable.xaml
    /// </summary>
    public partial class UserControlDataTable : UserControl 
    {
        #region vars

        private GetDataTable getDataTable;

        private SQLServerConnector sqlServerDB;
        private SQLIteConnector sqliteDB;

        private string tableName;
        private string dbName;

        private string serverName;
        private string nameDbFile;
        private string nameDBManagementSystem;

        private DataTable dataTable;

        private string preparingCellForEditId;

        private Dictionary<string, string> columnValuePairs = new Dictionary<string, string>();

        

        private Window window;
        #endregion

        #region builder
        public UserControlDataTable(DataTable content)
        {
            InitializeComponent();
            this.dataTable = content;
            this.tableName = DatabaseConfiguration.tableName;
            this.dbName = DatabaseConfiguration.dbName;
            this.nameDBManagementSystem = DatabaseConfiguration.nameDbManagementSystem;
            this.serverName = DatabaseConfiguration.serverName;
            this.nameDbFile = DatabaseConfiguration.filePathDb;

            getDataTable = new GetDataTable();

            CustomNotificationBuilder.CreateNotification(MainGrid);

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
        #endregion

        #region get link parent window
        public void GetLinkWindow(Window w)
        {
            this.window = w;
        }
        #endregion


        #region loaded datagrid for user control
        private void UserControl_Loaded(object sender, RoutedEventArgs e){DataTable.DataContext = dataTable;}
        #endregion

        #region handler for interaction with table data
        private async void DataTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedValue = ((TextBox)e.EditingElement).Text;

            var editedColumn = e.Column.Header.ToString();
            var basedColumn = DataTable.Columns[0].Header.ToString();
            int countInsertColumns;

            countInsertColumns = DataTable.Columns.Count;
            foreach (var i in DataTable.Columns)
            {
                if (i.IsReadOnly) countInsertColumns--;
            }
            if (!string.IsNullOrWhiteSpace(preparingCellForEditId))
            {
                switch (this.nameDBManagementSystem)
                {
                    case SQLServerConnector.nameDBManagementSystem:
                        sqlServerDB.UpdateRow(dbName, tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                        await Task.Delay(100);

                        DataTable.DataContext = getDataTable.GetDataTableSQLServer(tableName, dbName);

                        CustomNotificationBuilder.ShowNotificationOpacity("Успешное изменение в таблице!");
                        break;

                    case SQLIteConnector.nameDBManagementSystem:
                        sqliteDB.UpdateRow(tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                        await Task.Delay(100);

                        DataTable.DataContext = getDataTable.GetDataTableSQLite(tableName);

                        CustomNotificationBuilder.ShowNotificationOpacity("Успешное изменение в таблице!");
                        break;
                }
            }
            else
            {
                if (getDataTable.nullableColumns.Count == 0)
                {
                    if (columnValuePairs.ContainsKey(editedColumn))
                    {
                        return;
                    }
                    columnValuePairs.Add(editedColumn, editedValue);

                    if (columnValuePairs.Count == countInsertColumns)
                    {
                        switch (this.nameDBManagementSystem)
                        {
                            case SQLServerConnector.nameDBManagementSystem:
                                sqlServerDB.AddRow(tableName, columnValuePairs, dbName);
                                await Task.Delay(100);

                                DataTable.DataContext = getDataTable.GetDataTableSQLServer(tableName, dbName);

                                break;
                            case SQLIteConnector.nameDBManagementSystem:
                                sqliteDB.AddRow(tableName, columnValuePairs);
                                await Task.Delay(100);

                                DataTable.DataContext = getDataTable.GetDataTableSQLite(tableName);
                                break;
                        }
                    }
                }
                else if (getDataTable.nullableColumns.Count > 0)
                {
                    countInsertColumns -= getDataTable.nullableColumns.Count;
                    columnValuePairs.Add(editedColumn, editedValue);

                    if (columnValuePairs.Count >= countInsertColumns)
                    {
                        switch (this.nameDBManagementSystem)
                        {
                            case SQLServerConnector.nameDBManagementSystem:
                                sqlServerDB.AddRow(tableName, columnValuePairs, dbName);
                                await Task.Delay(100);

                                DataTable.DataContext = getDataTable.GetDataTableSQLServer(tableName, dbName);

                                break;
                            case SQLIteConnector.nameDBManagementSystem:
                                sqliteDB.AddRow(tableName, columnValuePairs);
                                await Task.Delay(100);

                                DataTable.DataContext = getDataTable.GetDataTableSQLite(tableName);
                                break;
                        }
                    }
                }
            }
        }

        private void DataTable_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var _selectedCell = DataTable.SelectedCells[0];
            var _cellContent = _selectedCell.Column.GetCellContent(_selectedCell.Item);

            preparingCellForEditId = (_cellContent as TextBlock)?.Text;

        }
        #endregion

        #region handler for interaction with ContextMenu
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var _selectedCell = DataTable.SelectedCells[0];
            var nameColumnIndefication = DataTable.SelectedCells[0].Column.Header.ToString();
            var _cellContent = _selectedCell.Column.GetCellContent(_selectedCell.Item);
            var indefication = (_cellContent as TextBlock)?.Text;

            switch (this.nameDBManagementSystem) {
                case SQLServerConnector.nameDBManagementSystem:

                    sqlServerDB = new SQLServerConnector(serverName);
                    sqlServerDB.DeleteRow(tableName, nameColumnIndefication, indefication, dbName);

                    DataTable.DataContext = getDataTable.GetDataTableSQLServer(tableName, dbName);

                    CustomNotificationBuilder.ShowNotificationOpacity("Успешное удаление в таблице!");
                    break;

                case SQLIteConnector.nameDBManagementSystem:
                    
                    sqliteDB.DeleteRow(tableName, nameColumnIndefication, indefication);
                    DataTable.DataContext = getDataTable.GetDataTableSQLite(tableName);

                    CustomNotificationBuilder.ShowNotificationOpacity("Успешное удаление в таблице!");
                    break;
                    
            }
        }
        #endregion
    }
}
