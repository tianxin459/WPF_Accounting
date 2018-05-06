using Accounting.Model;
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

namespace Accounting
{
    /// <summary>
    /// Interaction logic for Page_MemberDetail.xaml
    /// </summary>
    public partial class Page_MemberDetail : BasePage
    {
        public class ComboItem
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }

        public List<ComboItem> ItemsSubordinate { get; set; } = new List<ComboItem>();

        public Page_MemberDetail()
        {
            InitializeComponent();
            this.comboGender.ItemsSource = Enum.GetValues(typeof(Gender));
            this.comboSupervisor.IsReadOnly = true;

            List<MemberNote> itemsSupervisor = this.Members
                .Select(x => new MemberNote() { ID = x.ID, Name = x.Name })
                .ToList();


            ItemsSubordinate = this.Members
                .Select(x => new ComboItem() { ID = x.ID, Name = x.Name })
                .ToList();

            ItemsSubordinate.Clear();
            ItemsSubordinate.Add(new ComboItem() { ID = "1", Name = "aaaaaaaa" });


            this.comboSupervisor.ItemsSource = itemsSupervisor;
            comboSupervisor.DisplayMemberPath = "Name";
            comboSupervisor.SelectedValuePath = "ID";
            //comboGender.DisplayMemberPath = "Value";
            //comboGender.SelectedValuePath = "Key";


            this.txtID.SetBinding(TextBox.TextProperty, new Binding("ID") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtName.SetBinding(TextBox.TextProperty, new Binding("Name") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.comboGender.SetBinding(ComboBox.SelectedValueProperty, new Binding("Gender") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.comboSupervisor.SetBinding(ComboBox.SelectedValueProperty, new Binding("Supervisor.ID") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtAge.SetBinding(TextBox.TextProperty, new Binding("Age") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtPhone.SetBinding(TextBox.TextProperty, new Binding("Phone") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtBonus.SetBinding(TextBox.TextProperty, new Binding("Bonus") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtMemberShipFee.SetBinding(TextBox.TextProperty, new Binding("Fee") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtBank.SetBinding(TextBox.TextProperty, new Binding("Bank") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtAccount.SetBinding(TextBox.TextProperty, new Binding("Account") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });

            var sub = this.Members.Where(x => this.SelectedMember.Subordinate.Exists(y => y.ID == x.ID)).ToList<Member>();

            dgSubordinate.ItemsSource = ItemsSubordinate;
            dgSubordinate.DataContext = sub;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("pack://application:,,,/Page_Main.xaml"));
        }
    }
}
