using Accounting.Dialog;
using Accounting.Model;
using Accounting.Util;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Accounting
{
    /// <summary>
    /// Interaction logic for Page_Main.xaml
    /// </summary>
    public partial class Page_Main : BasePage
    {
        DataStorage data;
        #region filter
        private string _filterString = "";
        private ICollectionView _dataGridCollection;
        public ICollectionView DataGridCollection
        {
            get { return _dataGridCollection; }
            set {
                _dataGridCollection = value;
                NotifyPropertyChanged("DataGridCollection");
            }
        }
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                NotifyPropertyChanged("FilterString");
                FilterCollection();
            }
        }

        private void FilterCollection()
        {
            if (_dataGridCollection != null)
            {
                _dataGridCollection.Refresh();
            }
        }

        public bool Filter(object obj)
        {
            var data = obj as Member;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(_filterString))
                {
                    return (data.Name?.Contains(_filterString) ?? false)||(data.MID?.Contains(_filterString)??false);
                }
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        public Page_Main():base()
        {
            data = new DataStorage();
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();

            DataGridCollection = CollectionViewSource.GetDefaultView(this.Members);
            DataGridCollection.Filter = new Predicate<object>(Filter);
            this.dgStaff.ItemsSource = DataGridCollection;
            this.txtFilter.DataContext = FilterString;
            this.txtFilter.SetBinding(TextBox.TextProperty, new Binding("FilterString") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            this.NavigationService.RemoveBackEntry();
        }

        public void GenerateTestData()
        {
            for (int i = 1; i < 120; i++)
            {
                App.Members.Add(new Member() {
                    ID = i.ToString(),
                    Name = "Name" + i,
                    Account = "Account" + i,
                    Phone = "123"+i,
                });
            }
        }


        public List<Member> LoadData()
        {
            App.Members = DataStorage.LoadData();

            App.Members.ForEach(x => x.CalcuateBonusInMemberTree(App.Members));

#if DEBUG
            //GenerateTestData();
#endif

            return App.Members;
        }

        #region treeview build MemberNote
        public MemberNote GenerateTree(Member m)
        {

            var note = BuildChildNodes(m);
            note.ColorStr = "BlueViolet";
            note = BuildParentNodes(m, note);
            return note;
        }


        public MemberNote BuildParentNodes(Member m, MemberNote note)
        {

            if (m.Parent == null)
            {
                return note;
            }

            //var parentMember = App.Members
            //    .Where(x => x.ID == m.Supervisor.ID && x.Name == m.Supervisor.Name)
            //    .FirstOrDefault<Member>();
            var parentMemberRef = App.Members
                  .Where(x => x.Children.Exists(y => y.ID == m.ID))
                  .FirstOrDefault<Member>();
            if (parentMemberRef == null)
                return null;
            //?.Subordinate.Where(x=>x.ID==m.ID).FirstOrDefault();

            var parentMember = App.Members.Where(x => x.ID == parentMemberRef.ID).FirstOrDefault();

            var parentNote = new MemberNote()
            {
                //ID = parentMember.ID,
                Name = parentMember.Name,
                ID = parentMember.ID,
                Remark = $"{parentMember.Name} 奖金：{parentMember.Bonus}",
                //ColorStr = "lavender",
                //FontColor = "DarkMagena"
                //IsExpanded = true
            };

            var childNote = note;
            parentNote.Children.Add(childNote);
            //parentNote.Children.Add(childNote);

            if (parentNote == null)
                return parentNote;

            return BuildParentNodes(parentMember, parentNote);
        }

        public MemberNote BuildChildNodes(Member m)
        {
            var note = new MemberNote()
            {
                //ID = m.ID,
                Name = m.Name,
                ID = m.ID,
                Remark = m.Bonus == 0 ? $"{m.Name}" : $"{m.Name} 奖金：{m.Bonus} (" + m.CalTextBuilder+")",
                //IsExpanded = true
            };

            foreach (var child in m.Children)
            {
                var childNote = App.Members.Where(x => x.ID == child.ID).FirstOrDefault<Member>();
                if (childNote == null)
                {
                    child.ID = "";
                    continue;
                }
                note.Children.Add(BuildChildNodes(childNote));
                //note.Items.Add(BuildChildNodes(childNote));
            }
            m.Children = m.Children.Where(x => !string.IsNullOrEmpty(x.ID)).ToList();

            return note;
        }
        #endregion

        #region treeview build TreeViewItem
        //public TreeViewItem GenerateTree(Member m)
        //{

        //    var note = BuildChildNodes(m);
        //    note = BuildParentNodes(m, note);
        //    return note;
        //}


        //public TreeViewItem BuildParentNodes(Member m, TreeViewItem note)
        //{

        //    if (m.Parent == null)
        //    {
        //        return note;
        //    }

        //    //var parentMember = App.Members
        //    //    .Where(x => x.ID == m.Supervisor.ID && x.Name == m.Supervisor.Name)
        //    //    .FirstOrDefault<Member>();
        //    var parentMemberRef = App.Members
        //          .Where(x => x.Children.Exists(y => y.ID == m.ID))
        //          .FirstOrDefault<Member>();
        //    if (parentMemberRef == null)
        //        return null;
        //    //?.Subordinate.Where(x=>x.ID==m.ID).FirstOrDefault();

        //    var parentMember = App.Members.Where(x => x.ID == parentMemberRef.ID).FirstOrDefault();

        //    var parentNote = new TreeViewItem()
        //    {
        //        //ID = parentMember.ID,
        //        //Name = parentMember.ID,
        //        Tag = new TreeViewTagObj(parentMember.ID, parentMember.calText.ToString()),
        //        Header = $"{parentMember.Name} 奖金：{parentMember.Bonus}",
        //        IsExpanded = true
        //    };

        //    var childNote = note;
        //    parentNote.Items.Add(childNote);
        //    //parentNote.Children.Add(childNote);

        //    if (parentNote == null)
        //        return parentNote;

        //    return BuildParentNodes(parentMember, parentNote);
        //}

        //public TreeViewItem BuildChildNodes(Member m)
        //{
        //    var note = new TreeViewItem()
        //    {
        //        //ID = m.ID,
        //        //Name = m.ID,
        //        Tag = new TreeViewTagObj(m.ID, m.calText.ToString()),
        //        Header = m.Bonus==0? $"{m.Name}" : $"{m.Name} 奖金：{m.Bonus} --"+m.calText,
                
        //        IsExpanded = true
        //    };

        //    foreach (var child in m.Children)
        //    {
        //        var childNote = App.Members.Where(x => x.ID == child.ID).FirstOrDefault<Member>();
        //        if(childNote==null)
        //        {
        //            child.ID = "";
        //            continue;
        //        }
        //        note.Items.Add(BuildChildNodes(childNote));
        //        //note.Items.Add(BuildChildNodes(childNote));
        //    }
        //    m.Children = m.Children.Where(x => !string.IsNullOrEmpty(x.ID)).ToList();

        //    return note;
        //}
        #endregion


        private void dgStaff_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void dgStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.SelectedIndex < 0)
                return;
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
            if (row == null) return;

            App.SelectedMember = row.Item as Member;
            this.NavigationService.Navigate(new Page_MemberDetail(App.SelectedMember));
        }

        private void dgStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
            if (row == null) return;
            Member m = row.Item as Member;


            List<MemberNote> treeSource = new List<MemberNote>();
            treeSource.Add(GenerateTree(m));
            //List<TreeViewItem> treeSource = new List<TreeViewItem>();
            //treeSource.Add(GenerateTree(m));
            this.tvRelation.ItemsSource = treeSource;
            //this.tvRelation.Items.Clear();
            //this.tvRelation.Items.Add(GenerateTree(m));
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            App.SelectedMember = new Member(Member.GenerateID());
            this.NavigationService.Navigate(new Page_MemberDetail(new Member(Member.GenerateID())));
        }

        private void tvRelation_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pathHtmlTemplate = path + "\\Print\\Print.html";
            var print_text = File.ReadAllText(pathHtmlTemplate);
            print_text = print_text.Replace("$data$", JsonConvert.SerializeObject(this.Members));
            var printPath = pathHtmlTemplate.Replace("Print.html", "Print1.html");
            File.WriteAllText(printPath, print_text);
            //DataStorage.ExportPrintFile(printPath, print_text);
            System.Diagnostics.Process.Start(printPath);
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Data file (*.data)|*.data|Excel (*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "data";
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                if (path.EndsWith(".xlsx"))
                {
                    ExcelUtil.ExportToExcel(path, App.Members);
                }
                else
                {
                    DataStorage.ExportData(path, App.Members);
                }
                DialogHost.Show(new DialogSuccess("导出成功"));
            }
        }
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data file (*.data)|*.data";
            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                App.Members = DataStorage.ReadData(path);
                DataStorage.SaveData(App.Members);
                this.dgStaff.ItemsSource = App.Members;
                DialogHost.Show(new DialogSuccess("导入成功"));
            }
        }

        private void btnGoback_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (this.Parent as Window).DragMove();
        }

        private void btnBonusConfig_Click(object sender, RoutedEventArgs e)
        {

            var dialogBonusConfig = new BonusConfigDialog();
            //dialogSuccess.ComfirmButton.Click += DialogComfirmButton_Click;
            dialogBonusConfig.SaveButton.Click += BonusDialogSaveButton_Click;
            DialogHost.Show(dialogBonusConfig);
        }

        private void BonusDialogSaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Members.ForEach(x => x.CalcuateBonusInMemberTree(App.Members));

            DataGridCollection = CollectionViewSource.GetDefaultView(this.Members);
            DataGridCollection.Filter = new Predicate<object>(Filter);
            this.dgStaff.ItemsSource = DataGridCollection;
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "data";
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                ExcelUtil.ExportToExcel(path, App.Members);
                DialogHost.Show(new DialogSuccess("导出成功"));
            }
        }
    }
}
