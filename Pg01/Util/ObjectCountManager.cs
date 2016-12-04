#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#endregion

namespace Pg01Util
{
    public static class ObjectCountManager
    {
        private static readonly Dictionary<Type, int> Counter =
            new Dictionary<Type, int>();

        #region Private Functions

        public static void Dump()
        {
            var sb = new StringBuilder();
            sb.Append("[ObjectCount] ");
            foreach (var i in Counter)
            {
                sb.Append($"{i.Key}: {i.Value:000}  ");
            }
            Debug.WriteLine(sb.ToString());
        }

        #endregion

        #region Public Functions

        public static void CountUp(Type type)
        {
            if (!Counter.ContainsKey(type))
            {
                Counter[type] = 0;
            }
            Counter[type] += 1;
            Dump();
        }

        public static void CountDown(Type type)
        {
            if (!Counter.ContainsKey(type))
            {
                Counter[type] = 0;
            }
            Counter[type] -= 1;
            Dump();
        }

        #endregion
    }
}