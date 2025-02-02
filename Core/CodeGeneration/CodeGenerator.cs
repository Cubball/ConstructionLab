using System.Globalization;
using System.Text;
using Core.Models;
using Core.Validation;

namespace Core.CodeGeneration;

// TODO: public?
internal class CodeGenerator
{
    private const string DictionaryName = "dict";

    public static string Generate(List<StartBlock> startBlocks)
    {
        Validator.Validate(startBlocks);
        var sb = new StringBuilder();
        sb.AppendLine($"var {DictionaryName} = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();");
        var variablesVisitor = new VariablesVisitor();
        var labelsVisitor = new LabelVisitor();
        foreach (var block in startBlocks)
        {
            block.Accept(variablesVisitor);
            block.Accept(labelsVisitor);
        }

        foreach (var variable in variablesVisitor.Variables)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"{DictionaryName}[\"{variable}\"] = 0;");
        }

        foreach (var block in startBlocks)
        {
            var codeGenVisitor = new CodeGeneratingVisitor(labelsVisitor.Labels, DictionaryName);
            block.Accept(codeGenVisitor);
            sb.AppendLine("new System.Threading.Thread(() =>");
            sb.AppendLine("{");
            sb.AppendLine("try");
            sb.AppendLine("{");
            sb.Append(codeGenVisitor.Code);
            sb.AppendLine("}");
            sb.AppendLine("catch");
            sb.AppendLine("{");
            sb.AppendLine("System.Console.WriteLine(\"An unexpected error occured\");");
            sb.AppendLine("}");
            sb.AppendLine("}).Start();");
        }

        return sb.ToString();
    }
}