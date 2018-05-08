using Newtonsoft.Json;
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

        [JsonIgnore]
        private List<decimal> BonusBase = new List<decimal>() { 228, 190, 152, 114, 76, 38 };


        public decimal CalcuateBonusInMemberTree(List<Member> memberTree)
        {
            decimal bonus = 0;
            var m = memberTree
                .Where(x => x.ID == this.ID)
                .FirstOrDefault();


            if(m==null || m.Subordinate.Count==0)
            {
                return 0;
            }

            var level = 0;
            List<int> lvs = new List<int>();

            foreach (var sm in m.Subordinate)
            {
                var lv = level;
                bonus += SumChildrenBonus(sm.ID, memberTree,ref lv);
                if (lv < BonusBase.Count)
                {
                    bonus += BonusBase[lv];
                }
                lvs.Add(lv);
            }

            if (lvs.Max() > BonusBase.Count) // if 6 gen exceed, then have plus share of 2% of 38w;
            {
                bonus += (decimal)(380000 * 0.02);
            }

            this.Bonus = bonus;
            return bonus;
        }

        /// <summary>
        /// caculate and sum up the bouns for children
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberTree"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public decimal SumChildrenBonus(string id, List<Member> memberTree, ref int level)
        {
            decimal bonus = 0;
            var m = memberTree
                .Where(x => x.ID == id)
                .FirstOrDefault();

            if (m==null||m.Subordinate.Count == 0)
            {
                level = 0;
                return 0;
            }

            level++;

            foreach (var sm in m.Subordinate)
            {
                var lv = level;
                bonus += SumChildrenBonus(sm.ID, memberTree, ref lv);
                bonus += BonusBase[lv];
            }

            this.Bonus = bonus;
            return bonus;
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
