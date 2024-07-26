using DynamicDataHub.Modules;
using DynamicDataHub.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private string DBName;
        private string ServerName;
        private string NameDBFile;

        private string NameDBManagementSystem;

        private int IndexOfColumn;
        private int IndexOfDataType;
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

        //public DataTable GetDataTableSQLServer(ListBox TableList)
        //{
        //    if (TableList.SelectedItem == null) {
        //        DataTable.Visibility = Visibility.Hidden;
        //        return null;
        //    }
        //    else {
        //        DataTable.Visibility = Visibility.Visible;
        //        SqlServerDB = new SQLServerConnector(ServerName, DBName);
        //        DataTable table = new DataTable(TableList.SelectedItem.ToString());
        //        DataTable databases = null;


        //        databases = SqlServerDB.GetColumnTable(ListBoxTableList);


        //        IndexOfColumn = 0;
        //        IndexOfDataType = 1;

        //        int rows_columns = databases.Rows.Count;

        //        foreach (DataRow _row in databases.Rows)
        //        {
        //            switch (_row[IndexOfDataType].ToString())
        //            {
        //                case "int":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(Int32));
        //                    break;
        //                case "nvarchar":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
        //                    break;
        //                case "date":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(DateTime));
        //                    break;
        //                case "bit":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(Boolean));
        //                    break;
        //                case "nchar":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
        //                    break;
        //                case "char":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
        //                    break;
        //                case "varchar":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
        //                    break;
        //                case "float":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(float));
        //                    break;
        //                case "decimal":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(decimal));
        //                    break;
        //                case "datetime":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(DateTime));
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        SqlServerDB = null;
        //        databases = null;

        //        SqlServerDB = new SQLServerConnector(ServerName, DBName);
        //        databases = SqlServerDB.CreateQuery("SELECT * FROM [" + TableList.SelectedItem.ToString() + "]");

        //        int count_columns = databases.Columns.Count;

        //        List<object> row_values = new List<object>();
        //        foreach (DataRow _row in databases.Rows)
        //        {
        //            for (int i = 0; i < count_columns; i++)
        //            {
        //                row_values.Add(_row[i]);
        //                Console.WriteLine(_row[i]);
        //            }

        //            table.Rows.Add(row_values.ToArray());

        //            row_values.Clear();
        //        }

        //        return table;
        //    }
        //}

        //public DataTable GetDataTableSQLite(ListBox TableList)
        //{
        //    if (TableList.SelectedItem == null)
        //    {
        //        DataTable.Visibility = Visibility.Hidden;
        //        return null;
        //    }
        //    else
        //    {
        //        DataTable.Visibility = Visibility.Visible;
        //        SQLiteDB = new SQLIteConnector(NameDBFile);
        //        DataTable table = new DataTable(TableList.SelectedItem.ToString());
        //        DataTable databases = null;
        //        databases = SQLiteDB.GetColumnTable(ListBoxTableList);
        //        IndexOfColumn = 1;
        //        IndexOfDataType = 2;

        //        int rows_columns = databases.Rows.Count;

        //        foreach (DataRow _row in databases.Rows)
        //        {
        //            switch (_row[IndexOfDataType].ToString())
        //            {
        //                case "INTEGER":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(Int32));
        //                    break;
        //                case "TEXT":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(String));
        //                    break;
        //                case "BLOB":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(byte));
        //                    break;
        //                case "REAL":
        //                    table.Columns.Add(_row[IndexOfColumn].ToString(), typeof(float));
        //                    break;
        //            }
        //        }

        //        SQLiteDB = null;
        //        databases = null;

        //        SQLiteDB = new SQLIteConnector(NameDBFile);
        //        databases = SQLiteDB.CreateQuery("SELECT * FROM [" + TableList.SelectedItem.ToString() + "]");

        //        int count_columns = databases.Columns.Count;

        //        List<object> row_values = new List<object>();
        //        foreach (DataRow _row in databases.Rows)
        //        {
        //            for (int i = 0; i < count_columns; i++)
        //            {
        //                row_values.Add(_row[i]);
        //            }
        //            try
        //            {
        //                table.Rows.Add(row_values.ToArray());
        //            }
        //            catch(ArgumentException ex) {
        //                customMessageBoxBuilder = new CustomMessageBoxBuilder();
        //                customMessageBoxBuilder.ShowError("Ошибка", "У столбца в таблице отсутствует тип данных", "Ok", this);
        //            }

        //            row_values.Clear();
        //        }

        //        return table;
        //    }
        //}
        public DDHManager()
        {
            InitializeComponent();
            ConnectionWindow = new DDHAuthorization(this);
        }
        public void ConnectionSQLServer(String ServerName)
        {
            this.ServerName = ServerName;
            SqlServerDB = new SQLServerConnector(ServerName);
            try
            {
                TreeViewItem Databases = new TreeViewItem(){Header = "Базы данных"};
                if (SqlServerDB.GetDBNames().Count > 0)
                {
                    foreach (var i in SqlServerDB.GetDBNames())
                    {
                        TreeViewItem Database = new TreeViewItem() { Header = i };

                        TreeViewItem Tables = new TreeViewItem() { Header = "Таблицы" };
                        if(SqlServerDB.GetDBTables(i).Count > 0)
                        {
                            foreach (var j in SqlServerDB.GetDBTables(i))
                            {
                                TreeViewItem Table = new TreeViewItem() { Header = i };
                                Tables.Items.Add(Table);
                            }
                            Database.Items.Add(Tables);
                            Databases.Items.Add(Database);
                        }
                        
                    }
                    TreeContent.Items.Add(Databases);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ConnectionSQLite(string NameDBFIle_, string NameDBManagementSystem)
        {
            this.NameDBManagementSystem = NameDBManagementSystem;
            this.NameDBFile = NameDBFIle_;
            SQLiteDB = new SQLIteConnector(NameDBFIle_);
            try
            {
                //SQLiteDB.GetDBTables(ListBoxTableList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { ConnectionWindow.Close(); }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            TreeContent.Items.Clear();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }


        //private void ListBoxTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if(NameDBManagementSystem == "SQL Server Management Studio")
        //        DataTable.DataContext = GetDataTableSQLServer(ListBoxTableList);
        //    else
        //        DataTable.DataContext = GetDataTableSQLite(ListBoxTableList);
        //}

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
