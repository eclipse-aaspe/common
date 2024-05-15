internal class JsonScanner
{
    private int _index;
    private readonly string _jsonData;

    public JsonScanner(string jsonData)
    {
        _jsonData = jsonData;
    }

    public char PeekNextNonWhiteSpaceChar()
    {
        var save = _index;
        var c = Scan();
        _index = save;
        return c;
    }

    public void ScanFor(char expected)
    {
        var c = Scan();
        if (c != expected)
        {
            throw new IOException($"Expected '{expected}' but got '{c}'");
        }
    }

    public char GetNextChar()
    {
        if (_index < _jsonData.Length)
        {
            return _jsonData[_index++];
        }

        throw new IOException("Unexpected EOF reached");
    }

    public char Scan()
    {
        char c;
        while ((c = GetNextChar()) != '\0')
        {
            if (!char.IsWhiteSpace(c))
            {
                return c;
            }
        }

        throw new IOException("Unexpected EOF reached");
    }

    public bool IsIndexInJsonLength()
    {
        return _index < _jsonData.Length;
    }

    public bool IsNextCharacterWhitespace()
    {
        return char.IsWhiteSpace(_jsonData[_index++]);
    }

    public void RevertCurrentIndexByOne()
    {
        _index--;
    }
}