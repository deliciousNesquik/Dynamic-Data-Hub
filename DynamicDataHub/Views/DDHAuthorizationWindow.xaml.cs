using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DynamicDataHub.Modules;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;

namespace DynamicDataHub.Views
{
    #region class for working with json
    class DataBasesListJson
    {
        public List<string> DataBasesList { get; private set; } = new List<string>();
    }
    #endregion

    public partial class DDHAuthorization : Window
    {
        #region vars

        //Переменная для вызова модального окна с информацией
        private CustomMessageBoxBuilder customMessageBox;
        //Переменная для хранения обьекта ConnectionWindow
        private DDHManager ddhManager;
        //Переменная для хранения путя к файлу
        private string selectedFilePath;

        /*
         * 0 - Russian
         * 1 - English
         */
        private int languageSelected = 0;
        private Dictionary<string, List<string>> localizationOfWords = new Dictionary<string, List<string>>
        {
            {"NameDBServerBlock_0", new List<string>{"Server Name", "Имя сервера"}},
            {"NameDBServerBlock_1", new List<string>{"Database file", "Файл базы данных"}},
            {"ConnectionTB", new List<string>{"Connection", "Соединить"}},
            {"CreationsByTB", new List<string>{"made by", "создано"}},
            {"NotSelected", new List<string>{"Not selected", "Не выбрано"}},
            {"Error", new List<string>{"Error", "Ошибка"}},
            {"Close", new List<string>{"Close", "Закрыть"}},
            {"Connection", new List<string>{"Connection", "Подключение"}},
            {"FailedConnectDB", new List<string>{"Failed to connect to the database", "Не удалось подключится к базе данных"}},
            {"NoSelectDBFile", new List<string>{"Select the database file", "Выберите файл базы данных"}},
            {"SpecifyServerName", new List<string>{"Specify the server name", "Укажите имя сервера"}},
            {"SelectDBMS", new List<string>{ "Select a DBMS", "Выберите СУБД"}},
            
            {"ToolTipNameDBServerBox", new List<string>{"A field for entering the name of the server to connect to for further work with databases", "Поле для ввода имени сервера к которому необходимо подключиться для дальнейшей работы с базами данными"}},
            {"ToolTipOpenExplorer", new List<string>{"A button to select a database on your computer for further work with it", "Кнопка для того чтобы выбрать базу данных на вашем компьютере для дальнейшей работы с ней"}},
            {"ToolTipConnectionTB", new List<string>{"A button to connect the program to your one or many databases", " Кнопка для соединения программы с вашей одной или многими базами данных"}},
        };

        #endregion

        #region internal function
        private List<string> GetDataBasesList()
        {
            List<string> dataBasesList = new List<string>();

            var json = File.ReadAllText("./Data/DataBasesList.json");
            Console.WriteLine(json);
            var wrapper = JsonConvert.DeserializeObject<DataBasesListJson>(json);

            List<string> databases = wrapper.DataBasesList;
            foreach (var database in databases)
            {
                dataBasesList.Add(database);
            }

            return dataBasesList;
        }
        #endregion

