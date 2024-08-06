using DynamicDataHub.Modules;
using DynamicDataHub.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DynamicDataHub
{
    public partial class DDHManager : Window
    {
        #region vars
        private DDHAuthorization connectionWindow;
        private SQLServerConnector sqlServerDB;
        private SQLIteConnector sqliteDB;
        private CustomMessageBoxBuilder customMessageBoxBuilder;

        private string serverName;
        private string nameDbFile;

        private string nameDBManagementSystem;

        private int indexOfColumn;
        private int indexOfDataType;
        private int indexOfIsNullable;

        private string tableName;
        private string dbName;

        private string preparingCellForEditId;

        private List<string> nullableColumns = new List<string>();
        private Dictionary<string, string> columnValuePairs = new Dictionary<string, string>();
        #endregion

        #region internal functions
        private List<double> CallConnectionWindow(Frame owner)
        {
            var mainWindowBounds = new Rect(this.Left, this.Top, this.ActualWidth, this.ActualHeight);

            double absoluteX;
            double absoluteY;

            if (WindowState != WindowState.Maximized)
            {
                absoluteX = mainWindowBounds.X + ColumnObjectExplorer.Width.Value + 8 + ((owner.RenderSize.Width - connectionWindow.Width) / 2);
                absoluteY = mainWindowBounds.Y + RowObjectExplorer.Height.Value + RowConnecting.Height.Value + 2 + ((owner.RenderSize.Height - connectionWindow.Height) / 2);
            }
            else
            {
                absoluteX = ColumnObjectExplorer.Width.Value + 8 + ((owner.RenderSize.Width - connectionWindow.Width) / 2);
                absoluteY = RowObjectExplorer.Height.Value + RowConnecting.Height.Value + 2 + ((owner.RenderSize.Height - connectionWindow.Height) / 2);
            }

            List<double> positionElements = new List<double>();
            positionElements.Add(absoluteX);
            positionElements.Add(absoluteY);

            return positionElements;
        }

        public void ShowNotification(string notificationMessage)
        {
            double beginState = 0;
            double endState = 1;
            double animationSeconds = 3;

            DoubleAnimation notificationMessageAnim = new DoubleAnimation();
            notificationMessageAnim.From = beginState;
            notificationMessageAnim.To = endState;
            notificationMessageAnim.Duration = TimeSpan.FromSeconds(animationSeconds);

            InfoMessageTextBlock.Text = notificationMessage;
            InfoMessageStackPanel.BeginAnimation(StackPanel.OpacityProperty, notificationMessageAnim);

            notificationMessageAnim.From = endState;
            notificationMessageAnim.To = beginState;
            notificationMessageAnim.Duration = TimeSpan.FromSeconds(animationSeconds);
            InfoMessageStackPanel.BeginAnimation(StackPanel.OpacityProperty, notificationMessageAnim);
        }
        #endregion

        #region builders
        public DDHManager()
        {
            InitializeComponent();
            connectionWindow = new DDHAuthorization(this);
        }
        #endregion

        #region functions for displaying data
        public DataTable GetDataTableSQLServer(string TableName, string DBName)
        {
            nullableColumns.Clear();
            DataTable.Visibility = Visibility.Visible;
            sqlServerDB = new SQLServerConnector(serverName);

            DataTable table = new DataTable(TableName);
            DataTable databases = sqlServerDB.GetColumnTable(TableName, DBName);


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

            databases = sqlServerDB.IsIdentityColumn(tableName, dbName);

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


            databases = sqlServerDB.CreateQuery($"SELECT * FROM [{TableName}]", DBName);
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

        public DataTable GetDataTableSQLite(string TableName)
        {
            nullableColumns.Clear();

            DataTable.Visibility = Visibility.Visible;
            sqliteDB = new SQLIteConnector(nameDbFile);

            DataTable table = new DataTable(TableName);
            DataTable dataBases = sqliteDB.GetColumnTable(TableName);

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


            dataBases = sqliteDB.CreateQuery($"SELECT * FROM [{TableName}]");

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
                    customMessageBoxBuilder = new CustomMessageBoxBuilder();
                    customMessageBoxBuilder.ShowError("Ошибка", "У столбца в таблице отсутствует тип данных", "Ok", this);
                }

                rowValues.Clear();
            }

            return table;
        }
        #endregion

        #region functions of connecting to databases
        public void ConnectionSQLServer(String ServerName, String NameDBManagementSystem)
        {
            DataTable.DataContext = null;
            TreeContent.Items.Clear();

            this.serverName = ServerName;
            this.nameDBManagementSystem = NameDBManagementSystem;

            sqlServerDB = new SQLServerConnector(ServerName);
            try
            {
                TreeViewItem Databases = new TreeViewItem(){Header = "Базы данных"};
                TreeContent.Items.Add(Databases);

                if (sqlServerDB.GetDBNames().Count > 0)
                {
                    foreach (var i in sqlServerDB.GetDBNames())
                    {
                        TreeViewItem Database = new TreeViewItem() { Header = i };
                        Databases.Items.Add(Database);

                        TreeViewItem Tables = new TreeViewItem() { Header = "Таблицы" };
                        Database.Items.Add(Tables);

                        if (sqlServerDB.GetDBTables(i).Count > 0)
                        {
                            foreach (var j in sqlServerDB.GetDBTables(i))
                            {
                                TreeViewItem Table = new TreeViewItem() {Header = j};

                                Table.Selected += TableSelected;
                                Tables.Items.Add(Table);
                                
                            }
                        }
                        else
                        {
                            ShowNotification("База данных не содержит таблиц");
                        }
                        
                    }
                }
                else
                {
                    ShowNotification("Отсутствуют базы данных");
                }
            }
            catch (Exception ex)
            {
                customMessageBoxBuilder.ShowError("Внутренняя ошибка", ex.Message, "Назад", this);
            }
        }

        public void ConnectionSQLite(string NameDBFIle_, string NameDBManagementSystem)
        {
            DataTable.DataContext = null;
            TreeContent.Items.Clear();

            this.nameDBManagementSystem = NameDBManagementSystem;
            this.nameDbFile = NameDBFIle_;

            sqliteDB = new SQLIteConnector(NameDBFIle_);

            TreeViewItem Tables = new TreeViewItem() { Header = "Таблицы" };
            TreeContent.Items.Add(Tables);


            foreach(var i in sqliteDB.GetDBTables())
            {
                TreeViewItem Table = new TreeViewItem() { Header=i };

                Table.Selected += TableSelected;
                Tables.Items.Add(Table);
            }
            
        }
        #endregion

        #region handlers for interaction with UI elements
        private void TableSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tableName = (TreeViewItem)sender;
            this.tableName = tableName.Header.ToString();

            if (this.nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
            {
                DataTable.DataContext = GetDataTableSQLite(tableName.Header.ToString());
            }
            else if (this.nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
            {
                if (tableName.Parent.GetType() == typeof(TreeViewItem)) // verify that parent is TreeViewItem
                {
                    TreeViewItem Tables = (TreeViewItem)tableName.Parent;

                    if (Tables.Parent.GetType() == typeof(TreeViewItem))
                    {
                        TreeViewItem DB = (TreeViewItem)Tables.Parent;
                        this.tableName = tableName.Header.ToString();
                        this.dbName = DB.Header.ToString();

                        if (GetDataTableSQLServer(tableName.Header.ToString(), DB.Header.ToString()).Columns.Count > 0)
                        {
                            DataTable.DataContext = GetDataTableSQLServer(tableName.Header.ToString(), DB.Header.ToString());
                        }
                        else
                        {
                            customMessageBoxBuilder.ShowError("Ошибка в таблице", "В данной таблице отсутствуют столбцы", "Назад", this);
                        }
                    }
                }
            }                  
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { connectionWindow.Close(); }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            List<double> positionElement = CallConnectionWindow(FrameTableData);

            connectionWindow = new DDHAuthorization(this);
            connectionWindow.WindowStartupLocation = WindowStartupLocation.Manual;


            connectionWindow.Left = positionElement[0];
            connectionWindow.Top = positionElement[1];

            connectionWindow.ShowDialog();
            connectionWindow.Focus();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            DataTable.DataContext = null;
            TreeContent.Items.Clear();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WrapColumn_Click(object sender, RoutedEventArgs e)
        {
            //Создание обьекта класса ротейрТрансформ 
            RotateTransform rotateTransform = new RotateTransform(0);
            //Получение ротейрТрансформ у кнопки для дальнейшей логики
            var WrapColumnTransform = WrapColumn.RenderTransform as RotateTransform;

            //Проверка если угол кнопки равен 0, тогда панельку можно свернуть
            if (WrapColumnTransform?.Angle == 0 || WrapColumnTransform?.Angle == null)
            {
                //Скрытие элементов
                TBObjectExplorer.Visibility = Visibility.Hidden;
                Connect.Visibility = Visibility.Hidden;
                Disconnect.Visibility = Visibility.Hidden;
                //Сдвиг кнопки влево
                WrapColumn.Margin = new Thickness(-142.5, 0, 0, 0);
                Refresh.Margin = new Thickness(-145, 0, 0, 0);
                //Создания трансформа поворота и поворот кнопки на 180
                rotateTransform = new RotateTransform(180);
                WrapColumn.RenderTransform = rotateTransform;
                //Установление размера для колонки
                ColumnObjectExplorer.Width = new GridLength(30);
            }
            //Если угол не 0 тогда панельку нужно развернуть
            else
            {
                //Отображение элементов
                TBObjectExplorer.Visibility = Visibility.Visible;
                Connect.Visibility = Visibility.Visible;
                Disconnect.Visibility = Visibility.Visible;
                //Сдвиг кнопки на прежнее место
                WrapColumn.Margin = new Thickness(68.5, 5, 0, 5);
                Refresh.Margin = new Thickness(5, 0, 0, 0);
                //Создания трансформа поворота и поворот кнопки на 0
                rotateTransform = new RotateTransform(0);
                WrapColumn.RenderTransform = rotateTransform;
                //Установление размера для колонки
                ColumnObjectExplorer.Width = new GridLength(180);
            }

        }
        #endregion

        #region handlers for opening the connection window
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<double> positionElement = CallConnectionWindow(FrameTableData);

            connectionWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            connectionWindow.Left = positionElement[0];
            connectionWindow.Top = positionElement[1];

            connectionWindow.ShowDialog();
            connectionWindow.Focus();
        }
        private void Window_ContentRendered(object sender, EventArgs e){connectionWindow.Focus();}
        #endregion

        #region handler for interaction with ContextMenu
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var _selectedCell = DataTable.SelectedCells[0];
            var nameColumnIndefication = DataTable.SelectedCells[0].Column.Header.ToString();
            var _cellContent = _selectedCell.Column.GetCellContent(_selectedCell.Item);
            var indefication = (_cellContent as TextBlock)?.Text;

            if(nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
            {
                sqlServerDB = new SQLServerConnector(serverName);
                sqlServerDB.DeleteRow(tableName, nameColumnIndefication, indefication, dbName);
                DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
                ShowNotification("Успешное удаление в таблице!");
            }
            else if (nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
            {
                sqliteDB.DeleteRow(tableName, nameColumnIndefication, indefication);
                DataTable.DataContext = GetDataTableSQLite(tableName);
                ShowNotification("Успешное удаление в таблице!");
            }
        }
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
                if (nameDBManagementSystem == SQLServerConnector.NameDBManagementSystem)
                {
                    sqlServerDB.UpdateRow(dbName, tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                    await Task.Delay(100);

                    DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
                    ShowNotification("Успешное изменение в таблице!");
                }
                else if (nameDBManagementSystem == SQLIteConnector.NameDBManagementSystem)
                {
                    sqliteDB.UpdateRow(tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                    await Task.Delay(100);

                    DataTable.DataContext = GetDataTableSQLite(tableName);
                    ShowNotification("Успешное изменение в таблице!");
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

        private void DataTable_AddingNewItem(object sender, AddingNewItemEventArgs e) => ShowNotification("Успешное добавление в таблицу");
    }
}
