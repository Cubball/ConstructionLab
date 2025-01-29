using System.Globalization;
using System.Text;
using Core.Models;

namespace Core.CodeGen;

// TODO: public?
internal class CodeGenerator
{
    private const string DictionaryName = "dict";

    public static string Generate(List<IVisitable> firstBlocks)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"var {DictionaryName} = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();");
        var variablesVisitor = new VariablesVisitor();
        var labelsVisitor = new LabelVisitor();
        foreach (var block in firstBlocks)
        {
            block.Accept(variablesVisitor);
            block.Accept(labelsVisitor);
        }

        foreach (var variable in variablesVisitor.Variables)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"{DictionaryName}[\"{variable}\"] = 0;");
        }

        foreach (var block in firstBlocks)
        {
            var codeGenVisitor = new CodeGenVisitor(labelsVisitor.Labels, DictionaryName);
            block.Accept(codeGenVisitor);
            sb.AppendLine("new System.Threading.Thread(() => {");
            sb.Append(codeGenVisitor.Code);
            sb.AppendLine("}).Start();");
        }

        return sb.ToString();
    }
}