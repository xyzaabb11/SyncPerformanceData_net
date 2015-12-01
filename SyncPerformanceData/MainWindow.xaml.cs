using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace SyncPerformanceData
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer timer;
        private NotifyIcon notifyIcon;

        

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
        public MainWindow()
        {
            InitializeComponent();
            InitNotifyIcon();
            this.Closing += MainWindow_Closing;
            this.StateChanged += (sender, args) =>
            {
                if (WindowState == WindowState.Minimized)
                {
	                Visibility = Visibility.Hidden;
                }
            };
            Visibility = Visibility.Hidden;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(30);
            timer.Tick += SyncEx;
            timer.Start();
        }

        private void InitNotifyIcon()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "性能台架数据同步工具";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "性能台架数据同步工具";
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("显示窗口");
            open.Click += (sender, args) => { this.Visibility = Visibility.Visible; 
                WindowState = WindowState.Normal;
                Activate();
            };
            //打开菜单项
            System.Windows.Forms.MenuItem sync = new System.Windows.Forms.MenuItem("立即同步");
            sync.Click += (sender, args) => { SyncEx(this, new EventArgs()); };
            //设置
            System.Windows.Forms.MenuItem setting = new System.Windows.Forms.MenuItem("设置");
            setting.Click += (sender, args) => { Setting_OnClick(this, new RoutedEventArgs()); };
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += (sender, args) => { Environment.Exit(0); };
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, sync, setting, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                Visibility = Visibility.Visible;
            });
        }
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
        private void Exit_Clicked(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Sync_OnClick(object sender, RoutedEventArgs e)
        {
            SyncEx(this, new EventArgs());
        }

        private void Setting_OnClick(object sender, RoutedEventArgs e)
        {
            Setting setting = new Setting();
            setting.Owner = this;
            setting.ShowDialog();
        }

        private void SetOutText(string msg)
        {
            msg += Environment.NewLine;
            OuTextBox.AppendText(msg);
            OuTextBox.ScrollToEnd();
        }

        private void Sync(object sender, EventArgs e)
        {
            int rowcount = 0;
            try
            {
                notifyIcon.BalloonTipText = "开始同步";
                notifyIcon.ShowBalloonTip(1000);
	            SetOutText("[" + DateTime.Now + "]开始同步");
	            StringBuilder localdbpath = new StringBuilder(255);
	            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
	            GetPrivateProfileString("LOCALDB", "DBPath", "chengfeishujuku.mdb", localdbpath, 255, inipath);
	            SetOutText("本地数据路径:" +localdbpath);
	            var connectstr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + localdbpath + @";Jet OLEDB:Database Password=jinshi@@1234;";
	            var localconn = new OleDbConnection(connectstr);
	            localconn.Open();
                
	            if (localconn.State == ConnectionState.Open)
	            {
                    StringBuilder remoteip = new StringBuilder(255);
                    GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
//                     String mysqlStr = "Database=wit_performance;Data Source="+remoteip+";User Id=amje;Password=yjamje;pooling=true;CharSet=utf8;port=3306";
	                String mysqlStr = "server=" + remoteip + ";uid=wit;pwd=wit;database=wit_performance";
	                var mysql = new MySqlConnection(mysqlStr);
                    mysql.Open();
	                var mycmd = new MySqlCommand();
	                mycmd.Connection = mysql;
	                for (int i = 1; i <=6; ++i)
	                {
		                //var query = @"SELECT MAX(检测日期), 泵总成编号, 泵单元号, 修正码 FROM 性能检测数据记录表产品泵测试结果  where 缸位 = '" + i + @"' and 工况 = '4'  group by 泵总成编号, 泵单元号, 修正码; ";
	                    var query =
	                        "SELECT 产品图号, 台架编号, 检测日期, 泵总成编号, 产品泵总成批次号, 缸位, 泵单元号, 泵单元批次号, 修正码, 工况, 修正前油量最大值, 修正前平均油量, 修正前油量最小值, 修正前均方差, 修正前TIP, 修正后油量最大值, 修正后平均油量, 修正后油量最小值, 修正后均方差, 修正后TIP, 修正前是否合格, 修正后是否合格, 组内是否合格, MAX(检测日期)";
	                    query +=
                            "FROM 性能检测数据记录表产品泵测试结果 where 缸位 = '" + i + @"' and 工况 = '4' group by 产品图号, 台架编号, 检测日期, 泵总成编号, 产品泵总成批次号, 缸位, 泵单元号, 泵单元批次号, 修正码, 工况, 修正前油量最大值, 修正前平均油量, 修正前油量最小值, 修正前均方差, 修正前TIP, 修正后油量最大值, 修正后平均油量, 修正后油量最小值, 修正后均方差, 修正后TIP, 修正前是否合格, 修正后是否合格, 组内是否合格;";
	                    OleDbCommand mycom = new OleDbCommand(query, localconn);
	                    var myReader = mycom.ExecuteReader();
                        
	                    while (myReader.Read())
	                    {
                            var sql = ("INSERT INTO `performance_results`(`promapno`, `scaffoldno`, `testdate`, `pumpno`,`pumpgroup`, `tankno`, `pumpunitno`, `pumpunitgroup`, `pumpcode`, `workingcondition`,`prerevisemax`, `prereviseavg`, `prerevismin`, `prereviseRMSE`, `prereviseTIP`,  `afterrevisemax`, `afterreviseavg`, `afterrevismin`, `afterreviseRMSE`, `afterreviseTIPTIP`,`prerevisequalified`, `afterrevisequalified`, `groupqualified`) VALUES ('");

                            //var sql = "INSERT INTO `performance_results`(`testdate`,`pumpno`, `pumpunitno`, `pumpcode`,`tankno`) VALUES ('";

	                        var spump = myReader.GetString(6);
// 	                        sql += myReader.GetString(0) + "','" + myReader.GetString(1) + "','" + myReader.GetString(2) +
// 	                               "','" + myReader.GetString(3) + "','" + i;
	                        for (int j = 0; j < myReader.FieldCount - 1; j++)
	                        {
	                            sql += " '" + myReader.GetString(j) + "',";
	                        }
	                        sql = sql.Substring(0, sql.Length - 1);
                            sql += ")" + " ON DUPLICATE KEY UPDATE pumpunitno = '" + spump + "'";
                            mycmd.CommandText = sql;
                            mycmd.ExecuteNonQuery();
	                        ++rowcount;
	                    }
	                    
                        myReader.Close();
	                }
	                mysql.Close();
	            }
                localconn.Close();
            }
            catch (System.Exception ex)
            {
                SetOutText("错误:" + ex.Message);
            }
            SetOutText("[" + DateTime.Now + "]同步结束, 共同步" + rowcount+"条数据");
            notifyIcon.BalloonTipText = "同步结束";
            notifyIcon.ShowBalloonTip(1);
        }

        private void SyncEx(object sender, EventArgs e)
        {
            int rowcount = 0;
            
                List<string> pathList = new List<string>();
                for (int i = 1; i < 11; i++)
                {
                    pathList.Add(i + "chengfeishujuku.mdb");
                }
//                 pathList.Remove("2chengfeishujuku.mdb");
//                 foreach (var path in pathList)
                {
                    try
                    {
                        notifyIcon.BalloonTipText = "开始同步";
                        notifyIcon.ShowBalloonTip(1000);
                        SetOutText("[" + DateTime.Now + "]开始同步");
                        StringBuilder localdbpath = new StringBuilder(255);
                        var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
                        GetPrivateProfileString("LOCALDB", "DBPath", "chengfeishujuku.mdb", localdbpath, 255, inipath);
                        SetOutText("本地数据路径:" + localdbpath);
                        var connectstr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + localdbpath +
                                         @";Jet OLEDB:Database Password=jinshi@@1234;";
                        var localconn = new OleDbConnection(connectstr);
                        localconn.Open();

                        if (localconn.State == ConnectionState.Open)
                        {
                            StringBuilder remoteip = new StringBuilder(255);
                            GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
                            //                     String mysqlStr = "Database=wit_performance;Data Source="+remoteip+";User Id=amje;Password=yjamje;pooling=true;CharSet=utf8;port=3306";
                            String mysqlStr = "server=" + remoteip + ";uid=wit;pwd=wit;database=wit_performance";
                            var mysql = new MySqlConnection(mysqlStr);
                            mysql.Open();
                            var mycmd = new MySqlCommand();
                            mycmd.Connection = mysql;
                            var query = "SELECT * FROM 性能合格统计参考表 order by 编号";
                            OleDbCommand mycom = new OleDbCommand(query, localconn);
                            var myReader = mycom.ExecuteReader();
                            //var s = "SELECT `sn` FROM `pump_performance_results` ";
                            var s = "SELECT `sn` FROM `pump_performance_results1`";
                            mycmd.CommandText = s;
                            var reader = mycmd.ExecuteReader();
                            List<string> snList = new List<string>();
                            while (reader.Read())
                            {
                                snList.Add(reader.GetString(0));
                            }
                            reader.Close();
                            MySqlTransaction tx = mysql.BeginTransaction();
                            mycmd.Transaction = tx;
                            while (myReader.Read())
                            {
                                //var sql = ("INSERT INTO `performance_results`(`promapno`, `scaffoldno`, `testdate`, `pumpno`,`pumpgroup`, `tankno`, `pumpunitno`, `pumpunitgroup`, `pumpcode`, `workingcondition`,`prerevisemax`, `prereviseavg`, `prerevismin`, `prereviseRMSE`, `prereviseTIP`,  `afterrevisemax`, `afterreviseavg`, `afterrevismin`, `afterreviseRMSE`, `afterreviseTIPTIP`,`prerevisequalified`, `afterrevisequalified`, `groupqualified`) VALUES ('");

                                //var sql = "INSERT INTO `performance_results`(`testdate`,`pumpno`, `pumpunitno`, `pumpcode`,`tankno`) VALUES ('";

                                string sql;
                                var sn = myReader.GetValue(5) + "-" + myReader.GetValue(0) + "-" + myReader.GetValue(1);
                                if (snList.Contains(sn))
                                {
                                    snList.Remove(sn);
                                    continue;
                                }

                                sql =
                                    @"INSERT INTO `pump_performance_results1`(`sn`, `testdate`, `worknumber`, `worker`, `promapno`,";
                                sql +=
                                    @" `scaffoldno`, `pumpunitgroup`, `pumpno`, `pumpunitno`, `isrunin`, `judgebyman`, `comment`,";
                                sql +=
                                    @" `pumpunitprep`, `newdeproperty`, `pumpcode`, `tankno`, `prefuel1`, `prefuel2`, `prefuel3`,";
                                sql +=
                                    @" `prefuel4`, `prefuel5`, `prefuel6`, `afterfuel1`, `afterfuel2`, `afterfuel3`, `afterfuel4`,";
                                sql +=
                                    @" `afterfuel5`, `afterfuel6`, `preSGM1`, `preSGM2`, `preSGM3`, `preSGM4`, `preSGM5`, `preSGM6`,";
                                sql +=
                                    @" `afterSGM1`, `afterSGM2`, `afterSGM3`, `afterSGM4`, `afterSGM5`, `afterSGM6`, `preTIP1`,";
                                sql +=
                                    @" `preTIP2`, `preTIP3`, `preTIP4`, `preTIP5`, `preTIP6`, `afterTIP1`, `afterTIP2`, `afterTIP3`,";
                                sql += @" `afterTIP4`, `afterTIP5`, `afterTIP6`) VALUES (";
                                //                         var sn = myReader.GetValue(5) + "-" + myReader.GetInt32(0) + "-" + myReader.GetString(1);
                                sql += " '" + sn + "',";
                                if (sn == "21060")
                                {
                                    Console.WriteLine();
                                }
                                for (int j = 1; j < myReader.FieldCount; j++)
                                {
                                    sql += " '" + myReader.GetValue(j) + "',";
                                }
                                sql = sql.Substring(0, sql.Length - 1);
                                sql += ")";
                                mycmd.CommandText = sql;
                                mycmd.ExecuteNonQuery();
                                if (rowcount%1000 == 0)
                                {
                                    tx.Commit();
                                    tx = mysql.BeginTransaction();
                                }
                                ++rowcount;
                            }
                            tx.Commit();
                            myReader.Close();

                            mysql.Close();
                        }
                        localconn.Close();
                        SetOutText("[" + DateTime.Now + "]同步结束, 共同步" + rowcount + "条数据");
                        notifyIcon.BalloonTipText = "同步结束";
                        notifyIcon.ShowBalloonTip(1);
                        
                    }
                    catch(System.Exception ex)
                    {
                        SetOutText("错误:" + ex.Message);
                    }

                }
            
            
                
            
        }
    }
}
