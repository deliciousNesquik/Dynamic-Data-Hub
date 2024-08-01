using DynamicDataHub.Modules;
using DynamicDataHub.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace DynamicDataHub
{
    public partial class DDHManager : Window
    {
        #region Переменные
        private DDHAuthorization ConnectionWindow;
        private SQLServerConnector SqlServerDB;
        private SQLIteConnector SQLiteDB;
        private CustomMessageBoxBuilder customMessageBoxBuilder;

        private string ServerName;
        private string NameDbFile;

        private string NameDBManagementSystem;

        private int IndexOfColumn;
        private int IndexOfDataType;

        private string tableName;
        private string dbName;

        private string preparingCellForEditId;
        #endregion

        #region Внутренние функции
        private List<double> CallConnectionWindow(Frame owner)
        {
            var mainWindowBounds = new Rect(this.Left, this.Top, this.ActualWidth, this.ActualHeight);

            double absoluteX;
            double absoluteY;

            if (WindowState != WindowState.Maximized)
            {
                absoluteX = mainWindowBounds.X + ColumnObjectExplorer.Width.Value + 8 + ((owner.RenderSize.Width - ConnectionWindow.Width) / 2);
                absoluteY = mainWindowBounds.Y + RowObjectExplorer.Height.Value + RowConnecting.Height.Value + 2 + ((owner.RenderSize.Height - ConnectionWindow.Height) / 2);
            }
            else
            {
                absoluteX = ColumnObjectExplorer.Width.Value + 8 + ((owner.RenderSize.Width - ConnectionWindow.Width) / 2);
                absoluteY = RowObjectExplorer.Height.Value + RowConnecting.Height.Value + 2 + ((owner.RenderSize.Height - ConnectionWindow.Height) / 2);
            }

            List<double> positionElements = new List<double>();
            positionElements.Add(absoluteX);
            positionElements.Add(absoluteY);

            return positionElements;
        }
        #endregion

        #region Конструкторы
        public DDHManager()
        {
            InitializeComponent();
            ConnectionWindow = new DDHAuthorization(this);
        }
        #endregion

        #region Методы отображения данных
        public DataTable GetDataTableSQLServer(string TableName, string DBName)
        {
            DataTable.Visibility = Visibility.Visible;
            SqlServerDB = new SQLServerConnector(ServerName);

            DataTable table = new DataTable(TableName);
            DataTable databases = SqlServerDB.GetColumnTable(TableName, DBName);


            IndexOfColumn = 0;
            IndexOfDataType = 1;

            int rows_columns = databases.Rows.Count;

            foreach (DataRow _row in databases.Rows)
            {
                switch (_row[IndexOfDataType].ToString())
                {
                    case "int":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(Int32));
                        break;
                    case "nvarchar":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
                        break;
                    case "date":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(DateTime));
                        break;
                    case "bit":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(Boolean));
                        break;
                    case "nchar":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
                        break;
                    case "char":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
                        break;
                    case "varchar":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
                        break;
                    case "float":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(float));
                        break;
                    case "decimal":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(decimal));
                        break;
                    case "datetime":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(DateTime));
                        break;
                    case "image":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(string));
                        break;
                    case "uniqueidentifier":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(string));
                        break;
                    case "sysname":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(string));
                        break;
                    case "varbinary":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(string));
                        break;
                    default:
                        break;
                }
            }

            databases = SqlServerDB.IsIdentityColumn(tableName, dbName);

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


            databases = SqlServerDB.CreateQuery($"SELECT * FROM [{TableName}]", DBName);
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
            DataTable.Visibility = Visibility.Visible;
            SQLiteDB = new SQLIteConnector(NameDbFile);

            DataTable table = new DataTable(TableName);
            DataTable databases = SQLiteDB.GetColumnTable(TableName);

            IndexOfColumn = 1;
            IndexOfDataType = 2;

            int rows_columns = databases.Rows.Count;

            foreach (DataRow _row in databases.Rows)
            {
                switch (_row[IndexOfDataType].ToString())
                {
                    case "INTEGER":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(Int32));
                        break;
                    case "TEXT":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
                        break;
                    case "BLOB":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(byte));
                        break;
                    case "REAL":
                        table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(float));
                        break;
                }
            }


            databases = SQLiteDB.CreateQuery($"SELECT * FROM [{TableName}]");

            int count_columns = databases.Columns.Count;

            List<object> row_values = new List<object>();
            foreach (DataRow _row in databases.Rows)
            {
                for (int i = 0; i < count_columns; i++)
                {
                    row_values.Add(_row[i]);
                }
                try
                {
                    table.Rows.Add(row_values.ToArray());
                }
                catch (ArgumentException)
                {
                    customMessageBoxBuilder = new CustomMessageBoxBuilder();
                    customMessageBoxBuilder.ShowError("Ошибка", "У столбца в таблице отсутствует тип данных", "Ok", this);
                }

                row_values.Clear();
            }

            return table;
        }
        #endregion

        #region Методы подключения к базам данных
        public void ConnectionSQLServer(String ServerName, String NameDBManagementSystem)
        {
            DataTable.DataContext = null;
            TreeContent.Items.Clear();

            this.ServerName = ServerName;
            this.NameDBManagementSystem = NameDBManagementSystem;

            SqlServerDB = new SQLServerConnector(ServerName);
            try
            {
                TreeViewItem Databases = new TreeViewItem(){Header = "Базы данных"};
                TreeContent.Items.Add(Databases);

                if (SqlServerDB.GetDBNames().Count > 0)
                {
                    foreach (var i in SqlServerDB.GetDBNames())
                    {

                        TreeViewItem Database = new TreeViewItem() { Header = i };
                        Databases.Items.Add(Database);

                        TreeViewItem Tables = new TreeViewItem() { Header = "Таблицы" };
                        Database.Items.Add(Tables);

                        if (SqlServerDB.GetDBTables(i).Count > 0)
                        {
                            foreach (var j in SqlServerDB.GetDBTables(i))
                            {
                                TreeViewItem Table = new TreeViewItem() {Header = j};

                                Table.Selected += TableSelected;
                                Tables.Items.Add(Table);
                                
                            }
                        }
                        else
                        {
                            Console.WriteLine($"База данных: {i} не содержит таблиц");
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("нет баз данных");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ConnectionSQLite(string NameDBFIle_, string NameDBManagementSystem)
        {
            DataTable.DataContext = null;
            TreeContent.Items.Clear();

            this.NameDBManagementSystem = NameDBManagementSystem;
            this.NameDbFile = NameDBFIle_;

            SQLiteDB = new SQLIteConnector(NameDBFIle_);

            TreeViewItem Tables = new TreeViewItem() { Header = "Таблицы" };
            TreeContent.Items.Add(Tables);


            foreach(var i in SQLiteDB.GetDBTables())
            {
                TreeViewItem Table = new TreeViewItem() { Header=i };

                Table.Selected += TableSelected;
                Tables.Items.Add(Table);
            }
            
        }
        #endregion

        #region Обработчики взаимодействия с UI элементами
        private void TableSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tableName = (TreeViewItem)sender;
            this.tableName = tableName.Header.ToString();
            if (this.NameDBManagementSystem == "SQLite")
            {
                DataTable.DataContext = GetDataTableSQLite(tableName.Header.ToString());
            }
            else if (this.NameDBManagementSystem == "SQL Server Management Studio")
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
                            MessageBox.Show("Нет столбцов");
                        }
                    }
                }
            }                  
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { ConnectionWindow.Close(); }
        #endregion

        #region Обработчики открытия окна соединения
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<double> positionElement = CallConnectionWindow(FrameTableData);

            ConnectionWindow.WindowStartupLocation = WindowStartupLocation.Manual;


            ConnectionWindow.Left = positionElement[0];
            ConnectionWindow.Top = positionElement[1];

            ConnectionWindow.ShowDialog();
            ConnectionWindow.Focus();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ConnectionWindow.Focus();
        }
        
        #endregion

        #region Обрабочики сворачивания левой панели
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


        #region Обработчики нажатий на кнопки
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            List<double> positionElement = CallConnectionWindow(FrameTableData);

            ConnectionWindow = new DDHAuthorization(this);
            ConnectionWindow.WindowStartupLocation = WindowStartupLocation.Manual;


            ConnectionWindow.Left = positionElement[0];
            ConnectionWindow.Top = positionElement[1];

            ConnectionWindow.ShowDialog();
            ConnectionWindow.Focus();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            DataTable.DataContext = null;
            TreeContent.Items.Clear();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Обработчик взаимодействия с ContextMenu
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var _selectedCell = DataTable.SelectedCells[0];
            var nameColumnIndefication = DataTable.SelectedCells[0].Column.Header.ToString();
            var _cellContent = _selectedCell.Column.GetCellContent(_selectedCell.Item);
            var indefication = (_cellContent as TextBlock)?.Text;
            if(NameDBManagementSystem == "SQL Server Management Studio")
            {
                SqlServerDB = new SQLServerConnector(ServerName);
                SqlServerDB.DeleteRow(tableName, nameColumnIndefication, indefication, dbName);
                DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
            }
            else if (NameDBManagementSystem == "SQLite")
            {
                SQLiteDB.DeleteRow(tableName, nameColumnIndefication, indefication);
                DataTable.DataContext = GetDataTableSQLite(tableName);
            }
        }
        #endregion

        #region Обработчик взаимодействия с данными таблиц
        private async void DataTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedValue = ((TextBox)e.EditingElement).Text;

            var editedColumn = e.Column.Header.ToString();
            var basedColumn = DataTable.Columns[0].Header.ToString();

            if (NameDBManagementSystem == "SQL Server Management Studio")
            {
                SqlServerDB.UpdateRow(dbName, tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                await Task.Delay(100);
                DataTable.DataContext = GetDataTableSQLServer(tableName, dbName);
            }
            else if(NameDBManagementSystem == "SQLite")
            {
                SQLiteDB.UpdateRow(tableName, editedColumn, editedValue, basedColumn, preparingCellForEditId);
                await Task.Delay(100);
                DataTable.DataContext = GetDataTableSQLite(tableName);
            }

        }

        private void DataTable_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var _selectedCell = DataTable.SelectedCells[0];
            var _cellContent = _selectedCell.Column.GetCellContent(_selectedCell.Item);

            preparingCellForEditId = (_cellContent as TextBlock)?.Text;

        }
        #endregion
    }
}
