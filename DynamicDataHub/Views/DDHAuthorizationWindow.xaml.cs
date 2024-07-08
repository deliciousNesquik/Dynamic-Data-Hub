using DynamicDataHub.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DynamicDataHub.Views
{
    public partial class DDHAuthorization : Window
    {
        public List<string> _test = new List<string>();
        public DDHAuthorization()
        {
            InitializeComponent();
            ChoosingDBManagementSystem Test = new ChoosingDBManagementSystem();
            _test = Test.GetDBManagementSystems();
            foreach (string test in _test)
            {
                DBMSComboBox.Items.Add(test);
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {

        }

        private void DBMSComboBox_DropDownOpened(object sender, EventArgs e)
        {
            
            if (_test.Count == 0)
            {
                MessageBox.Show("У вас отсутствуют соответствующие СУБД");
            }
        }

        private void DBMSComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!DBMSComboBox.SelectedItem.ToString().Contains("SQLite")){
                NameBDBlock.Visibility = Visibility.Visible;
                NameBDBox.Visibility = Visibility.Visible;
                NameBDServerBlock.Text = "Имя сервера";
                NameBDServerBox.Clear();
                OpenExplorer.Visibility = Visibility.Hidden;
            }
            else{
                NameBDBlock.Visibility = Visibility.Hidden;
                NameBDBox.Visibility = Visibility.Hidden;
                NameBDServerBlock.Text = "Файл базы данных";
                OpenExplorer.Visibility = Visibility.Visible;
            }
        }

        private void OpenExplorer_Click(object sender, RoutedEventArgs e){
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Database Files (*.db)|*.db";

            if (openFileDialog.ShowDialog() == true){
                string selectedFilePath = openFileDialog.FileName;
                int lastSlashIndex = selectedFilePath.LastIndexOf('\\');
                int lastIndex = selectedFilePath.Length - 1;
                string result = selectedFilePath.Substring(lastSlashIndex + 1, lastIndex - lastSlashIndex);
                NameBDServerBox.Text = result;
            }
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
