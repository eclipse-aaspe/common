namespace aaspe_common.jsoncanonicalizer;

/// <summary>
/// Interface for scanning JSON strings.
/// </summary>
internal interface IJsonScanner
{
    /// <summary>
    /// Peeks at the next non-whitespace character in the JSON string without advancing the current index.
    /// </summary>
    /// <returns>The next non-whitespace character.</returns>
    char PeekNextNonWhiteSpaceCharacter();

    /// <summary>
    /// Scans for the next character in the JSON string and validates it against the expected character.
    /// </summary>
    /// <param name="expected">The character to validate against.</param>
    /// <exception cref="IOException">Thrown when the next character does not match the expected character.</exception>
    void ScanForNextCharacter(char expected);

    /// <summary>
    /// Retrieves the next character in the JSON string and advances the current index.
    /// </summary>
    /// <returns>The next character.</returns>
    /// <exception cref="IOException">Thrown when the end of the JSON string is reached unexpectedly.</exception>
    char GetNextCharacter();

    /// <summary>
    /// Scans the JSON string for the next character and returns it without advancing the current index.
    /// </summary>
    /// <returns>The next character.</returns>
    /// <exception cref="IOException">Thrown when the end of the JSON string is reached unexpectedly.</exception>
    char Scan();

    /// <summary>
    /// Checks if the current index is within the bounds of the JSON string length.
    /// </summary>
    /// <returns>True if the current index is within the JSON string length, otherwise false.</returns>
    bool IsIndexWithinJsonLength();

    /// <summary>
    /// Checks if the next character in the JSON string is whitespace.
    /// </summary>
    /// <returns>True if the next character is whitespace, otherwise false.</returns>
    bool IsNextCharacterWhiteSpace();

    /// <summary>
    /// Moves the current index back to the previous character in the JSON string.
    /// </summary>
    void MoveBackToPreviousCharacter();

    /// <summary>
    /// Moves the current index to the next character in the JSON string.
    /// Sets index to length of JSON, if the end is reached.
    /// </summary>
    void MoveToNextCharacter();
}