using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DynamicDataHub.Modules;
using Microsoft.Win32;

namespace DynamicDataHub.Views
{
    public partial class DDHAuthorization : Window
    {
        private readonly CustomMessageBoxBuilder customMessageBox = new CustomMessageBoxBuilder();
        public List<string> _test = new List<string>();
        public string selectedFilePath;

        public void IsNullOrWhiteSpaceTextBox(string _serverName, string _dbName)
        {
            if (string.IsNullOrWhiteSpace(_serverName) && string.IsNullOrWhiteSpace(_dbName))
                customMessageBox.ShowError("Ошибка", "Укажите имя сервера и базы данных!", "Закрыть");
            //MessageBox.Show("Укажите имя сервера и базы данных!", "");
            else if (string.IsNullOrWhiteSpace(_serverName))
                customMessageBox.ShowError("Ошибка", "Укажите имя сервера!", "Закрыть");
            //MessageBox.Show("Укажите имя сервера!");
            else if (string.IsNullOrWhiteSpace(_dbName))
                customMessageBox.ShowError("Ошибка", "Укажите имя базы данных!", "Закрыть");
            //MessageBox.Show("Укажите имя базы данных!");
        }

        public DDHAuthorization()
        {
            //customMessageBox.customMessageBox.Show();
            //customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
            InitializeComponent();
            var Test = new ChoosingDBManagementSystem();
            _test = Test.GetDBManagementSystems();
            foreach (var test in _test) DBMSComboBox.Items.Add(test);

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
                customMessageBox.ShowError("Ошибка", "У вас отсутствуют соответствующие СУБД", "Закрыть");
            //MessageBox.Show("У вас отсутствуют соответствующие СУБД");
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
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Database Files (*.db)|*.db";

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFilePath = openFileDialog.FileName;
                this.selectedFilePath = selectedFilePath;
                var lastSlashIndex = selectedFilePath.LastIndexOf('\\');
                var lastIndex = selectedFilePath.Length - 1;
                var result = selectedFilePath.Substring(lastSlashIndex + 1, lastIndex - lastSlashIndex);
                NameDBServerBox.Text = result;
            }
        }


        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            string _fileName;
            string _serverName;
            string _dbName;

            if (string.IsNullOrWhiteSpace(DBMSComboBox.Text))
            {
                customMessageBox.ShowError("Ошибка", "Выберите СУБД", "Закрыть");
                //MessageBox.Show("Выберите СУБД");
            }
            else
            {
                var _currentDBManagementSystem = DBMSComboBox.SelectedItem.ToString().Trim();

                switch (_currentDBManagementSystem)
                {
                    case "DB Browser for SQLite":
                        _fileName = NameDBServerBox.Text;
                        if (!string.IsNullOrEmpty(_fileName))
                        {
                            var ManagerWindow = new DDHManager(_fileName);
                            var test_connection = new SQLIteConnector(selectedFilePath);
                            if (test_connection.GetInfoConnection())
                                Close();
                            else
                                customMessageBox.ShowError("Ошибка", "Не удалось подключится к базе данных", "Закрыть");
                            //MessageBox.Show("Не удалось подключится к базе данных");
                        }
                        else
                        {
                            customMessageBox.ShowError("Ошибка", "Выберите файл базы данных", "Закрыть");
                            //MessageBox.Show("Выберите файл базы данных");
                        }

                        break;
                    case "SQL Server Management Studio":
                        _serverName = NameDBServerBox.Text;
                        _dbName = NameDBServerBox.Text;
                        if (!string.IsNullOrWhiteSpace(_serverName) && !string.IsNullOrWhiteSpace(_dbName))
                        {
                            var ManagerWindow = new DDHManager(_serverName, _dbName);
                            var test_connection = new SQLServerConnector(_serverName, _dbName);
                            if (test_connection.GetInfoConnection())
                                Close();
                            else
                                customMessageBox.ShowError("Ошибка", "Не удалось подключится к базе данных", "Закрыть");
                            //MessageBox.Show("Не удалось подключится к базе данных");
                        }
                        else
                        {
                            IsNullOrWhiteSpaceTextBox(_serverName, _dbName);
                        }

                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Неизвестная система управления базами данных: {_currentDBManagementSystem}");
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            customMessageBox.CustomMessageBox.Close();
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