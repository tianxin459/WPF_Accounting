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
            Positions = new ObservableCollection<string>() { "Forward", "Defense", "Goalie" };

            PlayerItems = new ObservableCollection<Player> {
                    new Player() {Name = "Tom",Position= "Forward"},
                    new Player() {Name = "Dick", Position= "Defense"},
                    new Player() {Name = "Harry", Position= "Goalie"}
            };
            Players = new ObservableCollection<Player>(){
                    new Player() {Name = "Tom",Position= "Forward"},
                    new Player() {Name = "Dick", Position= "Defense"},
                    new Player() {Name = "Harry", Position= "Goalie"}
      };
            InitializeComponent();
            ComboBoxColumn.ItemsSource = Positions;
            ComboBoxColumn2.ItemsSource = PlayerItems;
            dataGrid1.ItemsSource = Players;
            //dataGrid1.DataContext = Players;
        }
    }


    public class Player
    {
        public string Name { set; get; }
        public string Position { set; get; }
    }
}
