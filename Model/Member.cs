using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Model
{
    public class Member
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IDNumber { get; set; }
        public Gender Gender { get; set; }
        public string Age { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Phone { get; set; }
        public decimal Fee { get; set; }
        public decimal Bonus { get; set; }

        public RefMember Supervisor { get; set; } = new RefMember();
        public List<RefMember> Subordinate { get; set; } = new List<RefMember>();

        public RefMember Ref { get { return new RefMember() { ID = this.ID, Name = this.Name }; } }

        public Member() { }
        public Member(string ID) {
            this.ID = ID;
        }

        public static string GenerateID() {
            return DateTime.Now.ToString("yyyyMMddHHmmssms");
        }
    }

    public enum Gender
    {
        [Description("请选择")]
        None,
        [Description("男")]
        Male,
        [Description("女")]
        Female
    }

    public class RefMember
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
