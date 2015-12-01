using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenerateData
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            GerData();
        }
        private void GerData()
        {
            StringBuilder localdbpath = new StringBuilder(255);
            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            GetPrivateProfileString("LOCALDB", "DBPath", "chengfeishujuku.mdb", localdbpath, 255, inipath);
            var connectstr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + localdbpath +
                             @";Jet OLEDB:Database Password=jinshi@@1234;";
            var localconn = new OleDbConnection(connectstr);
            try
            {
                localconn.Open();
                if (localconn.State == ConnectionState.Open)
                {
                    for (int i = 1; i <=4; ++i)
                    {
                        var rand = new Random(DateTime.Now.Millisecond);
                        var pump = String.Format("{0:X}", rand.Next());
                        Thread.Sleep(5);
                        var pumpunit = String.Format("{0:X}", rand.Next());
                        Thread.Sleep(5);
                        var pumpcode = String.Format("{0:X}", rand.Next());
                        var cmd = localconn.CreateCommand();
                        cmd.CommandText = "INSERT INTO 性能合格统计参考表 (台架号, 生产日期, 泵总成编号, 缸位, 泵单元编号, 修正码) VALUES('SC001', '"
                                          + DateTime.Now + "','" + pump + "', '" + i + "','" + pumpunit + "','" + pumpcode + "');" ;
                        cmd.ExecuteNonQuery();
                        Thread.Sleep(10);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message /*"打开本地数据库失败，请确认配置中指向的本地数据库是否存在"*/, "错误", MessageBoxButton.OK);
            }
            finally
            {
                localconn.Close();
            }
        }
    }
}
