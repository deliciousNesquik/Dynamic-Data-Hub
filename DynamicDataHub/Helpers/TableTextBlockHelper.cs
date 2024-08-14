using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace DynamicDataHub.Helpers
{
    public static class TableTextBlockHelper
    {
        public static void SetDefaultView(this TextBlock textBlock)
        {
            textBlock.Text = "not selected";
            textBlock.Foreground = new SolidColorBrush(Colors.DimGray);
        }
        public static void SetTable(this TextBlock textBlock, string table)
        {
            textBlock.Text = table;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.TextDecorations = TextDecorations.Underline;
        }
    }
}
