﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Model
{
    public class MemberNote
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public List<MemberNote> Children { get; set; } = new List<MemberNote>();
    }
}
