using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Text;

namespace NugetProject
{
    public class NugetFile
    {
        [RequiredMember]
        public int requiredProperty { get; set; }

        public int notRequired { get; set; }

        [RequiredMember]
        public int requiredField;
    }
}
