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


        public List<Member> readData()
        {
            var jsonData = data.ReadData();
            
            Members = JsonConvert.DeserializeObject<List<Member>>(jsonData);

            //ds.SaveData(JsonConvert.SerializeObject(Members));
            return Members;
        }

        
        public MemberNote GenerateTree(Member m)
        {

            var note = BuildChildNodes(m);
            note = BuildParentNodes(m, note);

            return note;
        }


        public MemberNote BuildParentNodes(Member m, MemberNote note)
        {

            if (m.Supervisor == null)
            {
                return note;
            }

            var parentMember = Members
                .Where(x => x.ID == m.Supervisor.ID && x.Name == m.Supervisor.Name)
                .FirstOrDefault<Member>();

            var parentNote = new MemberNote()
            {
                ID = parentMember.ID,
                Name = parentMember.Name,
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

        public MemberNote BuildChildNodes(Member m)
        {
            var note = new MemberNote()
            {
                ID = m.ID,
                Name = m.Name,
                Header = m.Name,
                IsExpanded = true
            };

            foreach (var child in m.Subordinate)
            {
                var childNote = Members.Where(x => x.ID == child.ID && x.Name == child.Name).FirstOrDefault<Member>();
                note.Items.Add(BuildChildNodes(childNote));
                note.Children.Add(BuildChildNodes(childNote));
            }

            return note;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var members = readData();
            this.dgStaff.DataContext = members;
        }

        private void dgStaff_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void dgStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
            if (row == null) return;

            this.SelectedMember = row.Item as Member;
            this.NavigationService.Navigate(new Uri("pack://application:,,,/Page_MemberDetail.xaml"));
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
                this.Members = DataStorage.ReadData(path);
                DataStorage.SaveData(this.Members);
                this.dgStaff.ItemsSource = this.Members;
                DialogHost.Show(new DialogSuccess("导入成功"));
            }
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("pack://application:,,,/Page_MemberDetail.xaml"));
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedMember = new Member(Member.GenerateID());
            this.NavigationService.Navigate(new Uri("pack://application:,,,/Page_MemberDetail.xaml"));
        }
    }
}
