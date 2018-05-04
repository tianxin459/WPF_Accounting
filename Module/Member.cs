using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Module
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
        public RefMember Supervisor { get; set; }

        public List<RefMember> Subordinate { get; set; } = new List<RefMember>();

        public RefMember Ref { get { return new RefMember() { ID = this.ID, Name = this.Name }; } }
    }

    public class RefMember
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
