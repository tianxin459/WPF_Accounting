using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Model
{
    public class TreeViewTagObj
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public TreeViewTagObj(string id, string text)
        {
            ID = id;
            Text = text;
        }
    }
}
