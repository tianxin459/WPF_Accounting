using Accounting.Dialog;
using Accounting.Model;
using Accounting.Util;
using MaterialDesignThemes.Wpf;
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

        public Member Member
        {
            get
            {
                return App.SelectedMember;
            }
            set
            {
                App.SelectedMember = value;
            }
        }

        public List<ComboItem> ItemsSubordinate { get; set; } = new List<ComboItem>();
        //public List<ComboItem> ItemsSubordinate { get; set; } = new List<ComboItem>();



        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //refresh name based on id

            this.Member.Subordinate = this.Member.Subordinate.GroupBy(x => x.ID).Select(x => x.First()).ToList();
            this.Member
                .Subordinate
                .ForEach(x => x.Name = this.Members
                    .Where(y => y.ID == x.ID)
                    .Select(n => n.Name)
                .FirstOrDefault());

            //remove invalid item (null name)
            for (var i = this.Member.Subordinate.Count() - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(this.Member.Subordinate[i].Name))
                {
                    this.Member.Subordinate.RemoveAt(i);
                }
            }

            this.Member.CalcuateBonusInMemberTree(this.Members);
            this.ItemsSubordinate = this.Members
                    .Where(x => x.Subordinate.Count <= 2 && (x.Supervisor == null || x.Supervisor.ID == x.ID) && x.ID != this.Member.ID)
                    .Select(x => new ComboItem() { Name = x.Name, ID = x.ID }).ToList();

            BuildSubordinateCombo();
            buildControlBinding();
        }

        public Page_MemberDetail(Member member = null)
        {
            this.Member = member ?? App.SelectedMember;
            InitializeComponent();



        }

        private void buildControlBinding()
        {
            this.comboGender.ItemsSource = Enum.GetValues(typeof(Gender));
            this.comboSupervisor.IsReadOnly = true;

            List<ComboItem> itemsSupervisor = this.Members
                .Select(x => new ComboItem() { ID = x.ID, Name = x.Name })
                .ToList();

            this.comboSupervisor.ItemsSource = itemsSupervisor;
            comboSupervisor.DisplayMemberPath = "Name";
            comboSupervisor.SelectedValuePath = "ID";


            this.txtIDNumber.SetBinding(TextBox.TextProperty, new Binding("IDNumber") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtName.SetBinding(TextBox.TextProperty, new Binding("Name") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.comboGender.SetBinding(ComboBox.SelectedValueProperty, new Binding("Gender") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.comboSupervisor.SetBinding(ComboBox.SelectedValueProperty, new Binding("Supervisor.ID") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtAge.SetBinding(TextBox.TextProperty, new Binding("Age") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtPhone.SetBinding(TextBox.TextProperty, new Binding("Phone") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtBonus.SetBinding(TextBox.TextProperty, new Binding("Bonus") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtMemberShipFee.SetBinding(TextBox.TextProperty, new Binding("Fee") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtBank.SetBinding(TextBox.TextProperty, new Binding("Bank") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtAccount.SetBinding(TextBox.TextProperty, new Binding("Account") { Source = this.Member, Mode = BindingMode.TwoWay });

        }

        private void AddSubordinateCombo(int i)
        {
            var comboSub1 = new ComboBox();
            comboSub1.MinWidth = 100;
            comboSub1.Margin = new Thickness(10);
            comboSub1.Name = "comboSub" + i;
            comboSub1.HorizontalAlignment = HorizontalAlignment.Stretch;
            comboSub1.ItemsSource = this.ItemsSubordinate;
            comboSub1.DisplayMemberPath = "Name";
            comboSub1.SelectedValuePath = "ID";
            comboSub1.SetBinding(ComboBox.SelectedValueProperty, new Binding("ID") { Source = this.Member.Subordinate[i], Mode = BindingMode.TwoWay });
            comboSub1.SelectionChanged += ComboSub1_SelectionChanged;
            comboSub1.IsEditable = true;
            comboSub1.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
            HintAssist.SetHint(comboSub1, "关联下属");


            var textblock = new TextBlock();
            textblock.Name = "tbSub" + i;
            textblock.Margin = new Thickness(10);
            textblock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblock.MinWidth = 200;
            textblock.Text = this.Members
                .Where(x => x.ID == this.Member.Subordinate[i].ID)
                .Select(x => $"{x.Phone} {x.IDNumber} {x.Fee}")
                .FirstOrDefault();

            var btnDelete = new Button();
            btnDelete.Content = "delete";
            btnDelete.Name = "delbtn_" + i;
            btnDelete.Margin = new Thickness(10);
            btnDelete.Click += DeleteSubButton_Click;
            btnDelete.Style = this.FindResource("MaterialDesignFloatingActionMiniButton") as Style;
            btnDelete.Content = new PackIcon() { Kind = PackIconKind.Close };

            var panel1 = new StackPanel();
            panel1.Orientation = Orientation.Horizontal;

            panel1.Children.Add(comboSub1);
            panel1.Children.Add(textblock);
            panel1.Children.Add(btnDelete);

            panel1.HorizontalAlignment = HorizontalAlignment.Stretch;

            this.panelSubordinate.Children.Add(panel1);
        }

        private void BuildSubordinateCombo()
        {
            for (int i = 0; i < this.Member.Subordinate.Count; i++)
            {
                var comboSub1 = new ComboBox();
                comboSub1.MinWidth = 100;
                comboSub1.Margin = new Thickness(10);
                comboSub1.Name = "comboSub" + i;
                comboSub1.HorizontalAlignment = HorizontalAlignment.Stretch;
                comboSub1.ItemsSource = this.ItemsSubordinate;
                comboSub1.DisplayMemberPath = "Name";
                comboSub1.SelectedValuePath = "ID";
                comboSub1.SetBinding(ComboBox.SelectedValueProperty, new Binding("ID") { Source = this.Member.Subordinate[i], Mode = BindingMode.TwoWay });
                comboSub1.SelectionChanged += ComboSub1_SelectionChanged;
                comboSub1.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
                comboSub1.IsEditable = true;
                HintAssist.SetHint(comboSub1, "关联下属");

                var textblock = new TextBlock();
                textblock.Name = "tbSub" + i;
                textblock.Margin = new Thickness(10);
                textblock.HorizontalAlignment = HorizontalAlignment.Stretch;
                textblock.MinWidth = 200;
                textblock.Text = this.Members
                    .Where(x => x.ID == this.Member.Subordinate[i].ID)
                    .Select(x => $"{x.Phone} {x.IDNumber} {x.Fee}")
                    .FirstOrDefault();

                var btnDelete = new Button();
                btnDelete.Content = "delete";
                btnDelete.Name = "delbtn_" + i;
                btnDelete.Margin = new Thickness(10);
                btnDelete.Click += DeleteSubButton_Click;
                btnDelete.Style = this.FindResource("MaterialDesignFloatingActionMiniButton") as Style;
                btnDelete.Content = new PackIcon() { Kind = PackIconKind.Close };

                var panel1 = new StackPanel();
                panel1.Orientation = Orientation.Horizontal;

                panel1.Children.Add(comboSub1);
                panel1.Children.Add(textblock);
                panel1.Children.Add(btnDelete);
                //panel1.Children.Add(btnDelete2);

                panel1.HorizontalAlignment = HorizontalAlignment.Stretch;

                this.panelSubordinate.Children.Add(panel1);
            }
        }

        private void BtnDelete2_MouseUp1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var p = (sender as Button).Parent as StackPanel;

                var i = (sender as Button).Name.Replace("delbtn_i", string.Empty);
                p.Children.Clear();
                this.Member.Subordinate.RemoveAt(int.Parse(i));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteSubButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var p = (sender as Button).Parent as StackPanel;

                var strI = (sender as Button).Name.Replace("delbtn_", string.Empty);
                var i = int.Parse(strI);
                var removeMember = this.Member.Subordinate[i];
                this.Member.Subordinate.RemoveAt(i);
                var relatedChildren = this.Members.Where(x => x.ID ==removeMember.ID && x.Supervisor?.ID == this.Member.ID).ToList();
                relatedChildren.ForEach(x => x.Supervisor = null);
                p.Children.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ComboSub1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectID = (sender as ComboBox).SelectedValue;
            var parent = (sender as ComboBox).Parent as StackPanel;
            (parent.Children[1] as TextBlock).Text = this.Members
                    .Where(x => x.ID == selectID.ToString())
                    .Select(x => $"{x.Phone} {x.IDNumber} {x.Fee}")
                    .FirstOrDefault();
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Member.Subordinate
                    .ForEach(x => x.Name = this.Members.Where(m1 => m1.ID == x.ID).FirstOrDefault().Name);
                //var m = this.Member;
                if (this.Members.Exists(x => x.ID == this.Member.ID))
                {
                    var i = this.Members.FindIndex(x => x.ID == this.Member.ID);
                    this.Members[i] = this.Member;
                }
                else
                {
                    this.Members.Add(this.Member);
                }
                DataStorage.SaveData(this.Members);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var dialogSuccess = new DialogSuccess("保存成功");
            dialogSuccess.ComfirmButton.Click += DialogComfirmButton_Click;
            DialogHost.Show(dialogSuccess);
            //this.NavigationService.GoBack();

        }

        private void DialogComfirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void comboSupervisor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var supID = (sender as ComboBox).SelectedValue.ToString();

            //add supervisor
            this.Member.Supervisor = this.Members
                .Where(x => x.ID == supID)
                .Select(o => new RefMember() { ID = o.ID, Name = o.Name })
                .FirstOrDefault();

            //add subordinate for supervisor note
            this.Members.Where(x => x.ID == supID)
                .FirstOrDefault()
                .Subordinate
                .Add(new RefMember() { ID = this.Member.ID, Name = this.Member.Name });
        }


        private void AddSubButton_Click_1(object sender, RoutedEventArgs e)
        {
            var panel = this.FindName("panelSubordinate") as StackPanel;
            this.Member.Subordinate.Add(new RefMember());
            AddSubordinateCombo(this.Member.Subordinate.Count - 1);
        }
        private void AddNewSubButton_Click_1(object sender, RoutedEventArgs e)
        {
            var mid = Member.GenerateID();
            var newMember = new Member(mid)
            {
                Supervisor = new RefMember()
                {
                    ID = this.Member.ID,
                    Name = this.Member.Name
                }
            };
            this.Member.Subordinate.Add(new RefMember() { ID = mid });
            App.SelectedMemberStack.Push(this.Member);
            App.SelectedMember = newMember;

            this.NavigationService.Navigate(new Page_MemberDetail(newMember));
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BasePage_Unloaded(object sender, RoutedEventArgs e)
        {
            //if(this.SelectedMemberStack.Count()>0)
            //{
            //    this.SelectedMember = this.SelectedMemberStack.Pop();
            //}
        }


        //private void SubNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as ComboBox;
        //    var Items = this.dgSubordinate.ItemsSource as List<Member>;
        //    var selectItem = this.dgSubordinate.SelectedItem as Member;
        //    if(comboBox.SelectedIndex != -1)
        //    {
        //        var m = comboBox.SelectedItem as Member;
        //        this.txtAccount.Text = m.Name +"|"+selectItem?.Name;
        //        selectItem.Name = m.Name;
        //    }

        //}
    }
}
