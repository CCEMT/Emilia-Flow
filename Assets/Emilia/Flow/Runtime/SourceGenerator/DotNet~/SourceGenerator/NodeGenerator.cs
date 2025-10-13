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

                List<IMethodSymbol> valueMethods = new List<IMethodSymbol>();
                List<IMethodSymbol> valueMethodArgs = new List<IMethodSymbol>();

                List<IMethodSymbol> methods = new List<IMethodSymbol>();
                List<IMethodSymbol> methodArgs = new List<IMethodSymbol>();

                foreach (MethodDeclarationSyntax methodDeclaration in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    IMethodSymbol methodSymbol = model.GetDeclaredSymbol(methodDeclaration);
                    if (methodSymbol == null) continue;

                    if (methodSymbol.GetAttributes().Any(attr =>
                            attr.AttributeClass?.Name == nameof(FlowInputMethodPort) ||
                            attr.AttributeClass?.Name == nameof(FlowOutputMethodPort)))
                    {
                        if (methodSymbol.ReturnsVoid)
                        {
                            if (methodSymbol.Parameters.Length > 0) methodArgs.Add(methodSymbol);
                            else methods.Add(methodSymbol);
                        }
                    }

                    if (methodSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(FlowOutputValuePort)))
                    {
                        if (methodSymbol.ReturnsVoid == false)
                        {
                            if (methodSymbol.Parameters.Length > 0) valueMethodArgs.Add(methodSymbol);
                            else valueMethods.Add(methodSymbol);
                        }
                    }
                }

                if (valueMethods.Count <= 0 && valueMethodArgs.Count <= 0 && methods.Count <= 0 && methodArgs.Count <= 0) continue;

                var usings = classDeclaration.SyntaxTree.GetRoot()
                    .DescendantNodes()
                    .OfType<UsingDirectiveSyntax>()
                    .Select(u => u.ToString())
                    .ToList();

                var source = GenerateClassWithDictionary(namespaceName, className, methods, methodArgs, valueMethods, valueMethodArgs, usings);
                context.AddSource($"{classDeclaration.Identifier.Text}_Generated.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string GenerateClassWithDictionary(string namespaceName, string className,
            List<IMethodSymbol> methods,
            List<IMethodSymbol> methodArgs,
            List<IMethodSymbol> values,
            List<IMethodSymbol> valueArgs,
            List<string> usings)
        {
            StringBuilder sourceBuilder = new StringBuilder();
            
            foreach (var usingDirective in usings)
            {
                sourceBuilder.AppendLine(usingDirective);
            }
            
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"namespace {namespaceName}");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine($"    public partial class {className}");
            sourceBuilder.AppendLine("    {");
            sourceBuilder.AppendLine("        protected override void InitMethodCache()");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            base.InitMethodCache();");
            
            foreach (IMethodSymbol method in values) 
                sourceBuilder.AppendLine($@"            getValueCache[""{method.Name}""] = (_) => {method.Name}();");
            foreach (IMethodSymbol method in valueArgs) 
                sourceBuilder.AppendLine($@"            getValueCache[""{method.Name}""] = (arg) => (object) {method.Name}(({method.Parameters.FirstOrDefault().Type}) arg);");

            foreach (IMethodSymbol method in methods) 
                sourceBuilder.AppendLine($@"            methodCaches[""{method.Name}""] = (_) => {method.Name}();");
            foreach (IMethodSymbol method in methodArgs) 
                sourceBuilder.AppendLine($@"            methodCaches[""{method.Name}""] = (arg) => {method.Name}(({method.Parameters.FirstOrDefault().Type}) arg);");

            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

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