﻿using System;
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

        private Brush foreground = Brushes.White;
        public Window customMessageBox;
        public static bool ClosingState;
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
        public TextBlock CreateTextBlock(string text, Brush foreground = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment verticalAligment = VerticalAlignment.Center, int height = 70, TextWrapping textWrapping = TextWrapping.Wrap,
            double fontSize = 14.0)
        {
            if (foreground == null) { foreground = this.foreground; }
            TextBlock messageTextBlock = new TextBlock()
            {
                Text = text,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAligment,
                FontSize = fontSize,
                Foreground = foreground,
                TextWrapping = textWrapping,
                Height = height,

            };
            return messageTextBlock;
        }
        public void ShowError(string title, string message, string messageButton)
        {
            customMessageBox.Title = title;
            TextBlock messageTextBlock = CreateTextBlock(message);

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


            if (customMessageBox.Visibility == Visibility.Hidden && ClosingState)
            {
                customMessageBox.Visibility = Visibility.Visible;
            }
            else
            {
                customMessageBox.Show();
            }
        }
        #region Loading
        public async void ShowLoading(string title, string message)
        {
            customMessageBox.Title = title;
            TextBlock messageTextBlock = CreateTextBlock(message);
            string Loading = message;
            customMessageBox.Show();
            string Points = ". ";
            while (!ClosingState)
            {
                customMessageBox.Content = messageTextBlock;
                messageTextBlock.Text = Loading + Points;

                if (Points.Length == 6)
                {
                    Points = ". ";
                }
                else
                {
                    Points += ". ";
                }

                await Task.Delay(500);
            }
        }
        #endregion
    }
}

