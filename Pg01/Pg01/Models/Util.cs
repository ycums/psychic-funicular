using System.ComponentModel;

namespace Pg01.Models
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
    }
}