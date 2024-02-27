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
using System.Windows.Shapes;

namespace Aviator.Windows
{
    /// <summary>
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        #region Commands and Clicks

        private void Apply_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
