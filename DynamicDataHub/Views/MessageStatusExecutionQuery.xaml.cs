using DynamicDataHub.Modules;
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

namespace DynamicDataHub.Views
{
    /// <summary>
    /// Логика взаимодействия для MessageStatusExecutionQuery.xaml
    /// </summary>
    public partial class MessageStatusExecutionQuery : UserControl
    {
        public MessageStatusExecutionQuery()
        {
            InitializeComponent();
            MessageStatusExecuteTextBlock.Text = $"затронуто строк : {DatabaseConfiguration.countRowsAffected}";
            QueryExecutionTimeTextBlock.Text = $"Время выполнения запроса: {DateTime.Now.ToString()}";
        }
    }
}
