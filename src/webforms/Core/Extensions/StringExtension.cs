using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kunukn.XmlVisual.Core.Extensions
{
    /// <summary>
    /// KN
    /// </summary>
    public static class StringExtension
    {
        // Remove multiple whitespaces to one
        public static string OneWhiteSpace(this string str)
        {
//            var sb = new StringBuilder();
            str = Regex.Replace(str, @"[\s]", @" ");
            str = Regex.Replace(str, @"[ ]{2,}", @" ");
            if (str.StartsWith(" "))
                str = str.Substring(1);
            if (str.EndsWith(" "))
                str = str.Substring(0,str.Length-1);
            return str;
        }

        public static string RemoveChar(this string str, char c)
        {
            return str.Replace(c.ToString(), string.Empty);
        }

        public static string RemoveString(this string str, string s)
        {
            return str.Replace(s, string.Empty);
        }

        public static DateTime ToDateTime(this string str, string format)
        {
            return DateTime.ParseExact(str, format, CultureInfo.InvariantCulture);
        }

        public static float ToFloat(this string str)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            return float.Parse(str, culture);
        }

        public static double ToDouble(this string str)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            return Double.Parse(str, culture);
        }

        public static string RemoveBlings(this String str)
        {
            return str.Replace("\"", string.Empty).Replace("'", string.Empty);
        }

        public static string BlingsChange(this String str)
        {
            return str.Replace("'", "\"");
        }

        public static void RemoveLastNewline(this StringBuilder sb)
        {
            int len = Environment.NewLine.Length;
            bool isNewline = sb.ToString().EndsWith(Environment.NewLine);
            if (isNewline)
                sb.Remove(sb.Length - len, len);
        }

        public static string RemoveLastNewline(this string str)
        {
            int len = Environment.NewLine.Length;
            bool isNewline = str.EndsWith(Environment.NewLine);
            if (isNewline)
                return str.Remove(str.Length - len, len);
            return str;
        }
    }
}
