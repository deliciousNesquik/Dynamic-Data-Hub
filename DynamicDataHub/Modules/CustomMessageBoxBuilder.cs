using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicDataHub.Modules
{
    internal class CustomMessageBoxBuilder
    {
        public Window customMessageBox;
        public CustomMessageBoxBuilder()
        {
            customMessageBox = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)),
                Width = 350,
                Height = 150,
            };
            
        }

        public void ShowError(string title, string message, string messageButton)
        {
            customMessageBox.Title = title;
            TextBlock messageTextBlock = new TextBlock()
            {
                Text = message,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14.0,
                Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                TextWrapping = TextWrapping.Wrap,
                Height = 70,

            };
            Button closeButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = messageButton,
                Width = 150,
                Height = 30,
                FontSize = 20,
            };
            closeButton.Click += (s, e) => customMessageBox.Visibility = Visibility.Hidden;

            StackPanel panelCenter = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            panelCenter.Children.Add(messageTextBlock);
            panelCenter.Children.Add(closeButton);

            customMessageBox.Content = panelCenter;


            if (customMessageBox.Visibility == Visibility.Hidden)
            {
                customMessageBox.Visibility = Visibility.Visible;
            }
            else
            {
                customMessageBox.Show();
            }
        }
    }
}
