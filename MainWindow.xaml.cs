using Accounting.Module;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Accounting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<Member> initData()
        {
            var members = new ObservableCollection<Member>();

            for (int i = 0; i < 10; i++)
            {
                members.Add(new Member()
                {
                    ID = i.ToString(),
                    Name = "Name" + i,
                    Account = "xxxx-xxxx-xxxx-xxxx".Replace("x", i.ToString()),
                    Age = (i * 9).ToString(),
                    Phone = "xxxxxxxxxxxxxxxx".Replace("x", i.ToString()),
                    Fee = i,
                    Bonus = i
                });
            }
            members[2].Supervisor = members[1].Ref;
            members[2].Subordinate.Add(members[3].Ref);
            members[2].Subordinate.Add(members[4].Ref);
            return members;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var members = initData();
            this.dgStaff.DataContext = members;
        }
    }
}
