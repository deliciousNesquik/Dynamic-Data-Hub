using DynamicDataHub.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicDataHub
{
    public partial class DDHManager : Window
    {

        public static Window ConnectionWindow = new DDHAuthorization();

        public DDHManager()
        {
            InitializeComponent();
        }
        
        public DDHManager(string ServerName, string DBName)
        {
            InitializeComponent();
        }
        public DDHManager(string DBFileName)
        {
            InitializeComponent();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConnectionWindow.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.Show();
        }
    }
}
