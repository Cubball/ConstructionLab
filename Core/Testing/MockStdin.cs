namespace Core.Testing;

internal class MockStdin(List<string> lines) : TextReader
{
    private readonly List<string> _lines = lines;

    private int _idx;

    public override string? ReadLine()
    {
        if (_idx >= _lines.Count)
        {
            return null;
        }

        return _lines[_idx++];
    }
}