﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Accounting.Model
{
    public class MemberNote
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public string ColorStr { get; set; } = "White";
        public string FontColor { get; set; } = "DarkMagena";

        public List<MemberNote> Children { get; set; } = new List<MemberNote>();
    }
}
