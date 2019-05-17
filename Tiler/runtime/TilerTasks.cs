using System;

namespace Tiler.runtime
{
    internal static class TilerTasks
    {
        public static void ReArrangeWindows()
        {
            var dictionary = WindowsShell.GetWindowsByProcessId();
            Console.WriteLine(dictionary);
        }
    }
}