using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public string Str { get; set; } = "222";
        public Window2()
        {
            InitializeComponent();
            txtSearch.DataContext = Str;
            this.txtSearch.SetBinding(TextBox.TextProperty, new Binding("Str") { Source = this, Mode = BindingMode.TwoWay });

            //HintAssist.SetHint(this.comboSearch, "ssss");

            //this.comboSearch.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.cardFliper.IsFlipped = !this.cardFliper.IsFlipped;
        }
    }


    public class Player
    {
        public string Name { set; get; }
        public string Position { set; get; }
    }
}
