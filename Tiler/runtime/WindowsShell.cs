using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Tiler.runtime
{
    public static class WindowsShell
    {
        private delegate bool EnumWindowDelegate(IntPtr hWnd, IntPtr lParam);

        public static WindowPlacement GetPlacement(IntPtr hWnd)
        {
            var placement = new WindowPlacement();
            placement.Length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement;
        }

        public static WindowFrame GetWindowFrame(IntPtr hWnd)
        {
            GetWindowRect(hWnd, out var lpWindowRect);
            var lpWindowRectangle = lpWindowRect.ToRectangle();
            var baseWindowFrame = new WindowFrame(lpWindowRectangle, lpWindowRectangle);
            
            if (Environment.OSVersion.Version.Major < 6) return baseWindowFrame;

            var result = DwmGetWindowAttribute(hWnd, (int)Dwmwindowattribute.DwmwaExtendedFrameBounds, out var dwmRect, Marshal.SizeOf(typeof(RECT)));
            return result >= 0 ? new WindowFrame(lpWindowRectangle, dwmRect.ToRectangle()) : baseWindowFrame;
        }

        public static bool ChangeWindowPlacement(IntPtr hWnd, ref WindowPlacement lpWndPl) => SetWindowPlacement(hWnd, ref lpWndPl);

        /// <summary>
        /// Returns a dictionary of process id to Window handles for that process
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<IntPtr>> GetWindowsByProcessId()
        {
            var result = new Dictionary<int, List<IntPtr>>();
            var setHandle = GCHandle.Alloc(result);

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
            var gch = GCHandle.FromIntPtr(lParam);
            if (!(gch.Target is Dictionary<int, List<IntPtr>> dict))
            {
                throw new InvalidCastException("GCHandle Target could not be cast as Dictionary<uint, List<IntPtr>>");
            }

            // Get the window's title.
            var sbTitle = new StringBuilder(1024);
            GetWindowText(hWnd, sbTitle, sbTitle.Capacity);
            var title = sbTitle.ToString();

            // If the window is visible and has a title, save it.
            if (!IsWindowVisible(hWnd) || string.IsNullOrEmpty(title)) return true;

            GetWindowThreadProcessId(hWnd, out uint processId);
            Console.WriteLine("PROCESS ID Value " + processId + " Window Title is " + title);

            if (dict.ContainsKey((int) processId))
            {
                dict[(int) processId].Add(hWnd);
            }
            else
            {
                var list = new List<IntPtr> {hWnd};
                dict.Add((int) processId, list);
            }

            // Return true to indicate that we
            // should continue enumerating windows.
            return true;
        }

        [Flags]
        private enum Dwmwindowattribute
        {
            DwmwaExtendedFrameBounds = 9
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }
        }
        
        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }
        
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct WindowPlacement
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            
            private POINT ptMinPosition;
            private POINT ptMaxPosition;
            private RECT rcNormalPosition;

            public Rectangle NormalPosition
            {
                get => rcNormalPosition.ToRectangle();
                set => rcNormalPosition = new RECT(value.Left, value.Top, value.Right, value.Bottom);
            }
            
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine("Flags = " + Flags)
                  .AppendLine("NormalPosition = " + NormalPosition);
                return sb.ToString();
            }
        }

        public struct WindowFrame
        {
            public readonly Rectangle WindowRect;
            public readonly Rectangle DwmExtendedWindowRect;

            public WindowFrame(Rectangle windowRect, Rectangle dwmExtendedWindowRect)
            {
                WindowRect = windowRect;
                DwmExtendedWindowRect = dwmExtendedWindowRect;
            }

            public bool hasBorder() => !WindowRect.Equals(DwmExtendedWindowRect);
        }

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowDelegate lpEnumCallbackFunction,
            IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpWndPl);
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpWndPl);
        
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hWnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);
        
        [DllImport("user32.dll", SetLastError=true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    }
}