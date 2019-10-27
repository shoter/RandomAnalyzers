using System;

namespace RandomAnalyzers.RequiredProperty
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredPropertyAttribute : Attribute
    {
    }
}
