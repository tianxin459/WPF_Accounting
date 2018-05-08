using Accounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Accounting
{
    public class BasePage:Page
    {
        public const string GLOBAL_MEMBERS = "Members";
        public const string GLOBAL_SELECTEDMEMBERS = "SelectedMembers";
        public const string GLOBAL_SELECTEDMEMBERSTACK = "SelectedMemberStack";
        public List<Member> Members {
            get {
                if (!Application.Current.Properties.Contains(GLOBAL_MEMBERS))
                    return null;
                return Application.Current.Properties[GLOBAL_MEMBERS] as List<Member>;
            }
            set {
                Application.Current.Properties[GLOBAL_MEMBERS] = value;
            }
        }

        public Member SelectedMember
        {
            get
            {
                if (!Application.Current.Properties.Contains(GLOBAL_SELECTEDMEMBERS))
                    return null;
                return Application.Current.Properties[GLOBAL_SELECTEDMEMBERS] as Member;
            }
            set
            {
                Application.Current.Properties[GLOBAL_SELECTEDMEMBERS] = value;
            }
        }

        public Stack<Member> SelectedMemberStack
        {
            get
            {
                if (!Application.Current.Properties.Contains(GLOBAL_SELECTEDMEMBERSTACK))
                    return new Stack<Member>();
                return Application.Current.Properties[GLOBAL_SELECTEDMEMBERSTACK] as Stack<Member>;
            }
            set
            {
                Application.Current.Properties[GLOBAL_SELECTEDMEMBERSTACK] = value;
            }
        }
        
    }


    public partial class c : BasePage {
        public c() {
        }
    }
}
