using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RandomAnalyzers.RequiredMember
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RandomAnalyzersRequiredPropertyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RandomAnalyzersRequiredProperty";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString SinglePropertyMessageFormat = new LocalizableResourceString(nameof(Resources.SinglePropertyMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MultiplePropertyMessageFormat = new LocalizableResourceString(nameof(Resources.MultiplePropertyMessage), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private const string Category = "Naming";

        private static DiagnosticDescriptor SingleRule = new DiagnosticDescriptor(DiagnosticId, Title, SinglePropertyMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static DiagnosticDescriptor MultiplePropertyRule = new DiagnosticDescriptor(DiagnosticId, Title, MultiplePropertyMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(SingleRule, MultiplePropertyRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSemanticModelAction(AnalyzeSemantic);
        }

        private static void AnalyzeSemantic(SemanticModelAnalysisContext context)
        {
            var model = context.SemanticModel;
            var root = model.SyntaxTree.GetRoot();

            var nodes = root.DescendantNodes(descendIntoChildren: n => n.IsKind(SyntaxKind.ObjectCreationExpression) == false);

            var creations = nodes.Where(n => n.IsKind(SyntaxKind.ObjectCreationExpression)).Cast<ObjectCreationExpressionSyntax>();


            foreach(var creation in creations)
            {
                List<string> requiredProperties = new List<string>();

                var createdType = model.GetTypeInfo(creation);

                foreach(var member in createdType.Type.GetMembers())
                {
                    if(member is IPropertySymbol || member is IFieldSymbol)
                    {
                        foreach(AttributeData attrData in member.GetAttributes())
                        {
                            SymbolDisplayFormat displayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

                            string fullName = attrData.AttributeClass.ToDisplayString();

                            if(fullName == "RandomAnalyzers.RequiredMember.RequiredMemberAttribute")
                            {
                                requiredProperties.Add(member.Name);
                            }

                        }

                    }
                }


                if(creation.Initializer != null)
                {
                    var initializer = creation.Initializer;

                    if(initializer.Expressions.Any())
                    {
                        var expressions = initializer.Expressions;

                        foreach(var expr in expressions)
                        {
                            if(expr is AssignmentExpressionSyntax assignment)
                            {
                                string propName = assignment.Left.GetFirstToken().ValueText;

                                if (requiredProperties.Contains(propName))
                                {
                                    requiredProperties.Remove(propName);
                                }
                            }

                        }
                    }
                }

                if(requiredProperties.Any())
                {
                    DiagnosticDescriptor rule = SingleRule;

                    if(requiredProperties.Count > 1)
                    {
                        rule = MultiplePropertyRule;
                    }
                    var diagnostic = Diagnostic.Create(rule, creation.GetLocation(), createdType.Type.Name, string.Join(", ", requiredProperties));

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
