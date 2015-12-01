using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PumpcodeCarvePlug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
            System.Text.StringBuilder retVal, int size, string filePath);

        private List<Pump> pumps = new List<Pump>();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void PumpsearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            PumpunitListView.ItemsSource = null;
            StringBuilder remoteip = new StringBuilder(255);
            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
            String mysqlStr = "server=" + remoteip + ";uid=pump;pwd=pumpszmmcd;database=pump_performance";
            var mysql = new SqlConnection(mysqlStr);
            try
            {
                var sql = "select * from pump_code1 where pumpno like '%" + pumpcode.Text + "%'";
                mysql.Open();
                var cmd = mysql.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    pumps.Clear();
                    if (reader.Read())
                    {
                        Pump pump = new Pump();
                        pump.PumpNo = reader.GetString(3);
                        pump.PumpUnit = reader.GetString(4);
                        pump.PumpCode = reader.GetString(5).Replace(" ", "");
                        pump.TankNo = "1";
                        pumps.Add(pump);
                        pump = new Pump();
                        pump.PumpNo = reader.GetString(3);
                        pump.PumpUnit = reader.GetString(6);
                        pump.PumpCode = reader.GetString(7).Replace(" ", "");
                        pump.TankNo = "2";
                        pumps.Add(pump);
                        pump = new Pump();
                        pump.PumpNo = reader.GetString(3);
                        pump.PumpUnit = reader.GetString(8);
                        pump.PumpCode = reader.GetString(9).Replace(" ", "");
                        pump.TankNo = "3";
                        pumps.Add(pump);
                        pump = new Pump();
                        pump.PumpNo = reader.GetString(3);
                        pump.PumpUnit = reader.GetString(10);
                        pump.PumpCode = reader.GetString(11).Replace(" ", "");
                        pump.TankNo = "4";
                        pumps.Add(pump);

                    }

                    PumpunitListView.ItemsSource = pumps;
                    PumpunitListView.SelectedIndex = 0;
                    pumpcode.Text = pumps[0].PumpNo;
                }
                reader.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message /*"打开本地数据库失败，请确认配置中指向的本地数据库是否存在"*/, "错误", MessageBoxButton.OK);
            }
            finally
            {
                mysql.Close();
                //                 StartButton_OnClick(this, new RoutedEventArgs());
            }

        }

        private void MarkButton_OnClick(object sender, RoutedEventArgs e)
        {
            IntPtr mainPtr = FindWindowByName("激光标刻");
            if (mainPtr == IntPtr.Zero)
            {
                return;
            }
            StringBuilder lBuilder= new StringBuilder(255);
            Win32.GetClassName(mainPtr, lBuilder, 255);
            Console.WriteLine();
            IntPtr childPtr = FindChildWindow(mainPtr, "TPanel4");
//             Win32.Point pt;
//             pt.x = 10;
//             pt.y = 200;
//             IntPtr panelhwnd = Win32.ChildWindowFromPoint(mainPtr, pt);
            Console.WriteLine(mainPtr + "'" + childPtr);
            Win32.PostMessage(childPtr, Win32.WM_MOUSEMOVE, 1, 0x1bd0009);
            Win32.PostMessage(childPtr, Win32.WM_LBUTTONDOWN, 1, 0x1bd0009);
            Win32.PostMessage(childPtr, Win32.WM_LBUTTONUP, 0, 0x1bd0009);
            for (int kk = 0; kk < 100; ++kk)
            {
                Thread.Sleep(20);
                IntPtr objectHwnd = FindWindowByName("对象窗口");
                if (objectHwnd != IntPtr.Zero)
                {
                    IntPtr hchild = FindChildWindowByClass(objectHwnd, "TMemo");
                    if (hchild != IntPtr.Zero)
                    {
                        StringBuilder l = new StringBuilder(255);
                        Win32.GetWindowText(hchild, l, 255);
                        var pump = PumpunitListView.SelectedItem as Pump;
                        if (pump != null)
                        {
                            string text = pump.PumpCode.Replace(" ", "");
                            Win32.SendMessage(hchild, Win32.WM_SETTEXT, IntPtr.Zero, text);
                        }
                        Win32.PostMessage(objectHwnd, Win32.WM_KEYDOWN, (int)Win32.VK_RETURN, 1);
                        Thread.Sleep(500);
                        Win32.PostMessage(mainPtr, Win32.WM_KEYDOWN, (int)Win32.VK_F5, 1);
                        break;
                    }
                }

            }
            if (PumpunitListView.SelectedIndex == PumpunitListView.Items.Count - 1)
            {
                SetDataMarked();
                FocusInputBox_OnExecuted(this, null);
            }
            else
            {
                PumpunitListView.SelectedIndex++;
            }
        }

        private class Pump
        {
            public string PumpNo { get; set; }
            public string TankNo { get; set; }
            public string PumpUnit { get; set; }
            public string PumpCode { get; set; }
        }

        private void SetDataMarked()
        {
            StringBuilder remoteip = new StringBuilder(255);
            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
            String mysqlStr = "server=" + remoteip + ";uid=pump;pwd=pumpszmmcd;database=pump_performance";
            var mysql = new SqlConnection(mysqlStr);
            try
            {
                //var sql = "select * from pump_code1 where pumpno like '%" + pumpcode.Text + "%'";
                var sql = "update pump_code1 set isprint = 1, printdate = '"+DateTime.Now +"' where pumpno ='" + pumpcode.Text + "'";
                mysql.Open();
                var cmd = mysql.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message /*"打开本地数据库失败，请确认配置中指向的本地数据库是否存在"*/, "错误", MessageBoxButton.OK);
            }
            finally
            {
                mysql.Close();
            }
        }
        private void FocusInputBox_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            pumpcode.Focus();
            pumpcode.SelectAll();
        }

        private void UpSelect_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (PumpunitListView.SelectedIndex == 0)
            {
                PumpunitListView.SelectedIndex = PumpunitListView.Items.Count - 1;
            }
            else
            {
                PumpunitListView.SelectedIndex--;
            }
        }

        private void DownSelect_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (PumpunitListView.SelectedIndex == PumpunitListView.Items.Count - 1)
            {
                PumpunitListView.SelectedIndex = 0;
            }
            else
            {
                PumpunitListView.SelectedIndex++;
            }
        }

        private struct TableParam
        {
            public Int32 hChild;
            public StringBuilder SzBuilder;
            public bool bText;
        }

        public delegate bool EnumWindowsProc(int hWnd, int lParam);

        private IntPtr FindWindowByName(string winText)
        {
            return EnumWindowByName(delegate(IntPtr hwnd, IntPtr lparam)
            {
                StringBuilder lText = new StringBuilder(255);
                Win32.GetWindowText(hwnd, lText, 255);
                
                return lText.ToString().Contains(winText);
            });
        }


        private IntPtr EnumWindowByName(Win32.EnumWindowsProc filter)
        {
            IntPtr WindowHwnd = IntPtr.Zero;
            Win32.EnumWindows(delegate(IntPtr hwnd, IntPtr lparam)
            {
                if (filter(hwnd, lparam))
                {
                    WindowHwnd = hwnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return WindowHwnd;
        }

        private IntPtr FindChildWindowByClass(IntPtr parent, string winClass)
        {
            return EnumChildWindowByName(parent, delegate(IntPtr hwnd, IntPtr lparam)
            {
                StringBuilder lText = new StringBuilder(255);
                Win32.GetClassName(hwnd, lText, 255);
                return lText.ToString() == winClass;
            });
        }
        private IntPtr FindChildWindow(IntPtr parent,string winClass)
        {
            return EnumChildWindowByName(parent, delegate(IntPtr hwnd, IntPtr lparam)
            {
                StringBuilder lText = new StringBuilder(255);
                StringBuilder ll = new StringBuilder(255);
                Win32.GetClassName(hwnd, lText, 255);
                Win32.Rect rt = new Win32.Rect();
                Win32.GetWindowRect(hwnd, ref rt);
                if (lText.ToString() == "TPanel")
                {
                    Console.WriteLine(rt.Left + "," + rt.Top + "," + rt.Right + "," + rt.Bottom);
//                     if ((rt.Left == 1 && rt.Top == 129 && rt.Right == 30 && rt.Bottom == 791)
//                         || (rt.Left == 65 && rt.Top == 135 && rt.Right == 94 && rt.Bottom == 815))
                    if (((rt.Right - rt.Left) == 29) && ((rt.Bottom - rt.Top) == 662))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            });
        }
        private IntPtr EnumChildWindowByName(IntPtr parent, Win32.EnumWindowsProc filter)
        {
            IntPtr WindowHwnd = IntPtr.Zero;
            Win32.EnumChildWindows(parent, delegate(IntPtr hwnd, IntPtr lparam)
            {
                if (filter(hwnd, lparam))
                {
                    WindowHwnd = hwnd;
                    return false;
                }
                else
                {
                    EnumChildWindowByName(hwnd, filter);

                }
                return true;
            }, IntPtr.Zero);
            return WindowHwnd;
        }
    }
}
