using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tiler.runtime
{
    public static class WindowsShell
    {
        private delegate bool EnumWindowDelegate(IntPtr hWnd, IntPtr lParam);
            
        /// <summary>
        /// Returns a dictionary of process id to Window handles for that process
        /// </summary>
        /// <returns></returns>
        public static Dictionary<uint, List<IntPtr>> GetWindowsByProcessId()
        {
            Dictionary<uint, List<IntPtr>> result = new Dictionary<uint, List<IntPtr>>();
            GCHandle setHandle = GCHandle.Alloc(result);

            try
            {
                EnumDesktopWindows(IntPtr.Zero, GroupDesktopWindows, GCHandle.ToIntPtr(setHandle));
            }
            finally
            {
                if (setHandle.IsAllocated) setHandle.Free();
            }

            return result;
        }

        /// <summary>
        /// Internal Enum window delegate implementation
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam">A pointer to a Dictionary[uint, List[IntPtr]]</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">If the object referenced by lParam is not a dictionary</exception>
        private static bool GroupDesktopWindows(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gch = GCHandle.FromIntPtr(lParam);
            if (!(gch.Target is Dictionary<uint, List<IntPtr>> dict))
            {
                throw new InvalidCastException("GCHandle Target could not be cast as Dictionary<uint, List<IntPtr>>");
            }

            // Get the window's title.
            StringBuilder sbTitle = new StringBuilder(1024);
            GetWindowText(hWnd, sbTitle, sbTitle.Capacity);
            string title = sbTitle.ToString();

            // If the window is visible and has a title, save it.
            if (!IsWindowVisible(hWnd) || string.IsNullOrEmpty(title)) return true;
            
            GetWindowThreadProcessId(hWnd, out uint processId);
            Console.WriteLine("PROCESS ID Value " + processId + " Window Title is " +  title);
                
            if (dict.ContainsKey(processId))
            {
                dict[processId].Add(hWnd);
            }
            else
            {
                List<IntPtr> list = new List<IntPtr> {hWnd};
                dict.Add(processId, list);
            }

            // Return true to indicate that we
            // should continue enumerating windows.
            return true;
        }
        
        
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowDelegate lpEnumCallbackFunction,
            IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError=true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}