using System.Text.RegularExpressions;

namespace SummonsTracker.Text
{
    public static class TextUtils
    {
        public static string AddPlus(int value, bool addBrackets = false)
        {
            var val = value >= 0 ? $"+{value}" : value.ToString();

            return addBrackets ? $"({val})" : val;
        }
        public static string DeCamelCase(string text)
        {
            return Regex.Replace(text, @"\p{Lu}", m => " " + m.Value).Trim();
        }
    }
}