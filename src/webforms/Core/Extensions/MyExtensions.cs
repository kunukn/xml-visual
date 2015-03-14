using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Kunukn.XmlVisual.Core.Entities;
using Kunukn.XmlVisual.Core.Extensions;
using Kunukn.XmlVisual.Core.Utilities;

namespace Kunukn.XmlVisual.Core.Extensions
{
    public static class MyExtensions
    {
        const string S = "G";
        private static readonly CultureInfo daDK = new CultureInfo("da-DK");
        private static readonly CultureInfo enUS = new CultureInfo("en-US");

/*
        public static void Add<T>(this T entity) where T : ex3.Program.MyList<T>
        {
            entity.Add(entity);
        }*/

        public static int ToIntRounded(this double d)
        {
            return (int) (Math.Round(d));
        }
        public static int ToInt(this double d)
        {
            return (int) d;
        }

        public static VisualizationType ToVisualizationType(this int i)
        {
            return (VisualizationType)Enum.ToObject(typeof(VisualizationType), i);
        }

        public static bool IsEqual(this double d, double other)
        {
            return Math.Abs(d - other) < MathTool.Epsilon;            
        }

        public static List<int> Sorted(this HashSet<int> set)
        {
            var list = set.ToList();
            list.Sort();
            return list;
        }

        public static string Show<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder();
            foreach (var i in enumerable)
                sb.Append(i + " ");
            return sb.ToString();         
        }
        public static string GetString<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var i in enumerable)
                sb.Append(i + Environment.NewLine);
            return sb.ToString().RemoveLastNewline();
        }


        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.Count() == 0;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> current, IEnumerable<T> list)
        {
            return list.All(i => current.Contains(i));
        }

        public static bool ContainsNone<T>(this IEnumerable<T> current, IEnumerable<T> list)
        {
            return !list.Any(i => current.Contains(i));
        }


        public static void Print<T>(this IEnumerable<T> enumerable)
        {
            Print(enumerable, null);           
        }
        public static void Print<T>(this IEnumerable<T> enumerable, string header)
        {
            if( !string.IsNullOrEmpty(header))
                Console.WriteLine(header);

            if (enumerable == null)
                Console.WriteLine("[null]");
            else if(enumerable.Count()==0)
                Console.WriteLine("[empty]");
            else
                foreach (var i in enumerable)
                    Console.WriteLine(i);
        }

     
                   
        public static string P(this Object o) // pretty print
        {
            const string invalid = "?";
            
            if(o is string)
                return string.IsNullOrEmpty(((string)o)) ? invalid : o.ToString();

            if (o is double?)            
                return ((double?) o).Format();
                            
            var result = (o ?? invalid).ToString(); 
            if(result=="invalid")
                result = "?";
            return result;
        }

        public static bool IsValid(this Object o)
        {
            if(o == null)
                return false;

            if (o is string)            
                return ! string.IsNullOrEmpty(((string) o));
            
            return true;
        }
            
        public static string Format(this double d)
        {
            return FormatDk((double?)d);
        }
        public static string FormatUs(this double d)
        {
            return FormatUs((double?)d);
        }
        public static string Format(this double? d)
        {
            return FormatDk(d);            
        }
        public static string FormatDk(this double? d)
        {
            if (d.HasValue)
            {
                var r = Math.Round(d.Value, 2);
                return r.ToString(S, daDK);
            }
            return "?";
        }
        public static string FormatUs(this double? d)
        {
            if (d.HasValue)
            {
                var r = Math.Round(d.Value, 2);
                return r.ToString(S, enUS);
            }
            return "?";
        }

        public static double Round(this double d)
        {           
                return Math.Round(d, MathTool.ROUND);                            
        }


        // does not work, only with return
/*        public static List<T> Put<T>(this List<T> list, T t) 
        {
            if(list==null) // lazy init
                list = new List<T>();
            list.Add(t);
            return list;
        }*/


        // Extension method, string to enum
        public static Entities.XmlType ToXmlTypeEnum(this string str)
        {            
            if (Enum.IsDefined(typeof(XmlType), str))            
                return (XmlType)Enum.Parse(typeof(XmlType), str, true);            
            return XmlType.Unknown;
        }
        public static Entities.VisualizationType ToVisualizationTypeEnum(this string str)
        {
            if (Enum.IsDefined(typeof(VisualizationType), str))
                return (VisualizationType)Enum.Parse(typeof(VisualizationType), str, true);
            return VisualizationType.Unknown;
        }
        public static Entities.ApplicationMode ApplicationModeEnum(this string str)
        {
            if (Enum.IsDefined(typeof(ApplicationMode), str))
                return (ApplicationMode)Enum.Parse(typeof(ApplicationMode), str, true);
            return ApplicationMode.Unknown;
        }


        public static string GetString(this XmlType type)
        {
            return Enum.GetName(typeof(XmlType), type);
        }
        public static string GetString(this VisualizationType type)
        {
            return Enum.GetName(typeof(VisualizationType), type);
        }

        public static string GetString(this ApplicationMode type)
        {
            return Enum.GetName(typeof(ApplicationMode), type);
        }

        
    }
}
