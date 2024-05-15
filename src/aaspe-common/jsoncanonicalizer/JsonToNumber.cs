using System.Globalization;

namespace Org.Webpki.JsonCanonicalizer;

internal static class JsonToNumber
{
    public static double Convert(string number)
    {
        return double.Parse(number, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}