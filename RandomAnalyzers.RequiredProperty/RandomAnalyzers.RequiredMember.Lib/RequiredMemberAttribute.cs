using System;

namespace RandomAnalyzers.RequiredMember
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredMemberAttribute : Attribute
    {
    }
}
