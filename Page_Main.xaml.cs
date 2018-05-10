using Accounting.Dialog;
using Accounting.Model;
using Accounting.Util;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    return data.Name.Contains(_filterString);
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
            this.txtFilter.SetBinding(TextBox.TextProperty, new Binding("FilterString") { Source = this, Mode = BindingMode.TwoWay,UpdateSourceTrigger= UpdateSourceTrigger.PropertyChanged });


        }


        public List<Member> LoadData()
        {
            App.Members = DataStorage.LoadData();

            App.Members.ForEach(x => x.CalcuateBonusInMemberTree(App.Members));
            
            return App.Members;
        }

        
        public TreeViewItem GenerateTree(Member m)
        {

            var note = BuildChildNodes(m);
            note = BuildParentNodes(m, note);

            return note;
        }


        public TreeViewItem BuildParentNodes(Member m, TreeViewItem note)
        {

            if (m.Supervisor == null)
            {
                return note;
            }

            var parentMember = App.Members
                .Where(x => x.ID == m.Supervisor.ID && x.Name == m.Supervisor.Name)
                .FirstOrDefault<Member>();

            var parentNote = new TreeViewItem()
            {
                //ID = parentMember.ID,
                //Name = parentMember.ID,
                Tag = parentMember.ID,
                Header = $"{parentMember.Name} 奖金：{parentMember.Bonus}",
                IsExpanded = true
            };

            var childNote = note;
            parentNote.Items.Add(childNote);
            //parentNote.Children.Add(childNote);

            if (parentNote == null)
                return parentNote;

            return BuildParentNodes(parentMember, parentNote);
        }

        public TreeViewItem BuildChildNodes(Member m)
        {
            var note = new TreeViewItem()
            {
                //ID = m.ID,
                //Name = m.ID,
                Tag = m.ID,
                Header = m.Bonus==0? $"{m.Name}" : $"{m.Name} 奖金：{m.Bonus}",
                
                IsExpanded = true
            };

            foreach (var child in m.Subordinate)
            {
                var childNote = App.Members.Where(x => x.ID == child.ID).FirstOrDefault<Member>();
                note.Items.Add(BuildChildNodes(childNote));
                //note.Items.Add(BuildChildNodes(childNote));
            }

            return note;
        }


        private void dgStaff_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void dgStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
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
            this.btnBackup.Content = m.ID + '|' + m.Name;
            this.tvRelation.Items.Clear();
            this.tvRelation.Items.Add(GenerateTree(m));
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

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Data file (*.data)|*.data";
            saveFileDialog.DefaultExt = "data";
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                var path = saveFileDialog.FileName;
                DataStorage.ExportData(path, App.Members);
                DialogHost.Show(new DialogSuccess("导出成功"));
            }

            //if (result == true)
            //{
            //    //获得文件路径
            //    localFilePath = saveFileDialog.FileName.ToString();

            //    //获取文件名，不带路径
            //    fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

            //    //获取文件路径，不带文件名
            //    FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));

            //    //给文件名前加上时间
            //    newFileName = fileNameExt + "_" + DateTime.Now.ToString("yyyyMMdd");
            //    newFileName = FilePath + "\\" + newFileName;

            //    //在文件名里加字符
            //    //saveFileDialog.FileName.Insert(1,"dameng");
            //    //为用户使用 SaveFileDialog 选定的文件名创建读/写文件流。
            //    System.IO.File.WriteAllText(newFileName, wholestring); //这里的文件名其实是含有路径的。
            //}
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
            //this.FilterString = this.txtFilter.Text;
            //FilterCollection();
        }
    }
}
