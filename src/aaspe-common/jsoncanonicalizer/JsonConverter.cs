using System.Globalization;

namespace aaspe_common.jsoncanonicalizer;

/// <summary>
/// Provides functionality to convert JSON data.
/// </summary>
internal static class JsonConverter
{
    /// <summary>
    /// Converts a JSON string representing a number to a double value.
    /// </summary>
    /// <param name="number">The JSON string representation of the number to convert.</param>
    /// <returns>The double value represented by the input JSON string.</returns>
    /// <exception cref="FormatException">Thrown if the input JSON string is not in a valid format for a double number.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the input JSON string is null.</exception>
    public static double ToDouble(string number)
    {
        return double.Parse(number, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}