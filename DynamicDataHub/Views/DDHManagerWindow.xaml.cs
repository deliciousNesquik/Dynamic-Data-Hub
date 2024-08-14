using DynamicDataHub.Helpers;
using DynamicDataHub.Interfaces;
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
using System.Xml.Linq;

namespace DynamicDataHub
{
    public partial class DDHManager : Window
    {
        #region vars
        private DDHAuthorization connectionWindow;
        private SQLServerConnector sqlServerDB;
        private SQLIteConnector sqliteDB;
        private GetDataTable getDataTable;
        private CustomMessageBoxBuilder customMessageBoxBuilder = new CustomMessageBoxBuilder();

        public string serverName { get; private set; }
        public string nameDbFile { get; private set; }

        private string nameDBManagementSystem;

        public string tableName { get; private set; }
        public string dbName { get; private set; }


        private UserControlDataTable dataTableControl = new UserControlDataTable(null);


        StackPanel InfoMessageStackPanel;
        TextBlock InfoMessageTextBlock;
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

        
        #endregion

        #region builders
        public DDHManager()
        {
            InitializeComponent();
            FilterRowsButton.Click += FilterRowsButtonOnClick;
            connectionWindow = new DDHAuthorization(this);
            dataTableControl.GetLinkWindow(this);
            CustomNotificationBuilder.CreateNotification(MainGrid);

        }
        #endregion

        #region function for work filter
        private void FilterRowsButtonOnClick(object sender, RoutedEventArgs e)
        {
            UpdateColumnsComboBox();
            if (this.tableName == null) return;
            var selectedColumn = ColumnsComboBox.SelectedItem as string;
            DataTable dataTable = null;
            if (string.IsNullOrEmpty(SearchLine.Text))
            {
                CustomNotificationBuilder.ShowNotificationOpacity("Напишите условие");
                switch (this.nameDBManagementSystem)
                {
                    case SQLIteConnector.nameDBManagementSystem:
                        dataTable = sqliteDB.CreateQuery($"SELECT * FROM {this.tableName}");
                        dataTableControl = new UserControlDataTable(dataTable);
                        FrameTableData.Navigate(dataTableControl);
                        break;

                    case SQLServerConnector.nameDBManagementSystem:
                        dataTable = sqlServerDB.CreateQuery($"SELECT * FROM {this.tableName}");
                        dataTableControl = new UserControlDataTable(dataTable);
                        FrameTableData.Navigate(dataTableControl);
                        break;
                }

                return;
            }

            switch (this.nameDBManagementSystem)
            {
                case SQLIteConnector.nameDBManagementSystem:
                    if (selectedColumn is null)
                    {
                        dataTable = sqliteDB.CreateQuery($"SELECT * FROM {this.tableName}");
                        dataTableControl = new UserControlDataTable(dataTable);
                        FrameTableData.Navigate(dataTableControl);
                        break;
                    }

                    dataTable = sqliteDB.CreateQuery(
                        $"SELECT * FROM {this.tableName} WHERE {selectedColumn} LIKE '{SearchLine.Text}'");
                    dataTableControl = new UserControlDataTable(dataTable);
                    FrameTableData.Navigate(dataTableControl);
                    break;
                case SQLServerConnector.nameDBManagementSystem:
                    if (selectedColumn is null)
                    {
                        dataTable = sqliteDB.CreateQuery($"SELECT * FROM {this.tableName}");
                        dataTableControl = new UserControlDataTable(dataTable);
                        FrameTableData.Navigate(dataTableControl);
                        break;
                    }
                    dataTable = sqlServerDB.CreateQuery(
                        $"SELECT * FROM {this.tableName} WHERE {selectedColumn} LIKE '{SearchLine.Text}'", this.dbName);
                    dataTableControl = new UserControlDataTable(dataTable);
                    FrameTableData.Navigate(dataTableControl);
                    break;
            }
        }

        private void UpdateColumnsComboBox()
        {
            switch (this.nameDBManagementSystem)
            {
                case SQLIteConnector.nameDBManagementSystem:
                    //sqliteDB.GetColumnNames(this.tableName).ForEach(Console.WriteLine);
                    ColumnsComboBox.ItemsSource = sqliteDB.GetColumnNames(this.tableName);
                    break;
                case SQLServerConnector.nameDBManagementSystem:
                    //sqlServerDB.GetColumnNames(this.tableName, this.dbName).ForEach(Console.WriteLine);
                    ColumnsComboBox.ItemsSource = sqlServerDB.GetColumnNames(this.tableName, this.dbName);
                    break;
            }
        }

        #endregion

