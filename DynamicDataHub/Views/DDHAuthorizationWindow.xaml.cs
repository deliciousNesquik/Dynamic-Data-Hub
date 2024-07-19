using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using DynamicDataHub.Modules;
using Microsoft.Win32;

namespace DynamicDataHub.Views
{
    public partial class DDHAuthorization : Window
    {

        CustomMessageBoxBuilder customMessageBox = new CustomMessageBoxBuilder();
        public List<string> _test = new List<string>();
        public string selectedFilePath;

        public void IsNullOrWhiteSpaceTextBox(string _serverName, string _dbName)
        {
            if (string.IsNullOrWhiteSpace(_serverName) && string.IsNullOrWhiteSpace(_dbName))
            {
                customMessageBox.ShowError("Ошибка", "Укажите имя сервера и базы данных!", "Закрыть");
            }
            else if (string.IsNullOrWhiteSpace(_serverName))
            {
                customMessageBox.ShowError("Ошибка", "Укажите имя сервера!", "Закрыть");
            }
            else if (string.IsNullOrWhiteSpace(_dbName))
            {
                customMessageBox.ShowError("Ошибка", "Укажите имя базы данных!", "Закрыть");
            }
        }
        public DDHAuthorization()
        {

            InitializeComponent();
            ChoosingDBManagementSystem Test = new ChoosingDBManagementSystem();
            _test = Test.GetDBManagementSystems();
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

        private void Window_LocationChanged(object sender, EventArgs e)
        {

        }

        private void DBMSComboBox_DropDownOpened(object sender, EventArgs e)
        {

            if (_test.Count == 0)
            {
                customMessageBox.ShowError("Ошибка", "У вас отсутствуют соответствующие СУБД", "Закрыть");
            }
        }

        private void DBMSComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!DBMSComboBox.SelectedItem.ToString().Contains("SQLite"))
            {
                NameDBBlock.Visibility = Visibility.Visible;
                NameDBBox.Visibility = Visibility.Visible;
                NameDBServerBlock.Text = "Имя сервера";
                NameDBServerBox.Clear();
                NameDBBox.Clear();
                OpenExplorer.Visibility = Visibility.Hidden;
            }
            else
            {
                NameDBBlock.Visibility = Visibility.Hidden;
                NameDBBox.Visibility = Visibility.Hidden;
                NameDBServerBlock.Text = "Файл базы данных";
                OpenExplorer.Visibility = Visibility.Visible;
            }
        }

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
            string _dbName;

            if (string.IsNullOrWhiteSpace(DBMSComboBox.Text))
            {
                customMessageBox.ShowError("Ошибка", "Выберите СУБД", "Закрыть");
            }
            else
            {
                string _currentDBManagementSystem = DBMSComboBox.SelectedItem.ToString().Trim();

                switch (_currentDBManagementSystem)
                {
                    case "DB Browser for SQLite":
                        _fileName = NameDBServerBox.Text;
                        if (!string.IsNullOrEmpty(_fileName))
                        {
                            var ManagerWindow = new DDHManager(_fileName);
                            SQLIteConnector test_connection = new SQLIteConnector(selectedFilePath);
                            if (test_connection.GetInfoConnection())
                                this.Close();
                            else
                            {
                                customMessageBox.ShowError("Ошибка", "Не удалось подключится к базе данных", "Закрыть");
                            }
                        }
                        else
                        {
                            customMessageBox.ShowError("Ошибка", "Выберите файл базы данных", "Закрыть");
                        }
                        break;
                    case "SQL Server Management Studio":
                        _serverName = NameDBServerBox.Text;
                        _dbName = NameDBBox.Text;
                        if (!string.IsNullOrWhiteSpace(_serverName) && !string.IsNullOrWhiteSpace(_dbName))
                        {
                            customMessageBox.ShowLoading("Подключение", "Подключение");
                            var ManagerWindow = new DDHManager(_serverName, _dbName);
                            SQLServerConnector test_connection = new SQLServerConnector(_serverName, _dbName);
                            bool isConnected;

                            isConnected = await test_connection.GetInfoConnection();

                            if (isConnected)
                            {
                                customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                                this.Close();
                            }
                            else
                            {
                                customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                                CustomMessageBoxBuilder.ClosingState = true;
                                customMessageBox.ShowError("Ошибка", "Не удалось подключится к базе данных", "Закрыть");
                            }
                        }
                        else
                        {
                            IsNullOrWhiteSpaceTextBox(_serverName, _dbName);
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Неизвестная система управления базами данных: {_currentDBManagementSystem}");
                }
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            customMessageBox.customMessageBox.Close();
        }
        //public class ConfigurationSettings
        //{
        //    public string Title
        //    {
        //        get { return ConfigurationManager.AppSettings["Title"] as String; }
        //    }
        //}
    }

}
