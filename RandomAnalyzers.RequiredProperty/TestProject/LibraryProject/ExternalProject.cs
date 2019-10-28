using RandomAnalyzers.RequiredMember;
using System;

namespace LibraryProject
{
    public class ExternalProject
    {
        [RequiredMember]
        public int requiredProperty { get; set; }

        public int notRequired { get; set; }

        [RequiredMember]
        public int requiredField;
    }
}
