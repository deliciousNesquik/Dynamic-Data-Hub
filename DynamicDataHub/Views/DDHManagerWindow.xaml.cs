﻿using DynamicDataHub.Helpers;
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
        private UserControlDataTable dataTableControl;
        private string nameDBManagementSystem;

        private HashSet<string> tabItems = new HashSet<string>();


        /*
         * 0 - Russian
         * 1 - English
         */
        private int languageSelected = 0;
        private Dictionary<string, List<string>> localizationOfWords = new Dictionary<string, List<string>>
        {
            {"Title", new List<string>{"Database Manager", "Менеджер базы данных"}},
            {"Tables", new List<string>{"Tables", "Таблицы"}},
            {"Databases", new List<string>{"Databases", "Базы данных"}},
            {"NewQuery", new List<string>{"New Query", "Новый запрос"}},
            {"Settings", new List<string>{"Settings", "Настройки"}}, 
            {"TBObjectExplorer", new List<string>{ "Object Explorer", "Проводник"}},
            
            {"ConnectTB", new List<string>{ "Connect", "Соединить"}},
            {"DisconnectTB", new List<string>{ "Disconnect", "Отключить"}},
        };


        public string serverName { get; private set; }
        public string nameDbFile { get; private set; }
        public string tableName { get; private set; }
        public string dbName { get; private set; }
        #endregion

        #region internal functions
        private List<double> CallConnectionWindow(Frame owner)
        {
            var mainWindowBounds = new Rect(this.Left, this.Top, this.ActualWidth, this.ActualHeight);

            double absoluteX;
            double absoluteY;

            if (WindowState != WindowState.Maximized)
            {
                absoluteX = mainWindowBounds.X + LeftPanel.Width.Value + 8 + ((owner.RenderSize.Width - connectionWindow.Width) / 2);
                absoluteY = mainWindowBounds.Y + RowObjectExplorer.Height.Value + RowConnecting.Height.Value + 2 + ((owner.RenderSize.Height - connectionWindow.Height) / 2);
            }
            else
            {
                absoluteX = LeftPanel.Width.Value + 8 + ((owner.RenderSize.Width - connectionWindow.Width) / 2);
                absoluteY = RowObjectExplorer.Height.Value + RowConnecting.Height.Value + 2 + ((owner.RenderSize.Height - connectionWindow.Height) / 2);
            }

            List<double> positionElements = new List<double>();
            positionElements.Add(absoluteX);
            positionElements.Add(absoluteY);

            return positionElements;
        }

        private void AddTab()
        {
            Console.WriteLine("Добавление вкладки");
            if (tabItems.Add(DatabaseConfiguration.tableName))
            {
                TabItem tab = new TabItem();
                tab.Content = DatabaseConfiguration.dbName;

                StackPanel sp = new StackPanel 
                { 
                    Orientation = Orientation.Horizontal 
                };

                TextBlock textBlock = new TextBlock
                {
                    Text = DatabaseConfiguration.tableName,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button closeButton = new Button 
                { 
                    Content = "✖️",
                    Foreground = Brushes.White,
                    Width = 20, 
                    Height = 20, 
                    Margin = new Thickness(10, 0, 0, 0) 
                };
                closeButton.Click += CloseTabButtonClick; // Убедитесь, что у вас есть метод CloseTabButtonClick

                sp.Children.Add(textBlock);
                sp.Children.Add(closeButton);

                tab.Header = sp;
                tab.Focus();

                TabControlTable.Items.Add(tab);
            }
        }
        private void OpenTab(TabItem selectedTab)
        {
            StackPanel sp = selectedTab.Header as StackPanel;
            TextBlock tb = null;

            // Проверяем, действительно ли Header является StackPanel
            if (sp != null)
            {
                // Перебираем все дочерние элементы StackPanel
                foreach (UIElement child in sp.Children)
                {
                    // Проверяем, является ли дочерний элемент TextBlock
                    if (child is TextBlock)
                    {
                        tb = child as TextBlock;
                        break; // Выходим из цикла, так как нашли нужный TextBlock
                    }
                }
            }

            if (tb != null)
            {
                switch (this.nameDBManagementSystem) {
                    case SQLServerConnector.nameDBManagementSystem:
                        this.tableName = tb.Text; // Используем текст из найденного TextBlock
                        this.dbName = selectedTab.Content.ToString();

                        DatabaseConfiguration.tableName = this.tableName;
                        DatabaseConfiguration.dbName = this.dbName;

                        dataTableControl = new UserControlDataTable(getDataTable.GetDataTableSQLServer(this.tableName, this.dbName));
                        FrameData.Navigate(dataTableControl);
                        break;
                    case SQLIteConnector.nameDBManagementSystem:
                        this.tableName = tb.Text; // Используем текст из найденного TextBlock
                        //this.dbName = selectedTab.Content.ToString();

                        DatabaseConfiguration.tableName = this.tableName;
                        //DatabaseConfiguration.dbName = this.dbName;

                        dataTableControl = new UserControlDataTable(getDataTable.GetDataTableSQLite(this.tableName)); ;
                        FrameData.Navigate(dataTableControl);
                        break;
                }             
            }
            else
            {
                // Обработка случая, когда TextBlock не найден
                this.tableName = string.Empty;
            }
        }
        private void CloseTabButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                TabItem tabItem = FindParent<TabItem>(button);
                if (tabItem != null && TabControlTable.Items.Contains(tabItem))
                {
                    TabControlTable.Items.Remove(tabItem);

                    StackPanel sp = tabItem.Header as StackPanel;
                    TextBlock tb = null;

                    // Проверяем, действительно ли Header является StackPanel
                    if (sp != null)
                    {
                        // Перебираем все дочерние элементы StackPanel
                        foreach (UIElement child in sp.Children)
                        {
                            // Проверяем, является ли дочерний элемент TextBlock
                            if (child is TextBlock)
                            {
                                tb = child as TextBlock;
                                break; // Выходим из цикла, так как нашли нужный TextBlock
                            }
                        }
                    }

                    if (tb != null)
                    {
                        if (tabItems.Count == 1) FrameData.Navigate(null);
                        tabItems.Remove(tb.Text);
                    }

                }
            }
        }

        // Помощник для поиска родительского элемента заданного типа
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
        }


        #endregion

        #region builders
        public DDHManager()
        {
            InitializeComponent();
            
            connectionWindow = new DDHAuthorization(this);
            //dataTableControl.GetLinkWindow(this);
            CustomNotificationBuilder.CreateNotification(MainGrid);

            this.Title = localizationOfWords["Title"][languageSelected];
            NewQueryTB.Text = localizationOfWords["NewQuery"][languageSelected];
            SettingsTB.Text = localizationOfWords["Settings"][languageSelected];
            TBObjectExplorer.Text = localizationOfWords["TBObjectExplorer"][languageSelected];

            ConnectTB.Text = localizationOfWords["ConnectTB"][languageSelected];
            DisconnectTB.Text = localizationOfWords["DisconnectTB"][languageSelected];
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
                TreeViewItem Databases = new TreeViewItem() { Header = localizationOfWords["Databases"][languageSelected] };
                TreeContent.Items.Add(Databases);

                if (sqlServerDB.GetDBNames().Count > 0)
                {
                    foreach (var i in sqlServerDB.GetDBNames())
                    {
                        TreeViewItem Database = new TreeViewItem() { Header = i };
                        Databases.Items.Add(Database);

                        TreeViewItem Tables = new TreeViewItem() { Header = localizationOfWords["Tables"][languageSelected] };

                        ContextMenu contextMenuTreeViewItem = new ContextMenu();
                        MenuItem buttonTreeViewItem = new MenuItem();

                        buttonTreeViewItem.Header = "Создать таблицу";

                        buttonTreeViewItem.Click += NavigateToManagerTableUC;

                        contextMenuTreeViewItem.Items.Add(buttonTreeViewItem);

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
                            //CustomNotificationBuilder.ShowNotificationOpacity("База данных не содержит таблиц");
                        }

                        Tables.ContextMenu = contextMenuTreeViewItem;

                    }
                }
                else
                {
                    //CustomNotificationBuilder.ShowNotificationOpacity("Отсутствуют базы данных");
                }
            }
            catch (Exception ex)
            {
                //customMessageBoxBuilder.ShowError("Внутренняя ошибка", ex.Message, "Назад", this);
            }
        }

        private void NavigateToManagerTableUC(object sender, RoutedEventArgs e)
        {
            FrameData.Navigate(new TableManagerUserControl());
        }

        public void ConnectionSQLite(string NameDBFIle_, string NameDBManagementSystem)
        {
            TreeContent.Items.Clear();

            this.nameDBManagementSystem = NameDBManagementSystem;
            this.nameDbFile = NameDBFIle_;

            sqliteDB = new SQLIteConnector(NameDBFIle_);

            TreeViewItem Tables = new TreeViewItem() { Header = localizationOfWords["Tables"][languageSelected] };
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
            getDataTable = new GetDataTable();

            TreeViewItem tableName = (TreeViewItem)sender;
            this.tableName = tableName.Header.ToString();

            FrameData.Content = null;
            

            switch (this.nameDBManagementSystem) {
                case SQLIteConnector.nameDBManagementSystem:
                {
                    TreeViewItem parent = (TreeViewItem)tableName.Parent;
                    foreach (TreeViewItem t in parent.Items)
                    {
                        t.BorderThickness = new Thickness(0);
                    }
                    DatabaseConfiguration.tableName = this.tableName;
                    dataTableControl = new UserControlDataTable(getDataTable.GetDataTableSQLite(this.tableName));
                    FrameData.Navigate(dataTableControl);

                    tableName.IsSelected = false;
                    tableName.BorderBrush = new SolidColorBrush(Colors.White);
                    tableName.BorderThickness = new Thickness(0.5);

                    AddTab();

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

                            DatabaseConfiguration.tableName = this.tableName;
                            DatabaseConfiguration.dbName = this.dbName;

                            dataTableControl = new UserControlDataTable(getDataTable.GetDataTableSQLServer(this.tableName, this.dbName));
                            FrameData.Navigate(dataTableControl);
                            tableName.IsSelected = false;
                            tableName.BorderBrush = new SolidColorBrush(Colors.White);
                            tableName.BorderThickness = new Thickness(0.5);

                            AddTab();
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
            List<double> positionElement = CallConnectionWindow(FrameData);

            connectionWindow = new DDHAuthorization(this);
            connectionWindow.WindowStartupLocation = WindowStartupLocation.Manual;


            connectionWindow.Left = positionElement[0];
            connectionWindow.Top = positionElement[1];

            connectionWindow.ShowDialog();
            connectionWindow.Focus();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            FrameData.Navigate(null);
            TreeContent.Items.Clear();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region handlers for opening the connection window
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<double> positionElement = CallConnectionWindow(FrameData);

            connectionWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            connectionWindow.Left = positionElement[0];
            connectionWindow.Top = positionElement[1];

            connectionWindow.ShowDialog();
            connectionWindow.Focus();
        }
        private void Window_ContentRendered(object sender, EventArgs e) { 

            connectionWindow.Focus();
        }
        #endregion

        #region create a new user query (button click)
        private void NewQuery_Click(object sender, RoutedEventArgs e)
        {
            var queryEnvironment = new QueryExecutionEnvironment();
            FrameData.Navigate(queryEnvironment);
        }
        #endregion

        private void GridSplitter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (LeftPanel.Width.Value <= 190)
            {
                ConnectButton.Visibility = Visibility.Collapsed;
                if(LeftPanel.Width.Value <= 120)
                {
                    DisconnectButton.Visibility = Visibility.Collapsed;
                    TBObjectExplorer.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DisconnectButton.Visibility = Visibility.Visible;
                    TBObjectExplorer.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ConnectButton.Visibility = Visibility.Visible;
            }            
        }

        private void TabControlTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl != null)
            {
                var selectedTab = tabControl.SelectedItem as TabItem;
                if (selectedTab != null)
                {
                    OpenTab(selectedTab);
                }
            }
        }
    }
}
