using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using RandomAnalyzers.RequiredMember;

namespace RandomAnalyzers.RequiredMember.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using RandomAnalyzers.RequiredProperty;

    namespace ConsoleApplication1
    {
        class Test
        {
            [RequiredProperty]
            public int property { get; set; }

            public int second {get; set;}

        }
        class TypeName
        {   
            public void Method() 
            {

                var test = new Test()
                {
                    property = 123,
                };

                test.property = 321;
            

            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "RandomAnalyzersRequiredProperty",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RandomAnalyzersRequiredPropertyCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new RandomAnalyzersRequiredPropertyAnalyzer();
        }
    }
}
