using DynamicDataHub.Modules;
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
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {

        }

        private void DBMSComboBox_DropDownOpened(object sender, EventArgs e)
        {
            DBMSComboBox.Items.Clear();
            if (_test.Count == 0)
            {
                MessageBox.Show("У вас отсутствуют соответствующие СУБД");
            }
            else
            {
                foreach (string test in _test)
                {
                    DBMSComboBox.Items.Add(test);
                }
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