        #region functions of connecting to databases
        public void ConnectionSQLServer(String ServerName, String NameDBManagementSystem)
        {
            TreeContent.Items.Clear();
            this.serverName = ServerName;
            this.nameDBManagementSystem = NameDBManagementSystem;

            sqlServerDB = new SQLServerConnector(ServerName);
            try
            {
                TreeViewItem Databases = new TreeViewItem() { Header = "Базы данных" };
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
                                TreeViewItem Table = new TreeViewItem() { Header = j };

                                Table.Selected += TableSelected;
                                Tables.Items.Add(Table);

                            }
                        }
                        else
                        {
                            CustomNotificationBuilder.ShowNotificationOpacity("База данных не содержит таблиц");
                        }

                    }
                }
                else
                {
                    CustomNotificationBuilder.ShowNotificationOpacity("Отсутствуют базы данных");
                }
            }
            catch (Exception ex)
            {
                customMessageBoxBuilder.ShowError("Внутренняя ошибка", ex.Message, "Назад", this);
            }
        }

        public void ConnectionSQLite(string NameDBFIle_, string NameDBManagementSystem)
        {
            TreeContent.Items.Clear();

            this.nameDBManagementSystem = NameDBManagementSystem;
            this.nameDbFile = NameDBFIle_;

            sqliteDB = new SQLIteConnector(NameDBFIle_);

            TreeViewItem Tables = new TreeViewItem() { Header = "Таблицы" };
            TreeContent.Items.Add(Tables);


            foreach (var i in sqliteDB.GetDBTables())
            {
                TreeViewItem Table = new TreeViewItem() { Header = i };

                Table.Selected += TableSelected;
                Tables.Items.Add(Table);
            }

        }
        #endregion

        #region handlers for interaction with UI elements
        private void TableSelected(object sender, RoutedEventArgs e)
        {
            FilterStackPanel.Visibility = Visibility.Visible;
            SearchLine.Clear();
            ColumnsComboBox.ItemsSource = null;
            getDataTable = new GetDataTable();

            TreeViewItem tableName = (TreeViewItem)sender;
            this.tableName = tableName.Header.ToString();

            FrameTableData.Content = null;
            CurrentTableTextblock.SetTable(this.tableName);

            switch (this.nameDBManagementSystem) {
                case SQLIteConnector.nameDBManagementSystem:
                {
                    UpdateColumnsComboBox();
                    TreeViewItem parent = (TreeViewItem)tableName.Parent;
                    foreach (TreeViewItem t in parent.Items)
                    {
                        t.BorderThickness = new Thickness(0);
                    }
                    DatabaseConfiguration.tableName = this.tableName;
                    dataTableControl = new UserControlDataTable(getDataTable.GetDataTableSQLite(this.tableName));
                    FrameTableData.Navigate(dataTableControl);

                    tableName.IsSelected = false;
                    tableName.BorderBrush = new SolidColorBrush(Colors.White);
                    tableName.BorderThickness = new Thickness(0.5);

                    break;
                }
                case SQLServerConnector.nameDBManagementSystem:
                {
                    if (tableName.Parent.GetType() == typeof(TreeViewItem))
                    {
                        TreeViewItem Tables = (TreeViewItem)tableName.Parent;
                        foreach(TreeViewItem t in Tables.Items)
                        {
                            t.BorderThickness = new Thickness(0);
                        }

                        if (Tables.Parent.GetType() == typeof(TreeViewItem))
                        {
                            TreeViewItem DB = (TreeViewItem)Tables.Parent;
                            this.tableName = tableName.Header.ToString();
                            this.dbName = DB.Header.ToString();
                            UpdateColumnsComboBox();

                            DatabaseConfiguration.tableName = this.tableName;
                            DatabaseConfiguration.dbName = this.dbName;

                            dataTableControl = new UserControlDataTable(getDataTable.GetDataTableSQLServer(this.tableName, this.dbName));
                            FrameTableData.Navigate(dataTableControl);
                            tableName.IsSelected = false;
                            tableName.BorderBrush = new SolidColorBrush(Colors.White);
                            tableName.BorderThickness = new Thickness(0.5);
                        }
                    }
                        break;
                }
                default:
                    break;
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
            FrameTableData.Navigate(null);
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
                WrapColumn.Margin = new Thickness(-100.5, 0, 0, 0);
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
                WrapColumn.Margin = new Thickness(48.5, 5, 0, 5);
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
        private void Window_ContentRendered(object sender, EventArgs e) { 

            connectionWindow.Focus();
            UpdateColumnsComboBox();
        }
        #endregion

        #region create a new user query (button click)
        private void NewQuery_Click(object sender, RoutedEventArgs e)
        {
            FilterStackPanel.Visibility = Visibility.Hidden;
            var queryEnvironment = new QueryExecutionEnvironment();
            FrameTableData.Navigate(queryEnvironment);
        }
        #endregion
    }
}
