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
        public readonly Window CustomMessageBox = new Window()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.None,
            Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)),
            Width = 350,
            Height = 150,
        };

        public void ShowError(string title, string message, string messageButton)
        {
            CustomMessageBox.Title = title;
            var messageTextBlock = new TextBlock()
            {
                Text = $"Ошибка: \n{message}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14.0,
                Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                TextWrapping = TextWrapping.Wrap,
                Height = 70,

            };
            var closeButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Content = messageButton,
                Width = 120,
                Height = 25,
                FontSize = 14,
            };
            closeButton.Click += (s, e) => CustomMessageBox.Visibility = Visibility.Hidden;

            var panelCenter = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            panelCenter.Children.Add(messageTextBlock);
            panelCenter.Children.Add(closeButton);

            CustomMessageBox.Content = panelCenter;


            if (CustomMessageBox.Visibility == Visibility.Hidden)
            {
                CustomMessageBox.Visibility = Visibility.Visible;
            }
            else
            {
                CustomMessageBox.Show();
            }
        }
    }
}
