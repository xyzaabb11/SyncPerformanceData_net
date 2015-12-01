using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SyncPerformanceData
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        // 声明INI文件的写操作函数 WritePrivateProfileString()

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);


        private string inipath;
        public Setting()
        {
            InitializeComponent();
            StringBuilder sbBuilder = new StringBuilder(255);
            inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            GetPrivateProfileString("LOCALDB", "DBPath", "chengfeishujuku.mdb", sbBuilder, 255, inipath);
            localdb.Text = sbBuilder.ToString();
            GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", sbBuilder, 255, inipath);
            remoteip.Text = sbBuilder.ToString();
        }

        private void ChooseLocaldbpath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "ACCESS Files (.mdb)|*.mdb|All Files (*.*)|*.*";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() == true)
            {
                localdb.Text = dlg.FileName;
            }
        }

        private void Comfirm_OnClick(object sender, RoutedEventArgs e)
        {
            WritePrivateProfileString("LOCALDB", "DBPath", localdb.Text, inipath);
            WritePrivateProfileString("REMOTEDB", "ip", remoteip.Text, inipath);
            this.Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
