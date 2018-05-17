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

        private string _oldParentID = string.Empty;
        private List<string> _oldChildrenID = new List<string>();
        private List<StackPanel> _childPanels = new List<StackPanel>();


        public List<ComboItem> ItemsSubordinate { get; set; } = new List<ComboItem>();
        public List<ComboItem> ItemsSupervisor { get; set; } = new List<ComboItem>();
        
        #region page init&load
        public Page_MemberDetail(Member member = null)
        {
            this.Member = member ?? App.SelectedMember;
            this._oldParentID = this.Member.Parent?.ID;
            this._oldChildrenID = this.Member.Children?.Select(x => x?.ID).ToList();

            InitializeComponent();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //refresh name based on id
            this.Member.Children = this.Member.Children.GroupBy(x => x.ID).Select(x => x.First()).ToList();
            this.Member
                .Children
                .ForEach(x => x.Name = this.Members
                    .Where(y => y.ID == x.ID)
                    .Select(n => n.Name)
                .FirstOrDefault());

            //remove invalid item (null name)
            for (var i = this.Member.Children.Count() - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(this.Member.Children[i].Name))
                {
                    this.Member.Children.RemoveAt(i);
                }
            }

            this.Member.CalcuateBonusInMemberTree(this.Members);
            this.ItemsSubordinate = this.Members
                    .Where(x => x.Children.Count <= 2 && (x.Parent == null || x.Parent?.ID !=this.Member.ID) && x.ID != this.Member.ID && x.ID != this._oldParentID && !this._oldChildrenID.Contains(x.Parent?.ID))
                    .Select(x => new ComboItem() { Name = x.Name, ID = x.ID }).ToList();


            this.ItemsSupervisor = this.Members
                .Where(x => x.Children.Count() < 2||x.Children.Exists(c=>c.ID==this.Member.ID))
                .Select(x => new ComboItem() { ID = x.ID, Name = x.Name })
                .ToList();

            BuildSubordinateCombo();
            buildControlBinding();
        }

        #endregion

        private void buildControlBinding()
        {
            this.comboGender.ItemsSource = Enum.GetValues(typeof(Gender));
            this.comboSupervisor.IsReadOnly = true;

            this.comboSupervisor.ItemsSource = ItemsSupervisor;
            comboSupervisor.DisplayMemberPath = "Name";
            comboSupervisor.SelectedValuePath = "ID";

            this.tbTitle.Text = string.IsNullOrEmpty(this.Member.Name) ? "新建人员" : "编辑人员信息";

            
            this.txtIDNumber.SetBinding(TextBox.TextProperty, new Binding("IDNumber") { Source = this.Member, Mode = BindingMode.TwoWay });
            var nameBinding = new Binding("Name") { Source = this.Member, Mode = BindingMode.TwoWay };
            nameBinding.ValidationRules.Add(new NotEmptyValidationRule());
            this.txtName.SetBinding(TextBox.TextProperty, nameBinding);
            this.comboGender.SetBinding(ComboBox.SelectedValueProperty, new Binding("Gender") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.comboSupervisor.SetBinding(ComboBox.SelectedValueProperty, new Binding("Parent.ID") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtAge.SetBinding(TextBox.TextProperty, new Binding("Age") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtPhone.SetBinding(TextBox.TextProperty, new Binding("Phone") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtBonus.SetBinding(TextBlock.TextProperty, new Binding("Bonus") { Source = this.Member, Mode = BindingMode.OneWay });
            this.txtMemberShipFee.SetBinding(TextBox.TextProperty, new Binding("Fee") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtBank.SetBinding(TextBox.TextProperty, new Binding("Bank") { Source = this.Member, Mode = BindingMode.TwoWay });
            this.txtAccount.SetBinding(TextBox.TextProperty, new Binding("Account") { Source = this.Member, Mode = BindingMode.TwoWay });

            var dateBinding = new Binding("JoinDate") { Source = this.Member, Mode = BindingMode.TwoWay };
            //dateBinding.ValidationRules.Add(new NotEmptyValidationRule());
            this.txtJoinDate.SetBinding(DatePicker.SelectedDateProperty, dateBinding);
        }

        private void AddSubordinateCombo(int i,RefMember childMember = null)
        {
            if (childMember == null)
                childMember = new RefMember();

            var comboSub1 = new ComboBox();
            comboSub1.MinWidth = 100;
            comboSub1.Margin = new Thickness(10);
            comboSub1.Name = "comboSub" + i;
            comboSub1.HorizontalAlignment = HorizontalAlignment.Stretch;
            var currentItem = childMember;
            comboSub1.ItemsSource = this.ItemsSubordinate.Concat(new[] { new ComboItem(currentItem.ID, currentItem.Name) });
            comboSub1.DisplayMemberPath = "Name";
            comboSub1.SelectedValuePath = "ID";
            comboSub1.SetBinding(ComboBox.SelectedValueProperty, new Binding("ID") { Source = childMember, Mode = BindingMode.TwoWay });
            comboSub1.SelectionChanged += ComboSub1_SelectionChanged;
            comboSub1.IsEditable = true;
            comboSub1.Style = this.FindResource("MaterialDesignFloatingHintComboBox") as Style;
            HintAssist.SetHint(comboSub1, "关联下属");


            var textMember = this.Members
                .Where(x => x.ID == childMember.ID)
                .FirstOrDefault();
            

            var textblockPhone = new TextBox();
            textblockPhone.Name = "tbPhone" + i.ToString();
            if(this.FindName(textblockPhone.Name)!=null)
                this.RegisterName(textblockPhone.Name, textblockPhone);
            textblockPhone.Margin = new Thickness(10);
            textblockPhone.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockPhone.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
            textblockPhone.Text = textMember?.Phone;
            HintAssist.SetHint(textblockPhone, "电话号码");
            textblockPhone.IsEnabled = false;

            var textblockIDNumber = new TextBox();
            textblockIDNumber.Name = "tbIDNumber" + i.ToString();

            if (this.FindName(textblockIDNumber.Name) != null)
                this.RegisterName(textblockIDNumber.Name, textblockIDNumber);
            textblockIDNumber.Margin = new Thickness(10);
            textblockIDNumber.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockIDNumber.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
            textblockIDNumber.Text = textMember?.IDNumber;
            HintAssist.SetHint(textblockIDNumber, "身份证号");
            textblockIDNumber.IsEnabled = false;


            var textblockFee = new TextBox();
            textblockFee.Name = "tbFee" + i.ToString();
            if (this.FindName(textblockFee.Name) != null)
                this.RegisterName(textblockFee.Name, textblockIDNumber);
            textblockFee.Margin = new Thickness(10);
            textblockFee.HorizontalAlignment = HorizontalAlignment.Stretch;
            textblockFee.Style = this.FindResource("MaterialDesignFloatingHintTextBox") as Style;
            textblockFee.Text = textMember?.Fee.ToString();
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
            this._childPanels.Add(panel1);

            if(i==1)
            {
                this.btnPopup.Visibility = Visibility.Hidden;
            }
        }

        private void BuildSubordinateCombo()
        {
            int i = 0;
            this.panelSubordinate.Children.Clear();
            for (i = 0; i < this.Member.Children.Count; i++)
            {
                AddSubordinateCombo(i,this.Member.Children[i]);
            }
            subbuttonI = i;
        }

        private void BtnDelete2_MouseUp1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var p = (sender as Button).Parent as StackPanel;

                var i = (sender as Button).Name.Replace("delbtn_i", string.Empty);
                p.Children.Clear();
                this.Member.Children.RemoveAt(int.Parse(i));
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
                //this.UnregisterName("tbPhone" + strI);
                //this.UnregisterName("tbFee" + strI);
                //this.UnregisterName("tbIDNumber" + strI);
                
                var removeMember = this.Member.Children[i];
                var relatedChildren = this.Members.Where(x => x.ID ==removeMember.ID && x.Parent?.ID == this.Member.ID).ToList();
                relatedChildren.ForEach(x => x.Parent = null);
                this.Member.Children.RemoveAt(i);
                this.Member.CalcuateBonusInMemberTree(this.Members);
                this.txtBonus.Text = this.Member.Bonus.ToString();
                p.Children.Clear();
                this.btnPopup.Visibility = Visibility.Visible;
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
                .FirstOrDefault().Parent = new RefMember(this.Member.ID, this.Member.Name);

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
            foreach(var child in m.Children)
            {
                //set supervisor for selected child
                this.Members.Where(x => x.ID == child.ID.ToString())
                    .FirstOrDefault().Parent = new RefMember(m.ID,m.Name);
            }

            //update parent's children's Name
            var parentNote = this.Members.Where(x => x.ID == this.Member.Parent?.ID)
                .FirstOrDefault();
            if (parentNote != null)
            {

                var parentChild = parentNote.Children
                .Where(x => x.ID == this.Member.ID).FirstOrDefault();
                if (parentChild != null)
                {
                    parentChild.Name = this.Member.Name;
                }
            }

            if(!string.IsNullOrEmpty(this.Member.Parent?.ID))
            {
                var parentMember = this.Members
                    .Where(x => x.ID == this.Member.Parent.ID)
                    .FirstOrDefault();

                if(parentMember!=null && !parentMember.Children.Exists(x=>x.ID == this.Member.ID))
                {
                    parentMember.Children.Add(new RefMember(m.ID, m.Name));
                }
            }


            //clear orignal supervisor's subordinate if changed
            if (this.Member.Parent?.ID != this._oldParentID)
            {
                this.Members
                  .Where(x => x.ID == this._oldParentID)
                  .ToList()
                  .ForEach(x => x.Children = x.Children.Where(y => y.ID != this.Member.ID).ToList());
            }


            //clear orignal Subordinate's supervisor if changed
            foreach (var subid in this._oldChildrenID)
            {
                if (!this.Member.Children.Exists(x => x.ID == subid))
                {
                    this.Members
                        .Where(x => x.ID == subid)
                        .ToList()
                        .ForEach(x => x.Parent = null);
                }
            }
        }


        private void ReCaculateBonus() {
        }


        #region events
        private int subbuttonI = 0;
        private void AddSubButton_Click_1(object sender, RoutedEventArgs e)
        {
            var panel = this.FindName("panelSubordinate") as StackPanel;
            var childMember = new RefMember();
            this.Member.Children.Add(childMember);
            //AddSubordinateCombo(this.Member.Children.Count - 1);
            AddSubordinateCombo(subbuttonI, childMember);
        }
        private void AddNewSubButton_Click_1(object sender, RoutedEventArgs e)
        {
            var mid = Member.GenerateID();
            var newMember = new Member(mid)
            {
                Parent = new RefMember()
                {
                    ID = this.Member.ID,
                    Name = this.Member.Name
                }
            };
            this.Member.Children.Add(new RefMember() { ID = mid });
            App.SelectedMemberStack.Push(this.Member);
            App.SelectedMember = newMember;

            this.NavigationService.Navigate(new Page_MemberDetail(newMember));
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

                this.Member.Children
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
            this.Member.Parent = this.Members
                .Where(x => x.ID == supID)
                .Select(o => new RefMember(o.ID, o.Name))
                .FirstOrDefault();

            //add subordinate for supervisor note
            if (!string.IsNullOrEmpty(this.Member.Parent?.ID))
                this.Members.Where(x => x.ID == this.Member.Parent.ID && !x.Children.Exists(s => s.ID == this.Member.ID))
                    .FirstOrDefault()
                    ?.Children
                    ?.Add(new RefMember(this.Member.ID, this.Member.Name));

            //clear other sub collections
            this.Members
                .Where(x => x.ID != this.Member.Parent.ID && x.Children.Exists(s => s.ID == this.Member.ID))
                .ToList()
                .ForEach(x => x.Children = x.Children.Where(y => y.ID != this.Member.ID).ToList());


        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BasePage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //clear subordinate
                this.Members
                    .Where(x => x.Children.Exists(s => s.ID == this.Member.ID))
                    .ToList()
                    .ForEach(x => x.Children = x.Children.Where(s => s.ID != this.Member.ID).ToList());

                //clear supervisor
                this.Members
                    .Where(x => x.Parent?.ID == this.Member.ID)
                    .ToList()
                    .ForEach(x => x.Parent = null);

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

        #endregion

        private void btnGoback_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("pack://application:,,,/Page_Main.xaml"));
        }

        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (this.Parent as Window).DragMove();
        }
    }
}
