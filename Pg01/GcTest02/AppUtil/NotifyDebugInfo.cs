using System;
using System.Diagnostics;

namespace GcTest.AppUtil

{

    static class NotifyDebugInfo

    {

        public static void WriteLine(string message)

        {

            var dispMessage = $"{DateTime.Now:HH:MM:ss} {message}";

            Debug.WriteLine(dispMessage);

        }

    }

}