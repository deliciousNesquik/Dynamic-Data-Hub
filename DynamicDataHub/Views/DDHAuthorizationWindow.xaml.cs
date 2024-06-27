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
        public DDHAuthorization()
        {
            InitializeComponent();
            
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {

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
