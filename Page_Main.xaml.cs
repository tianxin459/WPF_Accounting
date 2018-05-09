using Accounting.Dialog;
using Accounting.Model;
using Accounting.Util;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
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
    /// Interaction logic for Page_Main.xaml
    /// </summary>
    public partial class Page_Main : BasePage
    {
        DataStorage data;
        //private List<Member> Members = new List<Model.Member>();
        public Page_Main():base()
        {
            data = new DataStorage();
            InitializeComponent();
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
                Header = parentMember.Name,
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
                Header = m.Name,
                
                IsExpanded = true
            };

            foreach (var child in m.Subordinate)
            {
                var childNote = App.Members.Where(x => x.ID == child.ID && x.Name == child.Name).FirstOrDefault<Member>();
                note.Items.Add(BuildChildNodes(childNote));
                //note.Items.Add(BuildChildNodes(childNote));
            }

            return note;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.dgStaff.ItemsSource = LoadData();
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
            this.NavigationService.Navigate(new Uri("pack://application:,,,/Page_MemberDetail.xaml"));
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            App.SelectedMember = new Member(Member.GenerateID());
            this.NavigationService.Navigate(new Page_MemberDetail(new Member(Member.GenerateID())));
        }
    }
}
