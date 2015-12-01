using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetPumpCodePlug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
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
        public extern static IntPtr WindowFromPoint(int x, int y);  

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int EnumThreadWindows(int dwThreadId, EnumWindowsProc lpfn, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc lpfn, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr EnumChildWindows(int hWndParent, EnumWindowsProc lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        static extern IntPtr ChildWindowFromPoint(int hWndParent, Point pt);

        [DllImport("user32.dll")]
        static extern IntPtr GetClassName(int hWndParent, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(int hwnd, out  Rect lpRect);
        [DllImport("user32.dll")]
        private static extern int GetParent(int hwnd);
        [DllImport("user32.dll")]
        private static extern int GetWindow(int hwnd, int index);
        [DllImport("user32.dll")]
        private static extern bool PtInRect(ref Rect rect, Point pt);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(int hwnd);

        public delegate bool EnumWindowsProc(int hWnd, int lParam);


        [DllImport("GetPumpCodeDll.dll", EntryPoint = "GetPumpNumber", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPumpNumber(StringBuilder lpPumpStringBuilder, int nMaxCount);
        [DllImport("GetPumpCodeDll.dll", EntryPoint = "GetPumpUintCode", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPumpUintCode(StringBuilder lpPumpStringBuilder, int nMaxCount);


        public MainWindow()
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder(256);
            Console.WriteLine("Handle:{0:X}", GetPumpNumber(sb, 256));
            Console.WriteLine(sb);
            sb = new StringBuilder(256);
            Console.WriteLine("Handle:{0:X}", GetPumpUintCode(sb, 256));
            Console.WriteLine(sb);

            var lv =
                System.Windows.Forms.Control.FromHandle(new IntPtr(GetPumpUintCode(sb, 256)));
//             Console.WriteLine(lv.Handle);
//             IntPtr hwnd = FindWindow(null, "基本信息版本V1.2");
//             Console.WriteLine("Handle:{0:X}", hwnd.ToInt32());
//             Point pt = new Point(0, 0);
//             for (int i = 0; i < 100; i= i + 5)
//             {
//                 pt.y = 0;
//                 for (int j = 0; j < 100; j = j + 20)
//                 {
//                     pt.x += i;
//                     pt.y += j;
//                     int h = SmallestWindowFromPoint(hwnd.ToInt32(), pt);
//                     StringBuilder sb = new StringBuilder(256);
//                     GetWindowText(h, sb, 256);
//                     Console.WriteLine("Handle:{0:X}, Text:{1}", hwnd, sb);
//                 }
//             }
//             SmallestWindowFromPoint(hwnd.ToInt32(), pt);
//             EnumChildWindows(hwnd.ToInt32(), EnumChildWindow, 0);
//             IntPtr pthwnd = ChildWindowFromPoint(hwnd.ToInt32(), 100, 100);
//             Console.WriteLine(pthwnd);
//             StringBuilder str = new StringBuilder(256);
//             GetWindowText(pthwnd.ToInt32(), str, 256);
//             Console.WriteLine(str);
//             GetClassName(pthwnd.ToInt32(), str, 256);
//             Console.WriteLine(str);
        }

        private int SmallestWindowFromPoint(int hwnd, Point ptPoint)
        {
            int hWnd = ChildWindowFromPoint(hwnd, ptPoint).ToInt32();
            if (hWnd != 0)
            {
                Rect rect = new Rect();
                GetWindowRect(hWnd, out rect);
                int hparent = GetParent(hWnd);
                if (hparent != 0)
                {
                    int htemp = hWnd;
                    do
                    {
                        htemp = GetWindow(htemp, 2);
                        Rect rcTemp = new Rect();
                        GetWindowRect(htemp, out rcTemp);
                        if (PtInRect(ref rect, ptPoint) && GetParent(htemp) == hparent && IsWindowVisible(htemp))
                        {
                            if (((rcTemp.Right - rcTemp.Left)*(rcTemp.Bottom - rcTemp.Top)) <
                                ((rect.Right - rect.Left)*(rect.Bottom - rect.Top)))
                            {
                                hWnd = htemp;
						        GetWindowRect(hWnd, out rect);
                            }
                        }
                    } while (htemp != 0);
                }

            }
            Console.WriteLine("Handle:{0:X}", hWnd);
            return hWnd;
        }

        private bool EnumChildWindow(int hwnd, int lparam)
        {
            if (hwnd != 0)
            {
                StringBuilder str = new StringBuilder(256);
                GetWindowText(hwnd, str, 256);
                Console.WriteLine("Handle:{0:X}, Text:{1}", hwnd, str);
            }
            return true;
        }
    }
}
