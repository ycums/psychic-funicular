using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Pg01.Models.Util
{
    internal static class Util
    {
        public static string ConvertToString<T>(T value)
        {
            return TypeDescriptor.GetConverter(typeof(T)).ConvertToString(value);
        }

        public static T ConvertFromString<T>(string value)
        {
            return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        public static bool Like(string wName, string p)
        {
            var regexPattern = Regex.Replace(
                p,
                ".",
                m =>
                {
                    var s = m.Value;
                    if (s.Equals("?"))
                        return ".";
                    if (s.Equals("*"))
                        return ".*";
                    return Regex.Escape(s);
                }
            );

            return new Regex(regexPattern).IsMatch(wName);
        }

        public static bool Between<T>(this T current, T lower, T higher, bool inclusive = true) where T : IComparable
        {
            if (lower.CompareTo(higher) > 0) Swap(ref lower, ref higher);

            return inclusive
                ? (lower.CompareTo(current) <= 0) && (current.CompareTo(higher) <= 0)
                : (lower.CompareTo(current) < 0) && (current.CompareTo(higher) < 0);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        ///     ディープコピーを作成する。
        ///     クローンするクラスには SerializableAttribute 属性、
        ///     不要なフィールドは NonSerializedAttribute 属性をつける。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T CloneDeep<T>(this T target)
        {
            object clone;
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, target);
                stream.Position = 0;
                clone = formatter.Deserialize(stream);
            }
            return (T) clone;
        }

        public static Color GetDisplayForeColor(Color value)
        {
            return value == Colors.Transparent
                ? Colors.Black
                : (GetLuma(value) <= 127 ? Colors.White : Colors.Black);
        }

        public static int GetLuma(Color c)
        {
            return (int) (c.R*0.3 + c.G*0.59 + c.B*0.11);
        }
    }
}