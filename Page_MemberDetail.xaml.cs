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

            public ComboItem() { }
            public ComboItem(string id,string name)
            {
                ID = id;
                Name = name;
            }
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
                .Where(x=>x.Subordinate.Count()<2)
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
            this.txtBonus.SetBinding(TextBlock.TextProperty, new Binding("Bonus") { Source = this.Member, Mode = BindingMode.OneWay });
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
            var currentItem = this.Member.Subordinate[i];
            comboSub1.ItemsSource = this.ItemsSubordinate.Concat(new[] { new ComboItem(currentItem.ID, currentItem.Name) });
            comboSub1.DisplayMemberPath = "Name";
            comboSub1.SelectedValuePath = "ID";
            comboSub1.SetBinding(ComboBox.SelectedValueProperty, new Binding("ID") { Source = this.Member.Subordinate[i], Mode = BindingMode.OneWay });
            comboSub1.SelectionChanged += ComboSub1_SelectionChanged;
            comboSub1.IsEditable = true;
            comboSub1.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
            HintAssist.SetHint(comboSub1, "关联下属");


            var textMember = this.Members
                .Where(x => x.ID == this.Member.Subordinate[i].ID)
                .FirstOrDefault();


            var textblock = new TextBlock();
            textblock.Name = "tbSub" + i;
            textblock.Margin = new Thickness(10);
            textblock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblock.MinWidth = 200;
            textblock.Text = $"{textMember.Phone} {textMember.IDNumber} {textMember.Fee}";
            textblock.Visibility = Visibility.Hidden;

            var textblockPhone = new TextBox();
            textblockPhone.Name = "tbPhone" + i.ToString();
            this.RegisterName(textblockPhone.Name, textblockPhone);
            textblockPhone.Margin = new Thickness(10);
            textblockPhone.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockPhone.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
            textblockPhone.Text = textMember.Phone;
            HintAssist.SetHint(textblockPhone, "电话号码");
            textblockPhone.IsEnabled = false;

            var textblockIDNumber = new TextBox();
            textblockIDNumber.Name = "tbIDNumber" + i.ToString();
            this.RegisterName(textblockIDNumber.Name, textblockIDNumber);
            textblockIDNumber.Margin = new Thickness(10);
            textblockIDNumber.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockIDNumber.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
            textblockIDNumber.Text = textMember.IDNumber;
            HintAssist.SetHint(textblockIDNumber, "身份证号");
            textblockIDNumber.IsEnabled = false;


            var textblockFee = new TextBox();
            textblockFee.Name = "tbFee" + i.ToString();
            this.RegisterName(textblockFee.Name, textblockFee);
            textblockFee.Margin = new Thickness(10);
            textblockFee.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockFee.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
            textblockFee.Text = textMember.Fee.ToString();
            HintAssist.SetHint(textblockFee, "缴纳费用");
            textblockFee.IsEnabled = false;

            var btnDelete = new Button();
            btnDelete.Content = "delete";
            btnDelete.Name = "delbtn_" + i;
            btnDelete.Margin = new Thickness(10);
            btnDelete.Click += DeleteSubButton_Click;
            btnDelete.VerticalAlignment = VerticalAlignment.Center;
            btnDelete.Height = 20;
            btnDelete.Width = 20;
            btnDelete.Style = this.FindResource("MaterialDesignFloatingActionMiniButton") as Style;
            btnDelete.Content = new PackIcon() { Kind = PackIconKind.Close };

            var panel1 = new StackPanel();
            panel1.Name = "panelSub_"+i;
            panel1.Orientation = Orientation.Horizontal;

            panel1.Children.Add(comboSub1);
            //panel1.Children.Add(textblock);
            panel1.Children.Add(textblockPhone);
            panel1.Children.Add(textblockIDNumber);
            panel1.Children.Add(textblockFee);
            panel1.Children.Add(btnDelete);

            panel1.HorizontalAlignment = HorizontalAlignment.Stretch;

            this.panelSubordinate.Children.Add(panel1);
        }

        private void BuildSubordinateCombo()
        {
            for (int i = 0; i < this.Member.Subordinate.Count; i++)
            {
                AddSubordinateCombo(i);
                //var comboSub1 = new ComboBox();
                //comboSub1.MinWidth = 100;
                //comboSub1.Margin = new Thickness(10);
                //comboSub1.Name = "comboSub" + i;
                //comboSub1.HorizontalAlignment = HorizontalAlignment.Stretch;
                //comboSub1.ItemsSource = this.ItemsSubordinate;
                //comboSub1.DisplayMemberPath = "Name";
                //comboSub1.SelectedValuePath = "ID";
                //comboSub1.SetBinding(ComboBox.SelectedValueProperty, new Binding("ID") { Source = this.Member.Subordinate[i], Mode = BindingMode.TwoWay });
                //comboSub1.SelectionChanged += ComboSub1_SelectionChanged;
                //comboSub1.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
                //comboSub1.IsEditable = true;
                //HintAssist.SetHint(comboSub1, "关联下属");

                //var textblock = new TextBlock();
                //textblock.Name = "tbSub" + i;
                //textblock.Margin = new Thickness(10);
                //textblock.HorizontalAlignment = HorizontalAlignment.Stretch;
                //textblock.MinWidth = 200;
                //textblock.Text = this.Members
                //    .Where(x => x.ID == this.Member.Subordinate[i].ID)
                //    .Select(x => $"{x.Phone} {x.IDNumber} {x.Fee}")
                //    .FirstOrDefault();

                //var btnDelete = new Button();
                //btnDelete.Content = "delete";
                //btnDelete.Name = "delbtn_" + i;
                //btnDelete.Margin = new Thickness(10);
                //btnDelete.Click += DeleteSubButton_Click;
                //btnDelete.Style = this.FindResource("MaterialDesignFloatingActionMiniButton") as Style;
                //btnDelete.Content = new PackIcon() { Kind = PackIconKind.Close };

                //var panel1 = new StackPanel();
                //panel1.Orientation = Orientation.Horizontal;

                //panel1.Children.Add(comboSub1);
                //panel1.Children.Add(textblock);
                //panel1.Children.Add(btnDelete);
                ////panel1.Children.Add(btnDelete2);

                //panel1.HorizontalAlignment = HorizontalAlignment.Stretch;

                //this.panelSubordinate.Children.Add(panel1);
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
                var relatedChildren = this.Members.Where(x => x.ID ==removeMember.ID && x.Supervisor?.ID == this.Member.ID).ToList();
                relatedChildren.ForEach(x => x.Supervisor = null);
                this.Member.Subordinate.RemoveAt(i);
                this.Member.CalcuateBonusInMemberTree(this.Members);
                this.txtBonus.Text = this.Member.Bonus.ToString();
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
            //set supervisor for selected child
            this.Members.Where(x => x.ID == selectID.ToString())
                .FirstOrDefault().Supervisor = new RefMember() { ID = this.Member.ID, Name = this.Member.Name };

            //var seqI = (sender as ComboBox).Name.Replace("comboSub", string.Empty);
            //var oldID = this.Member.Subordinate[int.Parse(seqI)].ID;
            //var newID = selectID.ToString();
            //clear supervisor for old id
            //this.Members
            //    .Where(x => x.ID == oldID)
            //    .ToList()
            //    .ForEach(x => x.Supervisor = null);

            #region label display
            var subMember = this.Members
                    .Where(x => x.ID == selectID.ToString())
                    .FirstOrDefault();

            //(parent.Children[1] as TextBlock).Text = $"{subMember.Phone} {subMember.IDNumber} {subMember.Fee}";

            var i = parent.Name.Replace("panelSub_", string.Empty);

            var tbPhone = parent.FindName("tbPhone" + i);
            if(tbPhone!=null)
                (tbPhone as TextBox).Text = subMember.Phone;
            
            var tbIDNumber= parent.FindName("tbIDNumber" + i);
            if (tbIDNumber != null)
                (tbIDNumber as TextBox).Text = subMember.IDNumber;

            var tbFee = parent.FindName("tbFee" + i);
            if (tbFee != null)
                (tbFee as TextBox).Text = subMember.Fee.ToString();
            #endregion
        }

        public void UpdateTreeCollection(Member m, List<Member> mt)
        {
            foreach(var sub in m.Subordinate)
            {
                //set supervisor for selected child
                this.Members.Where(x => x.ID == sub.ID.ToString())
                    .FirstOrDefault().Supervisor = new RefMember() { ID = m.ID, Name = m.Name };

                //clear supervisor for orignal child
                this.Members
                    .Where(x => x.ID != sub.ID.ToString() && x.Supervisor?.ID == m.ID)
                    .ToList()
                    .ForEach(x => x.Supervisor = null);
            }


        }


        private void ReCaculateBonus() {
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

                UpdateTreeCollection(this.Member, this.Members);

                ReCaculateBonus();

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
            if ((sender as ComboBox).SelectedValue == null)
                return;

            var supID = (sender as ComboBox).SelectedValue.ToString();

            //add supervisor
            this.Member.Supervisor = this.Members
                .Where(x => x.ID == supID)
                .Select(o => new RefMember() { ID = o.ID, Name = o.Name })
                .FirstOrDefault();

            //add subordinate for supervisor note
            if (!string.IsNullOrEmpty(this.Member.Supervisor?.ID))
                this.Members.Where(x => x.ID == this.Member.Supervisor.ID && !x.Subordinate.Exists(s => s.ID == this.Member.ID))
                    .FirstOrDefault()
                    ?.Subordinate
                    ?.Add(new RefMember() { ID = this.Member.ID, Name = this.Member.Name });

            //clear other sub collections
            this.Members
                .Where(x => x.ID != this.Member.Supervisor.ID && x.Subordinate.Exists(s => s.ID == this.Member.ID))
                .ToList()
                .ForEach(x => x.Subordinate = x.Subordinate.Where(y => y.ID != this.Member.ID).ToList());


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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //clear subordinate
                this.Members
                    .Where(x => x.Subordinate.Exists(s => s.ID == this.Member.ID))
                    .ToList()
                    .ForEach(x => x.Subordinate = x.Subordinate.Where(s => s.ID != this.Member.ID).ToList());

                //clear supervisor
                this.Members
                    .Where(x => x.Supervisor?.ID == this.Member.ID)
                    .ToList()
                    .ForEach(x => x.Supervisor = null);

                for (var i = this.Members.Count - 1; i >= 0; i--)
                {
                    if (this.Members[i].ID == this.Member.ID)
                    {
                        this.Members.RemoveAt(i);
                        break;
                    }
                }

                DataStorage.SaveData(this.Members);

                var dialogSuccess = new DialogSuccess("删除成功");
                dialogSuccess.ComfirmButton.Click += DialogComfirmButton_Click;
                DialogHost.Show(dialogSuccess);
            }
            catch(Exception ex)
            {
                throw ex;
            }


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
