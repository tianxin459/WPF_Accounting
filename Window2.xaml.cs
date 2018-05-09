using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Accounting
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public ObservableCollection<Player> Players { get; set; }
        public ObservableCollection<Player> PlayerItems { get; set; }
        public ObservableCollection<string> Positions { get; set; }
        public Window2()
        {
            InitializeComponent();
            HintAssist.SetHint(this.comboSearch, "ssss");

            this.comboSearch.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }


    public class Player
    {
        public string Name { set; get; }
        public string Position { set; get; }
    }
}
