using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GetPumpCodePlug
{
    /// <summary>
    /// login.xaml 的交互逻辑
    /// </summary>
    public partial class login : Window, INotifyPropertyChanged
    {
        private string _user;
        private string _password;

        public login()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string User
        {
            get { return _user; }
            set
            {
                if (value != _user)
                {
                    _user = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("User"));
                }
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Password"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate {};

        private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var remoteip = new StringBuilder(255);
            var inipath = System.AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            StaticInterface.GetPrivateProfileString("REMOTEDB", "ip", "127.0.0.1", remoteip, 255, inipath);
            var mysqlStr = "server=" + remoteip + ";uid=pump;pwd=pumpszmmcd;database=pump_performance";
            var mysql = new SqlConnection(mysqlStr);
            try
            {
                mysql.Open();
                var sql = "select * from auth where user = '" + User + "' and password = '" + MD5(passwordBox.Password) + "'";
            }
            catch (Exception)
            {
                MessageBox.Show("登录失败!");
            }
            finally
            {
                mysql.Close();
            }
            Close();
        }

        private string MD5(string ori)
        {
            byte[] result = Encoding.Default.GetBytes(ori.Trim());    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");  //tbMd5pass为输出加密文本的
        }
    }
}
