using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emilia.Flow.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Emilia.Flow.SourceGenerator
{
    [Generator]
    public class NodeGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            SyntaxReceiver receiver = context.SyntaxReceiver as SyntaxReceiver;
            if (receiver == null) return;

            Compilation compilation = context.Compilation;

            foreach (ClassDeclarationSyntax classDeclaration in receiver.CandidateClasses)
            {
                SemanticModel model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);

                string namespaceName = classDeclaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>().Name.ToString();
                string className = classDeclaration.Identifier.Text;

                if (classDeclaration.TypeParameterList != null)
                {
                    className += "<";
                    int count = classDeclaration.TypeParameterList.Parameters.Count;
                    for (var i = 0; i < count; i++)
                    {
                        TypeParameterSyntax typeParameter = classDeclaration.TypeParameterList.Parameters[i];
                        if (i == count - 1) className += typeParameter.Identifier.Text;
                        else className += typeParameter.Identifier.Text + ",";
                    }

                    className += ">";
                }

                List<IPropertySymbol> valueProperties = new List<IPropertySymbol>();
                List<IMethodSymbol> valueMethods = new List<IMethodSymbol>();

                List<IMethodSymbol> methods = new List<IMethodSymbol>();
                List<IMethodSymbol> methodArgs = new List<IMethodSymbol>();

                foreach (PropertyDeclarationSyntax propertyDeclaration in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
                {
                    IPropertySymbol propertySymbol = model.GetDeclaredSymbol(propertyDeclaration);
                    if (propertySymbol != null && propertySymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(FlowOutputValuePort))) valueProperties.Add(propertySymbol);
                }

                foreach (MethodDeclarationSyntax methodDeclaration in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    IMethodSymbol methodSymbol = model.GetDeclaredSymbol(methodDeclaration);
                    if (methodSymbol != null &&
                        methodSymbol.GetAttributes().Any(attr =>
                            attr.AttributeClass?.Name == nameof(FlowInputMethodPort) ||
                            attr.AttributeClass?.Name == nameof(FlowOutputMethodPort)))
                    {
                        if (methodSymbol.ReturnsVoid == false) valueMethods.Add(methodSymbol);
                        else
                        {
                            if (methodSymbol.Parameters.Length > 0) methodArgs.Add(methodSymbol);
                            else methods.Add(methodSymbol);
                        }
                    }
                }

                if (valueProperties.Count <= 0 && valueMethods.Count <= 0 && methods.Count <= 0) continue;

                var source = GenerateClassWithDictionary(namespaceName, className, methods, methodArgs, valueProperties, valueMethods);
                context.AddSource($"{classDeclaration.Identifier.Text}_Generated.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateClassWithDictionary(string namespaceName, string className, List<IMethodSymbol> methods, List<IMethodSymbol> methodArgs, List<IPropertySymbol> valueProperties,
            List<IMethodSymbol> valueMethods)
        {
            StringBuilder sourceBuilder = new StringBuilder($@"
using System;
using System.Collections.Generic;

namespace {namespaceName}
{{
    public partial class {className}
    {{
        protected override void InitMethodCache()
        {{
            base.InitMethodCache();
");
            foreach (IPropertySymbol property in valueProperties) sourceBuilder.AppendLine($@"            getValueCache[""{property.Name}""] = () => {property.Name};");
            foreach (IMethodSymbol method in valueMethods) sourceBuilder.AppendLine($@"            getValueCache[""{method.Name}""] = {method.Name};");

            foreach (IMethodSymbol method in methods) sourceBuilder.AppendLine($@"            methodCaches[""{method.Name}""] = (_) => {method.Name}();");
            foreach (IMethodSymbol method in methodArgs) sourceBuilder.AppendLine($@"            methodCaches[""{method.Name}""] = {method.Name};");

            sourceBuilder.AppendLine($@"
        }}
    }}
}}
");

            return sourceBuilder.ToString();
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclaration &&
                    classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) &&
                    classDeclaration.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .Any(attr => attr.Name.ToString() == nameof(FlowNodeGenerator)))
                {
                    CandidateClasses.Add(classDeclaration);
                }
            }
        }
    }
}