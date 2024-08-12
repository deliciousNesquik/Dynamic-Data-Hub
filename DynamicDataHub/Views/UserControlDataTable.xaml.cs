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
        private CustomMessageBoxBuilder customMessageBoxBuilder = new CustomMessageBoxBuilder();

        private int indexOfColumn;
        private int indexOfDataType;
        private int indexOfIsNullable;

        private SQLServerConnector sqlServerDB;
        private SQLIteConnector sqliteDB;

        private string tableName;
        private string dbName;

        private string serverName;
        private string nameDbFile;
        private string nameDBManagementSystem;


        private string preparingCellForEditId;

        private Dictionary<string, string> columnValuePairs = new Dictionary<string, string>();

        private List<string> nullableColumns = new List<string>();

        private Window window;


        public UserControlDataTable()
        {
            InitializeComponent();

            this.tableName = DatabaseConfiguration.tableName;
            this.dbName = DatabaseConfiguration.dbName;
            this.nameDBManagementSystem = DatabaseConfiguration.nameDbManagementSystem;
            this.serverName = DatabaseConfiguration.serverName;
            this.nameDbFile = DatabaseConfiguration.filePathDb;

            CustomNotificationBuilder.CreateNotification(MainGrid);

            switch (this.nameDBManagementSystem)
            {
                case "MS SQL Server":
                    sqlServerDB = new SQLServerConnector(this.serverName);
                    break;
                case "SQLite":
                    sqliteDB = new SQLIteConnector(this.nameDbFile);
                    break;
                default:
                    return;
            }
        }

        public void GetLinkWindow(Window w)
        {
            this.window = w;
        }

        #region functions for displaying data
        public DataTable GetDataTableSQLServer(string tableName, string dbName)
        {
            nullableColumns.Clear();
            //DataTable.Visibility = Visibility.Visible;
            sqlServerDB = new SQLServerConnector(serverName);

            DataTable table = new DataTable(tableName);
            DataTable databases = sqlServerDB.GetColumnTable(tableName, dbName);


            indexOfColumn = 0;
            indexOfDataType = 1;
            indexOfIsNullable = 2;

            int rows_columns = databases.Rows.Count;

            foreach (DataRow _row in databases.Rows)
            {
                if (_row[indexOfIsNullable].ToString() == "YES")
                {
                    nullableColumns.Add(_row[indexOfColumn].ToString());
                }
                switch (_row[indexOfDataType].ToString())
                {
                    case "int":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(Int32));
                        break;
                    case "nvarchar":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "date":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(DateTime));
                        break;
                    case "bit":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(Boolean));
                        break;
                    case "nchar":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "char":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "varchar":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "float":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(float));
                        break;
                    case "decimal":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(decimal));
                        break;
                    case "datetime":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(DateTime));
                        break;
                    case "image":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    case "uniqueidentifier":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    case "sysname":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    case "varbinary":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(string));
                        break;
                    default:
                        break;
                }
            }

            databases = sqlServerDB.IsIdentityColumn(this.tableName, this.dbName);

            foreach (DataColumn column in table.Columns)
            {
                string columnName = column.ColumnName.ToString();

                foreach (DataRow row in databases.Rows)
                {
                    if (row["name"].ToString() == columnName)
                    {
                        column.ReadOnly = true;
                    }
                }

            }


            databases = sqlServerDB.CreateQuery($"SELECT * FROM [{tableName}]", dbName);
            int count_columns = databases.Columns.Count;

            List<object> row_values = new List<object>();
            foreach (DataRow _row in databases.Rows)
            {
                for (int i = 0; i < count_columns; i++)
                {
                    row_values.Add(_row[i]);
                }

                table.Rows.Add(row_values.ToArray());

                row_values.Clear();
            }

            return table;
        }

        public DataTable GetDataTableSQLite(string tableName)
        {
            nullableColumns.Clear();

            DataTable.Visibility = Visibility.Visible;
            sqliteDB = new SQLIteConnector(nameDbFile);

            DataTable table = new DataTable(tableName);
            DataTable dataBases = sqliteDB.GetColumnTable(tableName);

            indexOfColumn = 1;
            indexOfDataType = 2;
            indexOfIsNullable = 3;

            int rows_columns = dataBases.Rows.Count;

            foreach (DataRow _row in dataBases.Rows)
            {
                if (Int32.Parse(_row[indexOfIsNullable].ToString()) == 0)
                {
                    nullableColumns.Add(_row[indexOfColumn].ToString());
                }

                switch (_row[indexOfDataType].ToString())
                {
                    case "INTEGER":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(Int32));
                        break;
                    case "TEXT":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(String));
                        break;
                    case "BLOB":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(byte));
                        break;
                    case "REAL":
                        table.Columns.Add(_row[indexOfColumn].ToString(), typeof(float));
                        break;
                }
            }


            dataBases = sqliteDB.CreateQuery($"SELECT * FROM [{tableName}]");

            int countСolumns = dataBases.Columns.Count;

            List<object> rowValues = new List<object>();
            foreach (DataRow _row in dataBases.Rows)
            {
                for (int i = 0; i < countСolumns; i++)
                {
                    rowValues.Add(_row[i]);
                }
                try
                {
                    table.Rows.Add(rowValues.ToArray());
                }
                catch (ArgumentException)
                {
                    //customMessageBoxBuilder = new CustomMessageBoxBuilder();
                    //customMessageBoxBuilder.ShowError("Ошибка", "У столбца в таблице отсутствует тип данных", "Ok", window);
                }

                rowValues.Clear();
            }

            return table;
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (this.nameDBManagementSystem)
            {
                case "MS SQL Server":
                    DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
                    break;
                case "SQLite":
                    DataTable.DataContext = GetDataTableSQLite(tableName);
                    break;
                default:
                    return;
            }
        }

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
                if (nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
                {
                    sqlServerDB.UpdateRow(dbName, tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                    await Task.Delay(100);

                    DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);


                    CustomNotificationBuilder.ShowNotificationOpacity("Успешное изменение в таблице!");
                }
                else if (nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
                {
                    sqliteDB.UpdateRow(tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                    await Task.Delay(100);

                    DataTable.DataContext = GetDataTableSQLite(tableName);

                    CustomNotificationBuilder.ShowNotificationOpacity("Успешное изменение в таблице!");
                }
            }
            else
            {
                if (nullableColumns.Count == 0)
                {
                    if (columnValuePairs.ContainsKey(editedColumn))
                    {
                        return;
                    }
                    columnValuePairs.Add(editedColumn, editedValue);

                    if (columnValuePairs.Count == countInsertColumns)
                    {
                        if (nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
                        {
                            sqlServerDB.AddRow(tableName, columnValuePairs, dbName);
                            await Task.Delay(100);

                            DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);

                            return;
                        }
                        else if (nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
                        {
                            sqliteDB.AddRow(tableName, columnValuePairs); await Task.Delay(100);

                            DataTable.DataContext = GetDataTableSQLite(tableName);

                            return;
                        }
                    }
                }
                else if (nullableColumns.Count > 0)
                {
                    countInsertColumns -= nullableColumns.Count;
                    columnValuePairs.Add(editedColumn, editedValue);

                    if (columnValuePairs.Count >= countInsertColumns)
                    {
                        if (nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
                        {
                            sqlServerDB.AddRow(tableName, columnValuePairs, dbName);
                            await Task.Delay(100);

                            DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
                        }
                        else if (nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
                        {
                            sqliteDB.AddRow(tableName, columnValuePairs);
                            await Task.Delay(100);

                            DataTable.DataContext = GetDataTableSQLite(tableName);
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

            if (nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
            {
                sqlServerDB = new SQLServerConnector(serverName);
                sqlServerDB.DeleteRow(tableName, nameColumnIndefication, indefication, dbName);
                DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
                CustomNotificationBuilder.ShowNotificationOpacity("Успешное удаление в таблице!");
            }
            else if (nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
            {
                sqliteDB.DeleteRow(tableName, nameColumnIndefication, indefication);
                DataTable.DataContext = GetDataTableSQLite(tableName);
                CustomNotificationBuilder.ShowNotificationOpacity("Успешное удаление в таблице!");
            }
        }
        #endregion
    }
}