        #region builders
        public DDHAuthorization(DDHManager connectionWindow)
        {
            this.ddhManager       = connectionWindow;
            this.customMessageBox = new CustomMessageBoxBuilder();

            InitializeComponent();
            NameDBServerBlock.Text = localizationOfWords["NameDBServerBlock_0"][languageSelected];
            ConnectionTB.Text = localizationOfWords["ConnectionTB"][languageSelected];
            CreationsByTB.Text = localizationOfWords["CreationsByTB"][languageSelected];

            ToolTipNameDBServerBox.Content = localizationOfWords["ToolTipNameDBServerBox"][languageSelected];
            ToolTipOpenExplorer.Content = localizationOfWords["ToolTipOpenExplorer"][languageSelected];
            ToolTipConnectionTB.Content = localizationOfWords["ToolTipConnectionTB"][languageSelected];

            this.customMessageBox.CenterInParentWindow(this);

            DBMSComboBox.Items.Add(localizationOfWords["NotSelected"][languageSelected]);
            foreach (string _ in GetDataBasesList())
            {
                DBMSComboBox.Items.Add(_);
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

        #region functions for combo box
        private void DBMSComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!DBMSComboBox.SelectedItem.ToString().Contains("SQLite"))
            {
                NameDBServerBlock.Text = localizationOfWords["NameDBServerBlock_0"][languageSelected];
                NameDBServerBox.Clear();
                OpenExplorer.Visibility = Visibility.Hidden;
            }
            else
            {
                NameDBServerBlock.Text = localizationOfWords["NameDBServerBlock_1"][languageSelected];
                NameDBServerBox.Clear();
                OpenExplorer.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region functions for connection data base
        private void OpenExplorer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter         = "Database Files (*.db)|*.db";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                this.selectedFilePath   = selectedFilePath;
                int lastSlashIndex      = selectedFilePath.LastIndexOf('\\');
                int lastIndex           = selectedFilePath.Length - 1;
                string result           = selectedFilePath.Substring(lastSlashIndex + 1, lastIndex - lastSlashIndex);
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
                case SQLIteConnector.nameDBManagementSystem:
                    _fileName = NameDBServerBox.Text;
                    if (!string.IsNullOrWhiteSpace(_fileName))
                    {
                        SQLIteConnector test_connection = new SQLIteConnector(selectedFilePath);
                        bool isConnected;

                        isConnected = await test_connection.GetInfoConnection();
                        if (isConnected)
                        {
                            DatabaseConfiguration.nameDbManagementSystem = _currentDBManagementSystem;
                            DatabaseConfiguration.filePathDb = this.selectedFilePath;
                            this.ddhManager.ConnectionSQLite(DatabaseConfiguration.filePathDb, DatabaseConfiguration.nameDbManagementSystem);
                            customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                            this.Close();
                        }
                        else
                        {
                            customMessageBox.ShowError(
                                                        localizationOfWords["Error"][languageSelected],
                                                        localizationOfWords["FailedConnectDB"][languageSelected], 
                                                        localizationOfWords["Close"][languageSelected], 
                                                        this);
                        }
                    }
                    else
                    {
                        customMessageBox.ShowError(
                                                    localizationOfWords["Error"][languageSelected],
                                                    localizationOfWords["NoSelectDBFile"][languageSelected], 
                                                    localizationOfWords["Close"][languageSelected], 
                                                    this);
                    }
                    break;
                case SQLServerConnector.nameDBManagementSystem:

                    _serverName = NameDBServerBox.Text;

                    if (!string.IsNullOrWhiteSpace(_serverName))
                    {
                        customMessageBox.ShowLoading(
                                                        localizationOfWords["Connection"][languageSelected],
                                                        localizationOfWords["Connection"][languageSelected], 
                                                        this);
                        SQLServerConnector test_connection = new SQLServerConnector(_serverName);
                        bool isConnected;

                        isConnected = await test_connection.GetInfoConnection();

                        if (isConnected)
                        {
                            DatabaseConfiguration.nameDbManagementSystem = _currentDBManagementSystem;
                            DatabaseConfiguration.serverName = _serverName;
                            this.ddhManager.ConnectionSQLServer(DatabaseConfiguration.serverName, DatabaseConfiguration.nameDbManagementSystem);
                            customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                            this.Close();
                        }
                        else
                        {
                            customMessageBox.customMessageBox.Visibility = Visibility.Hidden;
                            CustomMessageBoxBuilder.ClosingState = true;
                            customMessageBox.ShowError(
                                                        localizationOfWords["Error"][languageSelected],
                                                        localizationOfWords["FailedConnectDB"][languageSelected],
                                                        localizationOfWords["Close"][languageSelected],
                                                        this);
                        }
                    }
                    else
                    {
                        customMessageBox.ShowError(
                                                localizationOfWords["Error"][languageSelected],
                                                localizationOfWords["SpecifyServerName"][languageSelected],
                                                localizationOfWords["Close"][languageSelected],
                                                this);
                    }
                    break;
                default:
                    //throw new InvalidOperationException($"Неизвестная система управления базами данных: {_currentDBManagementSystem}");
                    customMessageBox.ShowError(
                                                localizationOfWords["Error"][languageSelected], 
                                                localizationOfWords["SelectDBMS"][languageSelected], 
                                                localizationOfWords["Close"][languageSelected], 
                                                this);
                    break;
            }
        }
        #endregion

        #region closing window event handler
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e){customMessageBox.customMessageBox.Close();}
        private void CloseWindow_Click(object sender, RoutedEventArgs e){this.Close();}
        #endregion
    }
}
