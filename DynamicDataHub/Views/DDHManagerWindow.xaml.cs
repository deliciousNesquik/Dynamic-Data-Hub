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
    public partial class DDHManager : Window{

        public DDHManager()
        {
            InitializeComponent();
            var ConnectionWindow = new DDHAuthorization();
            ConnectionWindow.Show();
        }
        public DDHManager(string ServerName, string DBName){
            InitializeComponent();
            MessageBox.Show("NOT SQLITE");
        }
        public DDHManager(string DBFileName){
            InitializeComponent();
            MessageBox.Show("SQLITE");
        }

        public static DDHManager Create(string DBFileName)
        {
            return new DDHManager(DBFileName);
        }

        public static DDHManager Create(string ServerName, string DBName)
        {
            return new DDHManager(ServerName, DBName);
        }
    }
}
