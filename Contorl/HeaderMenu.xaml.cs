using Accounting.Dialog;
using Accounting.Util;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Accounting.Contorl
{
    /// <summary>
    /// Interaction logic for HeaderMenu.xaml
    /// </summary>
    public partial class HeaderMenu : UserControl
    {
        public HeaderMenu()
        {
            InitializeComponent();
        }


        #region button event
        private void btnGoback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this.Parent)?.GoBack();
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
        }
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

    }
}
