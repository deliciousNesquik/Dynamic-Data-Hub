using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using DynamicDataHub.Modules;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace DynamicDataHub.Views
{
    public partial class DDHAuthorization : Window
    {
        #region Переменные
        private CustomMessageBoxBuilder customMessageBox;
        private DDHManager ddhManager;
        private List<string> _test;
        private string selectedFilePath;

        #endregion

        #region Внутренние функции
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(this.Left + ";" + this.Top);
        }
        #endregion

        #region Конструкторы
        public DDHAuthorization(DDHManager w)
        {
            InitializeComponent();

            this.ddhManager = w;
            this.customMessageBox = new CustomMessageBoxBuilder();
            ChoosingDBManagementSystem Test = new ChoosingDBManagementSystem();

            customMessageBox.CenterInParentWindow(this);
            
            _test = Test.GetDBManagementSystems();
            DBMSComboBox.Items.Add("Не выбрано");

            foreach (string test in _test)
            {
                DBMSComboBox.Items.Add(test);
            }
            

            prodUpalLink.RequestNavigate += OnlLinkOnRequestNavigate;
            nxtvrturLink.RequestNavigate += OnlLinkOnRequestNavigate;
            deliciousNesquikLink.RequestNavigate += OnlLinkOnRequestNavigate;
            return;

            void OnlLinkOnRequestNavigate(object s, RequestNavigateEventArgs e)
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
        }
        #endregion

        #region Методы для работы с ComboBox
        private void DBMSComboBox_DropDownOpened(object sender, EventArgs e)
        {

            if (_test.Count == 0)
            {
                customMessageBox.ShowError("Ошибка", "У вас отсутствуют соответствующие СУБД", "Закрыть", this);
            }
        }
        private void DBMSComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!DBMSComboBox.SelectedItem.ToString().Contains("SQLite"))
            {
                NameDBServerBlock.Text = "Имя сервера";
                NameDBServerBox.Clear();
                OpenExplorer.Visibility = Visibility.Hidden;
            }
            else
            {
                NameDBServerBlock.Text = "Файл базы данных";
                NameDBServerBox.Clear();
                OpenExplorer.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Методы для соединения с базой данных
        private void OpenExplorer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Database Files (*.db)|*.db";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                this.selectedFilePath = selectedFilePath;
                int lastSlashIndex = selectedFilePath.LastIndexOf('\\');
                int lastIndex = selectedFilePath.Length - 1;
                string result = selectedFilePath.Substring(lastSlashIndex + 1, lastIndex - lastSlashIndex);
                NameDBServerBox.Text = result;
            }
        }
        private async void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {

            string _fileName;
            string _serverName;

            string _currentDBManagementSystem = DBMSComboBox.SelectedItem.ToString().Trim();

            switch (_currentDBManagementSystem)
            {
                case "SQLite":
                    _fileName = NameDBServerBox.Text;
                    if (!string.IsNullOrWhiteSpace(_fileName))
                    {
                        SQLIteConnector test_connection = new SQLIteConnector(selectedFilePath);
                        bool isConnected;

                        isConnected = await test_connection.GetInfoConnection();
                        if (isConnected)
                        {
                            this.ddhManager.ConnectionSQLite(this.selectedFilePath, SQLIteConnector.NameDBManagementSystem);
                            customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                            this.Close();
                        }
                        else
                        {
                            customMessageBox.ShowError("Ошибка", "Не удалось подключится к базе данных", "Закрыть", this);
                        }
                    }
                    else
                    {
                        customMessageBox.ShowError("Ошибка", "Выберите файл базы данных", "Закрыть", this);
                    }
                    break;
                case "SQL Server Management Studio":

                    _serverName = NameDBServerBox.Text;

                    if (!string.IsNullOrWhiteSpace(_serverName))
                    {
                        customMessageBox.ShowLoading("Подключение", "Подключение", this);
                        SQLServerConnector test_connection = new SQLServerConnector(_serverName);
                        bool isConnected;

                        isConnected = await test_connection.GetInfoConnection();

                        if (isConnected)
                        {
                            this.ddhManager.ConnectionSQLServer(_serverName, SQLServerConnector.NameDBManagementSystem);
                            customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                            this.Close();
                        }
                        else
                        {
                            customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                            CustomMessageBoxBuilder.ClosingState = true;
                            customMessageBox.ShowError("Ошибка", "Не удалось подключится к базе данных", "Закрыть", this);
                        }
                    }
                    else
                    {
                        customMessageBox.ShowError("Ошибка", "Укажите имя сервера!", "Закрыть", this);
                    }
                    break;

                case "Не выбрано":
                    customMessageBox.ShowError("Ошибка", "Выберите СУБД", "Закрыть", this);
                    break;
                default:
                    throw new InvalidOperationException($"Неизвестная система управления базами данных: {_currentDBManagementSystem}");
            }
        }
        #endregion

        #region Обработчики закрытий окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e){customMessageBox.customMessageBox.Close();}
        private void CloseWindow_Click(object sender, RoutedEventArgs e){this.Close();}
        #endregion

    }
}
