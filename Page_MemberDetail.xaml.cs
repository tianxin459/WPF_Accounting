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

        public List<Member> ItemsSubordinate { get; set; } = new List<Member>();
        //public List<ComboItem> ItemsSubordinate { get; set; } = new List<ComboItem>();


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var es = (sender as Button).Content;
        }

        public Page_MemberDetail()
        {
            InitializeComponent();


            this.comboGender.ItemsSource = Enum.GetValues(typeof(Gender));
            this.comboSupervisor.IsReadOnly = true;

            List<MemberNote> itemsSupervisor = this.Members
                .Select(x => new MemberNote() { ID = x.ID, Name = x.Name })
                .ToList();


            //ItemsSubordinate = this.Members
            //    .Select(x => new ComboItem() { ID = x.ID, Name = x.Name })
            //    .ToList();

            //ItemsSubordinate.Clear();
            //ItemsSubordinate.Add(new ComboItem() { ID = "1", Name = "aaaaaaaa" });


            this.comboSupervisor.ItemsSource = itemsSupervisor;
            comboSupervisor.DisplayMemberPath = "Name";
            comboSupervisor.SelectedValuePath = "ID";

            var btn = new Button() { Content = "Click" };
            btn.Click += new RoutedEventHandler(btn_Click);

            var btn2 = new Button() { Content = "Click2" };
            btn2.Click += new RoutedEventHandler(btn_Click);

            this.panelAccount.Children.Add(btn);
            this.panelAccount.Children.Add(btn2);
            //comboGender.DisplayMemberPath = "Value";
            //comboGender.SelectedValuePath = "Key";


            this.txtIDNumber.SetBinding(TextBox.TextProperty, new Binding("IDNumber") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtName.SetBinding(TextBox.TextProperty, new Binding("Name") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.comboGender.SetBinding(ComboBox.SelectedValueProperty, new Binding("Gender") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.comboSupervisor.SetBinding(ComboBox.SelectedValueProperty, new Binding("Supervisor.ID") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtAge.SetBinding(TextBox.TextProperty, new Binding("Age") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtPhone.SetBinding(TextBox.TextProperty, new Binding("Phone") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtBonus.SetBinding(TextBox.TextProperty, new Binding("Bonus") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtMemberShipFee.SetBinding(TextBox.TextProperty, new Binding("Fee") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtBank.SetBinding(TextBox.TextProperty, new Binding("Bank") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });
            this.txtAccount.SetBinding(TextBox.TextProperty, new Binding("Account") { Source = this.SelectedMember, Mode = BindingMode.TwoWay });

            var ItemsSubordinate = this.Members.Where(x => this.SelectedMember.Subordinate.Exists(y => y.ID == x.ID)).ToList<Member>();
            //var itemsSub = this.Members.Where(x => !this.SelectedMember.Subordinate.Exists(y => y.ID == x.ID)).ToList<Member>();
            var sub = this.Members.ToList<Member>();
            dgSubordinate.ItemsSource = ItemsSubordinate;
            this.comboSub.ItemsSource = sub;
            //dgSubordinate.DataContext = sub;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var m = this.SelectedMember;
        }

        private void SubNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var Items = this.dgSubordinate.ItemsSource as List<Member>;
            var selectItem = this.dgSubordinate.SelectedItem as Member;
            if(comboBox.SelectedIndex != -1)
            {
                var m = comboBox.SelectedItem as Member;
                this.txtAccount.Text = m.Name +"|"+selectItem?.Name;
                selectItem.Name = m.Name;
            }

        }
    }
}
