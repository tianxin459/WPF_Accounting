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
        public string MID { get; set; } = "";
        public string Name { get; set; } = "";
        public string IDNumber { get; set; } = "";
        public Gender Gender { get; set; }
        public string Age { get; set; } = "";
        public string Bank { get; set; } = "";
        public string Account { get; set; } = "";
        public string Phone { get; set; } = "";
        public decimal Fee { get; set; }
        public decimal Bonus { get; set; }
        public string JoinDate { get; set; } = "";

        public RefMember Parent { get; set; }
        public List<RefMember> Children { get; set; } = new List<RefMember>();

        public RefMember Ref { get { return new RefMember(this.ID,this.Name); } }
        public string ChildrenText { get; set; }

        public Member() { }
        public Member(string ID) {
            this.ID = ID;
        }

        [JsonIgnore]
        public string CalTextBuilder { get {
                return String.Join(" + ", this.ChildrenList.Select(x=>x.Split('|')[0]));
            } }
        [JsonIgnore]
        public List<string> ChildrenList { get; set; } = new List<string>();
        [JsonIgnore]
        public List<string> ChildrenIDList { get; set; } = new List<string>();

        public decimal CalcuateBonusInMemberTree(List<Member> memberTree)
        {
            //this.CalTextBuilder = new StringBuilder();
            List<string> ChildrenList = new List<string>();

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
            
            List<int> lvs = new List<int>();
            
            foreach (var sm in m.Children)
            {
                var sumChild = SumChildrenBonus(sm.ID, memberTree, this.ChildrenList,(BonusBase.Count - 1));
                sumChild.Level++;
                sumChildren.Add(sumChild);
            }

            foreach (var s in sumChildren)
            {
                var sl = new decimal();
                bonus += s.Sum;
                if (s.Level < BonusBase.Count())
                {
                    sl = BonusBase[s.Level];

                    this.ChildrenList.Add(string.Format("Lv{1}:{0}", sl, s.Level));
                    //this.CalTextBuilder.Append("+");
                    bonus += sl;
                }
            }
            
            var lvl = sumChildren.Select(x => x.Level).Max();
            this.ChildrenList.Add(string.Format("|MaxLv:{0}", lvl));
            decimal extBonus = 0 ;

            if (sumChildren.Any(x=>x.ExtractBonusBool))
            {
                if (App.Members.Where(x => !string.IsNullOrEmpty(x.JoinDate) && DateTime.Parse(x.JoinDate).Year == DateTime.Now.Year).Count() > 100)
                {
                    extBonus = (decimal)(380000 * 0.02);
                    this.ChildrenList.Add(string.Format(" ({1}级后奖励金:{0})", extBonus, (BonusBase.Count - 1)));
                }
            }

            this.Bonus = bonus + extBonus;

            //fill up with children ids
            this.ChildrenIDList.Clear();
            FillChildrenIDs(this, this.ChildrenIDList, memberTree);

            //fill up the children text;
            this.ChildrenText = String.Join("\n", App.Members.Where(x => this.Children.Exists(c => c.ID == x.ID)).Select(s => s.MID + " - " + s.Name).ToArray());

            return bonus;
        }

        /// <summary>
        /// caculate and sum up the bouns for children
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberTree"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public CalItem SumChildrenBonus(string id, List<Member> memberTree, List<string> sb,int remainLevel)
        {
            CalItem sumReturn = new CalItem();
            List<CalItem> sumChildren = new List<CalItem>();

            var m = memberTree
                .Where(x => x.ID == id)
                .FirstOrDefault();

            if (m==null||m.Children.Count == 0||remainLevel==0)
            {
                sumReturn.Level = 0;
                sumReturn.Sum = 0;
                if (m.Children.Count > 0)
                {
                    sumReturn.ExtractBonusBool = true;
                }
                return sumReturn;
            }

            List<int> lvs = new List<int>();
            
            foreach (var sm in m.Children)
            {
                var sumChild = new CalItem();
                sumChild = SumChildrenBonus(sm.ID, memberTree, sb,(remainLevel-1));
                sumChild.Level++;
                sumChildren.Add(sumChild);
            }
            
            foreach(var s in sumChildren)
            {
                if (s.Level < BonusBase.Count())
                {
                    sumReturn.Sum += s.Sum;

                    var sl = BonusBase[s.Level];
                    sb.Add(string.Format("{1}:{0}", sl, m.Name));
                    //sb.Append("+");
                    sumReturn.Sum += sl;
                }
                else
                {
                    sumReturn.Sum += s.Sum;
                }
            }
            sumReturn.Level = sumChildren.Select(x => x.Level).Max();
            sumReturn.ExtractBonusBool = sumChildren.Any(x => x.ExtractBonusBool == true);
            return sumReturn;
        }

        public static string GenerateID() {
            return DateTime.Now.ToString("yyyyMMddHHmmssms");
        }


        public static void FillChildrenIDs(Member m, List<string> idList,List<Member> memberTree)
        {
            foreach (var c in m.Children)
            {
                idList.Add(c.ID);
                var mc = memberTree.Where(x => x.ID == c.ID).FirstOrDefault();
                if (mc == null)
                    continue;
                FillChildrenIDs(mc, idList, memberTree);
            }
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
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";

        public RefMember(string id="", string name = "")
        {
            ID = id;
            Name = name;
        }
    }
}
