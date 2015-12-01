using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GetPumpCodePlug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private DispatcherTimer synctimer;

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        public MainWindow()
        {
            InitializeComponent();

            login dlg = new login();
//            dlg.Owner = this;
            dlg.ShowDialog();

            this.DataContext = this;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += GetLastPump;
            synctimer = new DispatcherTimer();
            synctimer.Interval = TimeSpan.FromMinutes(10);
            synctimer.Tick += Sync;
            synctimer.Start();
            StartButton_OnClick(this, new RoutedEventArgs());

        }

        private void Sync(object sender, EventArgs e)
        {
            if (File.Exists("temp.txt"))
            {
                StringBuilder remoteip = new StringBuilder(255);
                var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
                GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
                String mysqlStr = "server=" + remoteip + ";uid=pump;pwd=pumpszmmcd;database=pump_performance";
                var mysql = new SqlConnection(mysqlStr);
                try
                {
                    StreamReader sr = new StreamReader("temp.txt");
                    var sql = sr.ReadToEnd();
                    sr.Close();
                    mysql.Open();
                    var cmd = mysql.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    File.Delete("temp.txt");
                }
                catch (Exception)
                {

                    
                }
                finally
                {
                    mysql.Close();
                }
            }
        }
        

        private void GetLastPump(object sender, EventArgs e)
        {
            StringBuilder localdbpath = new StringBuilder(255);
            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            GetPrivateProfileString("LOCALDB", "DBPath", "chengfeishujuku.mdb", localdbpath, 255, inipath);
            var connectstr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + localdbpath +
                             @";Jet OLEDB:Database Password=jinshi@@1234;";
            var localconn = new OleDbConnection(connectstr);
            OleDbDataReader myReader = null;
            try
            {

                localconn.Open();

                if (localconn.State == ConnectionState.Open)
                {
                    for (int i = 1; i <= 4; ++i)
                    {
                        //var query = @"SELECT MAX(检测日期), 泵总成编号, 泵单元号, 修正码 FROM 性能检测数据记录表产品泵测试结果  where 缸位 = '" + i + @"' and 工况 = '4'  group by 泵总成编号, 泵单元号, 修正码; ";
/*
                        var datequery = "SELECT MAX(CDate(生产日期)) FROM 性能合格统计参考表 where 缸位 = '" + i + @"'";
                        OleDbCommand mycom = new OleDbCommand(datequery, localconn);
                        var myReader = mycom.ExecuteReader();
                        if (myReader.HasRows == false)
                            continue;
                        myReader.Read();
                        var datetime = myReader.GetDateTime(0).ToLocalTime().ToString();
                        string date = datetime.Split(new char[]{'/', '-'})[2];
                        myReader.Close();
                        var query =
                            "SELECT 台架号, 生产日期, 泵总成编号, 缸位, 泵单元编号, 修正码  FROM 性能合格统计参考表 ";
                        query +=
                            "where 生产日期 like '%" + date + "'";*/
                        var query =
                            "SELECT 台架号, CDate(生产日期), 泵总成编号, 缸位, 泵单元编号, 修正码  FROM 性能合格统计参考表 ";
                        query +=
                            "where CDate(生产日期) = (SELECT MAX(CDate(生产日期)) FROM 性能合格统计参考表 where 缸位 = '" + i +
                            @"' );";


                        OleDbCommand mycom = new OleDbCommand(query, localconn);
                        try
                        {
	                        myReader = mycom.ExecuteReader();
                        }
                        catch (System.Exception ex)
                        {
                            query =
                            "SELECT 台架号, CDate(生产日期), 泵总成编号, 缸位, 泵单元编号, 修正码  FROM 性能合格统计参考表 " +
                                "where 生产日期 = (SELECT MAX(CDate(生产日期)) FROM 性能合格统计参考表 where 缸位 = '" + i +
                                @"' );";
                            mycom.CommandText = query;
                            myReader = mycom.ExecuteReader();
                        }
/*
                        if (myReader.HasRows == false)
                        {
                            query =
                            "SELECT 台架号, 生产日期, 泵总成编号, 缸位, 泵单元编号, 修正码  FROM 性能合格统计参考表 " +
                                "where 生产日期 = (SELECT MAX(生产日期) FROM 性能合格统计参考表 where 生产日期 like '"
                                + String.Format("{0:D}/{1:D}/{2:D} {3:D}%", datetime.Year, datetime.Month, datetime.Day, datetime.Hour)
                                + " or 生产日期 like " 
                                + String.Format("{0:D}/{1:D}/{2:D} {3:D}%", datetime.Year, datetime.Month, datetime.Day, datetime.Hour - 1)
                                + "' and 缸位 = '" + i +
                                @"' );";
                            mycom = new OleDbCommand(query, localconn);
                            myReader = mycom.ExecuteReader();
                        }
*/
                        if (myReader != null && myReader.Read())
                        {
                            PumpNoTextBox.Text = PumpNo = myReader.GetString(2);
                            Scaffoldno = myReader.GetString(0);
                            _testdate[i] = myReader.GetDateTime(1).ToString();
                            _pumpunitno[i] = myReader.GetString(4);
                            _pumpcode[i] = myReader.GetString(5);
                           
                                if (i == 1)
                                {
                                    PumpUnitNo1TextBox.Text = myReader.GetString(4);
                                    PumpCode1TextBox.Text = myReader.GetString(5).Replace(" ", "");
                                }
                                else if (i == 2)
                                {
                                    PumpUnitNo2TextBox.Text = myReader.GetString(4);
                                    PumpCode2TextBox.Text = myReader.GetString(5).Replace(" ", "");
                                }
                                else if (i == 3)
                                {
                                    PumpUnitNo3TextBox.Text = myReader.GetString(4);
                                    PumpCode3TextBox.Text = myReader.GetString(5).Replace(" ", "");
                                }
                                else if (i == 4)
                                {
                                    PumpUnitNo4TextBox.Text = myReader.GetString(4);
                                    PumpCode4TextBox.Text = myReader.GetString(5).Replace(" ", "");
                                }
                            
                        }
                        myReader.Close();
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (myReader != null)
                {
                    myReader.Close();
                }
                MessageBox.Show(this, ex.Message /*"打开本地数据库失败，请确认配置中指向的本地数据库是否存在"*/, "错误", MessageBoxButton.OK);
//                 StartButton_OnClick(this, new RoutedEventArgs());
                
            }
            finally
            {
                localconn.Close();
//                 timer.Stop();
//                 StartButton.Content = "开始查询";
            }
        }

        private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
//             StartButton_OnClick(this, new RoutedEventArgs());
            StringBuilder remoteip = new StringBuilder(255);
            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
            String mysqlStr = "server=" + remoteip + ";uid=pump;pwd=pumpszmmcd;database=pump_performance";
            var mysql = new SqlConnection(mysqlStr);
            string sql = "";
            try
            {

                sql = "";
                PumpNo = PumpNoTextBox.Text;
                _pumpunitno[1] = PumpUnitNo1TextBox.Text;
                _pumpcode[1] = PumpCode1TextBox.Text;
                _pumpunitno[2] = PumpUnitNo2TextBox.Text;
                _pumpcode[2] = PumpCode2TextBox.Text;
                _pumpunitno[3] = PumpUnitNo3TextBox.Text;
                _pumpcode[3] = PumpCode3TextBox.Text;
                _pumpunitno[4] = PumpUnitNo4TextBox.Text;
                _pumpcode[4] = PumpCode4TextBox.Text;
                sql = "INSERT INTO pump_code1(scaffoldno, testdate, pumpno, pumpunitno1, pumpcode1,pumpunitno2, pumpcode2,pumpunitno3, pumpcode3,pumpunitno4, pumpcode4) VALUES('" +
                    Scaffoldno + "','" + DateTime.Now + "','" + PumpNo + "', '" + _pumpunitno[1] + "', '" +
                    _pumpcode[1] + _pumpunitno[2] + "', '" +
                    _pumpcode[2] + _pumpunitno[3] + "', '" +
                    _pumpcode[3] + _pumpunitno[4] + "', '" +
                    _pumpcode[4] + "');";
                /*for (int i = 1; i <= 4; i++)
                {
                    sql +=
                        "INSERT INTO pump_code(scaffoldno, testdate, pumpno, tankno, pumpunitno, pumpcode) VALUES('" +
                        Scaffoldno + "','" + DateTime.Now + "','" + PumpNo + "', '" + i + "', '" + _pumpunitno[i] +
                        "', '" + _pumpcode[i] + "');";
                }*/
                mysql.Open();
                var mycmd = new SqlCommand();

                /*for (int i = 1; i <= 4; i++)
                {
                    if (PumpNo == "" || _pumpcode[i] == "" || _pumpunitno[i] == "")
                    {
                        continue;
                    }*/

                mycmd.Connection = mysql;
                mycmd.CommandText = "select * from pump_code1 where pumpno like '%" + PumpNo + "%';";
                var reader = mycmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (MessageBox.Show("数据库中已有此总成数据，是否强行替换为新数据？", "警告", MessageBoxButton.YesNo) ==
                        MessageBoxResult.No)
                    {
                        reader.Close();
                        mysql.Close();
                        return;
                    }
                    else
                    {
                        reader.Close();
                        mycmd.CommandText = "delete from pump_code1 where pumpno like '%" + PumpNo + "%';";
                        mycmd.ExecuteNonQuery();

                    }
                }
//                     else
//                     {
                reader.Close();
                var ssql =
                    "INSERT INTO pump_code1(scaffoldno, testdate, pumpno, pumpunitno1, pumpcode1,pumpunitno2, pumpcode2,pumpunitno3, pumpcode3,pumpunitno4, pumpcode4) VALUES('" +
                    Scaffoldno + "','" + DateTime.Now + "','" + PumpNo + "', '" + _pumpunitno[1] + "', '" +
                    _pumpcode[1] + "', '" + _pumpunitno[2] + "', '" +
                    _pumpcode[2] + "', '" + _pumpunitno[3] + "', '" +
                    _pumpcode[3] + "', '" + _pumpunitno[4] + "', '" +
                    _pumpcode[4] + "');";
                mycmd.CommandText = ssql;
                mycmd.ExecuteNonQuery();
//                     }

                //   }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message /*"打开本地数据库失败，请确认配置中指向的本地数据库是否存在"*/, "错误", MessageBoxButton.OK);
                StreamWriter fw = File.AppendText("temp.txt");
                fw.WriteLine(sql);
                fw.Flush();
                fw.Close();
            }
            finally
            {
                mysql.Close();
//                 StartButton_OnClick(this, new RoutedEventArgs());
            }
        }

        private void SettingButton_OnClick(object sender, RoutedEventArgs e)
        {
            Setting setting = new Setting();
            setting.Owner = this;
            setting.ShowDialog();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
//             if ((StartButton.Content as string) == "自动查询")
//             {
//                 timer.Start();
//                 StartButton.Content = "手动修改";
//                 PumpUnitNo1TextBox.IsReadOnly = true;
//                 PumpCode1TextBox.IsReadOnly = true;
//                 PumpUnitNo2TextBox.IsReadOnly = true;
//                 PumpCode2TextBox.IsReadOnly = true;
//                 PumpUnitNo3TextBox.IsReadOnly = true;
//                 PumpCode3TextBox.IsReadOnly = true;
//                 PumpUnitNo4TextBox.IsReadOnly = true;
//                 PumpCode4TextBox.IsReadOnly = true;
//             }
//             else
//             {
//                 timer.Stop();
//                 StartButton.Content = "自动查询";
//                 PumpUnitNo1TextBox.IsReadOnly = false;
//                 PumpCode1TextBox.IsReadOnly = false;
//                 PumpUnitNo2TextBox.IsReadOnly = false;
//                 PumpCode2TextBox.IsReadOnly = false;
//                 PumpUnitNo3TextBox.IsReadOnly = false;
//                 PumpCode3TextBox.IsReadOnly = false;
//                 PumpUnitNo4TextBox.IsReadOnly = false;
//                 PumpCode4TextBox.IsReadOnly = false;
//             }
            if ((StartButton.Content as string) == "自动查询")
            {
                StartButton.Content = "更新数据";
            }
            GetLastPump(this, new EventArgs());
        }

        string pumpno;

        public String PumpNo
        {
            get { return pumpno; }
            set { pumpno = value; }
        }
        private String Scaffoldno { get; set; }
        private String[] _testdate = new string[6];
        private String[] _pumpunitno = new string[6];
        private String[] _pumpcode = new string[6];
    }
}
