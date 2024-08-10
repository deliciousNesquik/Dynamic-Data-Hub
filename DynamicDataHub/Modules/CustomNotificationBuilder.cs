using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DynamicDataHub.Modules
{
    internal class CustomNotificationBuilder
    {
        public static StackPanel panel { get; private set; }
        public static TextBlock textBlock { get; private set; }

        public static void CreateNotification(Grid mainGrid)
        {
            // Создание StackPanel
            StackPanel infoMessageStackPanel = new StackPanel
            {
                Opacity = 0,
                Name = "InfoMessageStackPanel",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 20, 20)
            };

            // Создание Border
            Border border = new Border
            {
                Width = 250,
                Height = 40,
                //CornerRadius = new CornerRadius(10),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1)
            };

            // Создание Grid и Rectangle
            Grid grid = new Grid();
            Rectangle rect = new Rectangle
            {
                RadiusX = 10,
                RadiusY = 10,
                Visibility = Visibility.Hidden
            };
            grid.Clip = rect.RenderedGeometry;

            // Создание внутреннего Grid и TextBlock
            Grid innerGrid = new Grid();
            TextBlock infoMessageTextBlock = new TextBlock
            {
                Name = "InfoMessageTextBlock",
                Text = "Успешное выполнение ...",
                Foreground = Brushes.White,
                Margin = new Thickness(10)
            };

            // Добавление элементов во внутренний Grid
            innerGrid.Children.Add(infoMessageTextBlock);

            // Добавление внутреннего Grid во внешний Grid
            //grid.Children.Add(innerGrid);

            // Добавление Grid в Border
            //border.Child = grid;
            border.Child = innerGrid;

            // Добавление Border в StackPanel
            
            infoMessageStackPanel.Children.Add(border);
            //infoMessageStackPanel.Children.Add(infoMessageTextBlock);

            // Добавление StackPanel в основной контейнер (например, Grid)
            // Предполагается, что у вас есть Grid с именем "MainGrid"
            mainGrid.Children.Add(infoMessageStackPanel);
            Grid.SetRow(infoMessageStackPanel, 2); // Установка строки, если ваш Grid имеет RowDefinitions
            Grid.SetColumn(infoMessageStackPanel, 1); // Установка столбца, если ваш Grid имеет ColumnDefinitions

            panel = infoMessageStackPanel;
            textBlock = infoMessageTextBlock;
        }

        public static void ShowNotificationOpacity(string notificationMessage)
        {
            double beginState = 0;
            double endState = 1;
            double animationDuration = 3;

            DoubleAnimation notificationMessageAnim = new DoubleAnimation();
            notificationMessageAnim.From = beginState;
            notificationMessageAnim.To = endState;
            notificationMessageAnim.Duration = TimeSpan.FromSeconds(animationDuration);

            textBlock.Text = notificationMessage;
            panel.BeginAnimation(StackPanel.OpacityProperty, notificationMessageAnim);
            Task.Delay(500);

            notificationMessageAnim.From = endState;
            notificationMessageAnim.To = beginState;
            notificationMessageAnim.Duration = TimeSpan.FromSeconds(animationDuration);
            panel.BeginAnimation(StackPanel.OpacityProperty, notificationMessageAnim);
        }
    }
}
