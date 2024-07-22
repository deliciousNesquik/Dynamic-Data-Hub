using DynamicDataHub.Modules;
using DynamicDataHub.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicDataHub
{
    public partial class DDHManager : Window
    {
        #region Переменные
        public static Window ConnectionWindow = new DDHAuthorization();
        private SQLServerConnector SqlServerDB;
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
        }
        public DDHManager(string ServerName, string DBName)
        {
            InitializeComponent();
            TableList.Items.Clear();

            SqlServerDB = new SQLServerConnector(ServerName, DBName);

            var databases = SqlServerDB.CreateQuery("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
            try
            {
                foreach (DataRow row in databases.Rows)
                {
                    if (row[0].ToString() == "sysdiagrams")
                    {
                        continue;
                    }
                    if (TableList != null)
                    {
                        var listItem = new ListBoxItem { Content = row[0].ToString(), Visibility = Visibility.Visible };
                        TableList.Items.Add(listItem);

                    }
                    else
                    {
                        MessageBox.Show("TableList не инициализирован");
                    }
                }
            } 
            catch (Exception ex){ 
                MessageBox.Show(ex.Message);
            }

        }
        public DDHManager(string DBFileName)
        {
            InitializeComponent();
        }
        #endregion

        #region Обработчики открытия окна соединения
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<double> positionElement = CallConnectionWindow(FrameTableData);

            ConnectionWindow = new DDHAuthorization();
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

            ConnectionWindow = new DDHAuthorization();
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

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TableList.DataContext = SqlServerDB.GetDBTables(TableList);
        }
    }
}
