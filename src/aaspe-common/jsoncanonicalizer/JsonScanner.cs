namespace aaspe_common.jsoncanonicalizer
{
    internal class JsonScanner
    {
        private int _currentIndex;
        private readonly string _jsonData;

        public JsonScanner(string jsonData)
        {
            _jsonData = jsonData ?? throw new ArgumentNullException(nameof(jsonData));
        }

        public char PeekNextNonWhiteSpaceCharacter()
        {
            var savedIndex = _currentIndex;
            var nextChar = Scan();
            _currentIndex = savedIndex;
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
            return char.IsWhiteSpace(_jsonData[_currentIndex++]);
        }

        public void MoveBackToPreviousCharacter()
        {
            if (_currentIndex <= 0)
            {
                _currentIndex = 0;
            }

            _currentIndex--;
        }
    }
}