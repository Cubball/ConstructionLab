using System.Globalization;
using System.Text;

namespace Core.Testing;

internal class MockStdout : TextWriter
{
    public List<string> Lines { get; } = [];

    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(int value)
    {
        Lines.Add(value.ToString(CultureInfo.InvariantCulture));
    }
}