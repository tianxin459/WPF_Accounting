using Accounting.Model;
using Accounting.Util;
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
using Newtonsoft.Json;

namespace Accounting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataStorage data;
        private List<Member> Members = new List<Model.Member>();
        public MainWindow()
        {
            data = new DataStorage();
            InitializeComponent();
        }


        public List<Member> LoadData()
        {
            Members = DataStorage.LoadData();

            //ds.SaveData(JsonConvert.SerializeObject(Members));
            return Members;
        }


        public void GenerateTree()
        {
            MemberNote root = new MemberNote() { Name = "Menu" };
            MemberNote childItem1 = new MemberNote() { Name = "Child item #1" };
            childItem1.Children.Add(new MemberNote() { Name = "Child item #1.1" });
            childItem1.Children.Add(new MemberNote() { Name = "Child item #1.2" });
            root.Children.Add(childItem1);
            root.Children.Add(new MemberNote() { Name = "Child item #2" });
            this.tvRelation.Items.Add(root);
        }

        public MemberNote GenerateTree(Member m)
        {

            var note = BuildChildNodes(m);
            note = BuildParentNodes(m, note);

            return note;
        }


        public MemberNote BuildParentNodes(Member m,MemberNote note)
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
            };

            var childNote = note;
            parentNote.Children.Add(childNote);

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
            };

            foreach(var child in m.Subordinate)
            {
                var childNote = Members.Where(x => x.ID == child.ID && x.Name == child.Name).FirstOrDefault<Member>();
                note.Children.Add(BuildChildNodes(childNote));
            }

            return note;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var members = LoadData();
            this.dgStaff.DataContext = members;
        }

        private void dgStaff_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void dgStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
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
            MemberDetail popup = new MemberDetail();
            popup.ShowDialog();
        }
    }
}
