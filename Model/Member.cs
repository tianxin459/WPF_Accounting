using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Model
{
    public class Member
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Phone { get; set; }
        public decimal Fee { get; set; }
        public decimal Bonus { get; set; }

        public Member Supervisor { get; set; }
        public List<Member> Subordinate { get; set; } = new List<Member>();

        public RefMember Supervisor2 { get; set; }
        public List<RefMember> Subordinate2 { get; set; } = new List<RefMember>();

        public RefMember Ref { get { return new RefMember() { ID = this.ID, Name = this.Name }; } }
    }

    public class RefMember
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
