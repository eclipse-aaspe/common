namespace aaspe_common.jsoncanonicalizer;

/// <inheritdoc cref="IJsonScanner"/>
internal class JsonScanner : IJsonScanner
{
    private int _currentIndex;
    private readonly string _jsonData;

    public JsonScanner(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            throw new ArgumentNullException(nameof(jsonData));
        }

        _jsonData = jsonData;
    }

    public char PeekNextNonWhiteSpaceCharacter()
    {
        var savedIndex = _currentIndex;
        var nextChar = Scan();
        _currentIndex = savedIndex;
        return nextChar;
    }
    
    public char PeekNextCharacter()
    {
        var savedIndex = _currentIndex;
        if (savedIndex == _jsonData.Length)
        {
            return Convert.ToChar(string.Empty);
        }
        var nextChar = _jsonData[savedIndex+1];
        return nextChar;
    }

    public void ScanForNextCharacter(char expected)
    {
        var nextChar = Scan();
        if (nextChar != expected)
        {
            throw new IOException($"Expected '{expected}' but got '{nextChar}'");
        }
    }

    public char GetNextCharacter()
    {
        if (_currentIndex < _jsonData.Length)
        {
            return _jsonData[_currentIndex++];
        }

        throw new IOException("Unexpected EOF reached");
    }

    public char Scan()
    {
        char nextChar;
        while ((nextChar = GetNextCharacter()) != '\0')
        {
            if (!char.IsWhiteSpace(nextChar))
            {
                return nextChar;
            }
        }

        throw new IOException("Unexpected EOF reached");
    }

    public bool IsIndexWithinJsonLength()
    {
        return _currentIndex < _jsonData.Length;
    }

    public bool IsNextCharacterWhiteSpace()
    {
        return IsIndexWithinJsonLength() && char.IsWhiteSpace(PeekNextCharacter());
    }

    public void MoveBackToPreviousCharacter()
    {
        if (_currentIndex <= 0)
        {
            _currentIndex = 0;
        }
        else
        {
            _currentIndex--;
        }
    }

    public void MoveToNextCharacter()
    {
        if (_currentIndex < _jsonData.Length)
        {
            _currentIndex++;
        }

        _currentIndex = _jsonData.Length;
    }
}