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

        [JsonIgnore]
        public static List<decimal> BonusBase = new List<decimal>() { 0, 228, 190, 152, 114, 76, 38 };

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

        public RefMember Parent { get; set; }
        public List<RefMember> Children { get; set; } = new List<RefMember>();

        public RefMember Ref { get { return new RefMember(this.ID,this.Name); } }

        public Member() { }
        public Member(string ID) {
            this.ID = ID;
        }

        [JsonIgnore]
        public StringBuilder calText { get; set; } = new StringBuilder();

        public decimal CalcuateBonusInMemberTree(List<Member> memberTree)
        {
            decimal bonus = 0;
            List<CalItem> sumChildren = new List<CalItem>();
            var m = memberTree
                .Where(x => x.ID == this.ID)
                .FirstOrDefault();


            if(m==null || m.Children.Count==0)
            {
                this.Bonus = bonus;
                return 0;
            }

            var level = 0;
            List<int> lvs = new List<int>();

            int bi = 1;
            foreach (var sm in m.Children)
            {
                var lv = level;

                var sumChild = SumChildrenBonus(sm.ID, memberTree, ref lv, this.calText);
                sumChild.Level++;
                var lvBonus = new Decimal();
                sumChildren.Add(sumChild);
            }
            foreach (var s in sumChildren)
            {
                var sl = new decimal();
                bonus += s.Sum;
                if (s.Level < BonusBase.Count())
                {
                    sl = BonusBase[s.Level];

                    this.calText.Append(string.Format("Lv{1}-{0}", sl, s.Level));
                    this.calText.Append("+");
                    bonus += sl;
                }
            }
            
            var lvl = sumChildren.Select(x => x.Level).Max();
            this.calText.Append(string.Format("|MaxLv:{0}", lvl));

            if (sumChildren.Count > 0 && lvl > BonusBase.Count) // if 6 gen exceed, then have plus share of 2% of 38w;
            {
                if (App.Members.Count() > 100)
                {
                    var extBonus = (decimal)(380000 * 0.02);
                    this.calText.Append(string.Format("|MaxLv:{0} (6级后奖励金:{1})", extBonus));
                }
                    
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
        public CalItem SumChildrenBonus(string id, List<Member> memberTree, ref int level, StringBuilder sb)
        {
            CalItem sumReturn = new CalItem();
            List<CalItem> sumChildren = new List<CalItem>();
            decimal bonus = 0;
            var m = memberTree
                .Where(x => x.ID == id)
                .FirstOrDefault();

            if (m==null||m.Children.Count == 0)
            {
                sumReturn.Level = 0;
                sumReturn.Sum = 0;
                return sumReturn;
            }

            List<int> lvs = new List<int>();
            
            foreach (var sm in m.Children)
            {
                var lv = level;
                var sumChild = new CalItem();
                sumChild = SumChildrenBonus(sm.ID, memberTree, ref lv, sb);
                sumChild.Level++;
                var lvBonus = new Decimal() ;
                //if (sumChild.Level < BonusBase.Count())
                //{ 
                //    lvBonus =  BonusBase[sumChild.Level];
                //}
                //bonus += lvBonus;
                //sb.Append(string.Format("(c_{0}+lv{1}_{2})", childBonusSum.ToString(),lv+1, lvBonus.ToString()));
                //sb.Append(string.Format("{0}", sumChild.Sum.ToString()));
                //sb.Append("+");
                lvs.Add(lv);
                sumChildren.Add(sumChild);
            }

            //level++;
            

            //if (lvs.Count() > 0)
            //{
            //    level = lvs.Max();
            //}
            foreach(var s in sumChildren)
            {
                if (s.Level < BonusBase.Count())
                {
                    sumReturn.Sum += s.Sum;

                    var sl = BonusBase[s.Level];
                    sb.Append(string.Format("{1}:{0}", sl,m.Name));
                    sb.Append("+");
                    sumReturn.Sum += sl;
                }
            }
            sumReturn.Level = sumChildren.Select(x => x.Level).Max();
            //this.Bonus = bonus;
            return sumReturn;
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

        public RefMember(string id="", string name = "")
        {
            ID = id;
            Name = name;
        }
    }
}
