using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject
{
    public class ExternalFile
    {
        [RequiredMember]
        public int requiredProperty { get; set; } 

        public int notRequired { get; set; }

        [RequiredMember]
        public int requiredField;
    }
}
