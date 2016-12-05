#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

#endregion

namespace Pg01Util
{
    public static class ObjectCountManager
    {
        private static readonly Dictionary<Type, int> Counter =
            new Dictionary<Type, int>();

        private static readonly Semaphore Sem = new Semaphore(1, 1);

        #region Private Functions

        public static void Dump()
        {
            Sem.WaitOne();
            try
            {
                var sb = new StringBuilder();
                sb.Append("[ObjectCount] ");
                foreach (var i in Counter)
                {
                    sb.Append($"{i.Key}: {i.Value:000}  ");
                }
                Debug.WriteLine(sb.ToString());
            }
            finally
            {
                Sem.Release();
            }
        }

        #endregion

        #region Public Functions

        public static void CountUp(Type type)
        {
            Sem.WaitOne();
            try
            {
                if (!Counter.ContainsKey(type))
                {
                    Counter[type] = 0;
                }
                Counter[type] += 1;
            }
            finally
            {
                Sem.Release();
            }
            //Dump();
        }

        public static void CountDown(Type type)
        {
            Sem.WaitOne();
            try
            {
                if (!Counter.ContainsKey(type))
                {
                    Counter[type] = 0;
                }
                Counter[type] -= 1;
            }
            finally
            {
                Sem.Release();
            }
            //Dump();
        }

        #endregion
    }
}