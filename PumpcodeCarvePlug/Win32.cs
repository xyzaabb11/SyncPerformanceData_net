using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PumpcodeCarvePlug
{
    static  class Win32
    {
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [DllImport("user32.dll ")]
        public static extern IntPtr FindWindow(string classname, string title);
        [DllImport("user32.dll ")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll ")]
        public extern static IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll")]
        public static extern int EnumThreadWindows(int dwThreadId, EnumWindowsProc lpfn, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, Point pt);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWndParent, ref Rect lpRect);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClassName(IntPtr hWndParent, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        static extern int GetWindowRect(int hwnd, out  Rect lpRect);
        [DllImport("user32.dll")]
        static extern int GetParent(int hwnd);
        [DllImport("user32.dll")]
        static extern int GetWindow(int hwnd, int index);
        [DllImport("user32.dll")]
        static extern bool PtInRect(ref Rect rect, Point pt);
        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(int hwnd);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd,uint Msg,int wParam,int lParam);
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_MOUSEMOVE = 0x0200;
        public const uint WM_SETTEXT = 0x000C;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint VK_RETURN = 0x0D;
        public const uint VK_F5 = 0x74;
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}
