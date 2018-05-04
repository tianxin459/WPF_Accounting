using Accounting.Model;
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

namespace Accounting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<Member> Members = new List<Model.Member>();

        public List<Member> readData()
        {
            var members = new List<Member>();

            for (int i = 0; i < 10; i++)
            {
                members.Add(new Member()
                {
                    ID = i.ToString(),
                    Name = "Name" + i,
                    Account = "xxxx-xxxx-xxxx-xxxx".Replace("x", i.ToString()),
                    Age = (i * 9).ToString(),
                    Phone = "xxxxxxxxxxxxxxxx".Replace("x", i.ToString()),
                    Fee = i,
                    Bonus = i
                });
            }
            members[2].Supervisor = members[1];
            members[2].Subordinate.Add(members[3]);
            members[2].Subordinate.Add(members[4]);


            members[2].Supervisor2 = members[1].Ref;
            members[2].Subordinate2.Add(members[3].Ref);
            members[2].Subordinate2.Add(members[4].Ref);
            this.tvRelation.Items.Add(members[2].Ref);
            Members = members;
            return members;
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
            while (m.Supervisor == null)
            {
                var notec = new MemberNote()
                {
                    ID = m.ID,
                    Name = m.Name,
                    Children = m.Subordinate.Select(x => new MemberNote() { ID = x.ID, Name = x.Name }).ToList()
                };
            }

            var note = BuildChildNodes(m);

            return GenerateTree(m.Supervisor);
        }


        public MemberNote BuildParentNodes(Member m)
        {

            if (m.Supervisor == null)
            {
                return null;
            }

            var parentNote = new MemberNote()
            {
                ID = m.ID,
                Name = m.Name,
                Children = new List<MemberNote>() { new MemberNote() { ID = m.ID, Name = m.Name } }
            };


            return parentNote;
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
                note.Children.Add(BuildChildNodes(m));
            }

            return note;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var members = readData();
            this.dgStaff.DataContext = members;
            //GenerateTree();
        }

        private void dgStaff_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            //if (row == null) return;
            //Member m = row.Item as Member;
            //this.btnBackup.Content = m.ID+'|'+m.Name;
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

            this.tvRelation.Items.Add(GenerateTree(m));
        }
    }
}
