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
        public const string DiagnosticId = "RandomAnalyzersRequiredMember";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString singleMemberMessageFormat = new LocalizableResourceString(nameof(Resources.SingleMemberMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString multipleMembersMessageFormat = new LocalizableResourceString(nameof(Resources.MultipleMembersMessage), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private const string category = "Correctness";

        private static DiagnosticDescriptor singlePropertyRule = new DiagnosticDescriptor(DiagnosticId, title, singleMemberMessageFormat, category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: description);
        private static DiagnosticDescriptor multiplePropertyRule = new DiagnosticDescriptor(DiagnosticId, title, multipleMembersMessageFormat, category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(singlePropertyRule, multiplePropertyRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSemanticModelAction(analyzeSemantic);
        }

        private static void analyzeSemantic(SemanticModelAnalysisContext context)
        {
            var model = context.SemanticModel;
            IEnumerable<ObjectCreationExpressionSyntax> creations = findCreations(model);

            foreach (var creation in creations)
            {
                var createdType = model.GetTypeInfo(creation);

                List<string> requiredMembers = getRequiredMembers(createdType).ToList();
                findUsedMembers(creation).ForEach(m => requiredMembers.Remove(m));

                if (requiredMembers.Any())
                {
                    createDiagnostic(context, creation.GetLocation(), createdType.Type.Name, requiredMembers);
                }
            }
        }

        private static void createDiagnostic(SemanticModelAnalysisContext context, Location location, string typeName, List<string> requiredMembers)
        {
            DiagnosticDescriptor rule = singlePropertyRule;

            if (requiredMembers.Count > 1)
            {
                rule = multiplePropertyRule;
            }
            var diagnostic = Diagnostic.Create(rule, location, typeName, string.Join(", ", requiredMembers));

            context.ReportDiagnostic(diagnostic);
        }

        private static List<string> findUsedMembers(ObjectCreationExpressionSyntax creation)
        {
            List<string> usedMembers = new List<string>();
            if (creation.Initializer != null)
            {
                var initializer = creation.Initializer;

                if (initializer.Expressions.Any())
                {
                    var expressions = initializer.Expressions;

                    foreach (var expr in expressions)
                    {
                        if (expr is AssignmentExpressionSyntax assignment)
                        {
                            string propName = assignment.Left.GetFirstToken().ValueText;
                            usedMembers.Add(propName);
                        }

                    }
                }
            }

            return usedMembers;
        }

        private static IEnumerable<ObjectCreationExpressionSyntax> findCreations(SemanticModel model)
        {
            var root = model.SyntaxTree.GetRoot();
            var nodes = root.DescendantNodes(descendIntoChildren: n => n.IsKind(SyntaxKind.ObjectCreationExpression) == false);
            var creations = nodes.Where(n => n.IsKind(SyntaxKind.ObjectCreationExpression)).Cast<ObjectCreationExpressionSyntax>();
            return creations;
        }

        private static IEnumerable<string> getRequiredMembers(TypeInfo createdType)
        {
            foreach (var member in createdType.Type.GetMembers())
            {
                if (member is IPropertySymbol || member is IFieldSymbol)
                {
                    foreach (AttributeData attrData in member.GetAttributes())
                    {
                        if (isProperAttribute(attrData))
                        {
                            yield return member.Name;
                        }

                    }
                }
            }
        }

        private static bool isProperAttribute(AttributeData attrData)
        {
            SymbolDisplayFormat displayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

            string fullName = attrData.AttributeClass.ToDisplayString(displayFormat);

            return fullName == "RandomAnalyzers.RequiredMember.RequiredMemberAttribute";
        }
    }
}
