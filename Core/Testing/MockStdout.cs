using System.Globalization;
using System.Text;

namespace Core.Testing;

internal sealed class MockStdout : TextWriter
{
    public List<string> Lines { get; } = [];

    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(int value)
    {
        Lines.Add(value.ToString(CultureInfo.InvariantCulture));
    }

    public override void WriteLine(string? value)
    {
        Lines.Add(value ?? string.Empty);
    }
}