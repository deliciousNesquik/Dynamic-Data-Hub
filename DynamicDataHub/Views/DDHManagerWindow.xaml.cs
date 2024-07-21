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

        private void WrapColumn_Click(object sender, RoutedEventArgs e)
        {
            RotateTransform rotateTransform = new RotateTransform(0);
            var WrapColumnTransform = WrapColumn.RenderTransform as RotateTransform;


            if (WrapColumnTransform?.Angle == 0)
            {
                TBObjectExplorer.Visibility = Visibility.Hidden;
                Connect.Visibility = Visibility.Hidden;
                Disconnect.Visibility = Visibility.Hidden;
                Refresh.Visibility = Visibility.Hidden;
                WrapColumn.Margin = new Thickness(0);
                SPWrap.HorizontalAlignment = HorizontalAlignment.Left;

                rotateTransform = new RotateTransform(180);
                WrapColumn.RenderTransform = rotateTransform;



                //ObjectExplorer.Width = new GridLength(0.03, GridUnitType.Star);
            }
            else
            {
                TBObjectExplorer.Visibility = Visibility.Visible;
                Connect.Visibility = Visibility.Visible;
                Disconnect.Visibility = Visibility.Visible;
                Refresh.Visibility = Visibility.Visible;
                WrapColumn.Margin = new Thickness(87, 5, 0, 5);
                //SPWrap.HorizontalAlignment = HorizontalAlignment.Left;

                rotateTransform = new RotateTransform(0);
                WrapColumn.RenderTransform = rotateTransform;



                //ObjectExplorer.Width = new GridLength(0.03, GridUnitType.Star);
            }

        }
    }
}
