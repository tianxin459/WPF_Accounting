using Accounting.Model;
using Accounting.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Http;

namespace Accounting
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        AppInit appInit = new AppInit();
        public Login()
        {

            var jsonData = DataStorage.LoadAppData(DataStorage.AppFileName);
            appInit = JsonConvert.DeserializeObject<AppInit>(jsonData);

            InitializeComponent();

            //check authorization
            new Task(()=> {
                var currentDate = DateTime.Now;
                var checkDate = appInit.CheckActivateDate;
                if (currentDate >= checkDate)
                {
                    if (IsAppBlock())
                    {
                        Environment.Exit(0);
                        //System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        this.appInit.CheckActivateDate = DateTime.Parse("12/12/2099");
                        var appText = JsonConvert.SerializeObject(appInit);
                        DataStorage.ExportText(DataStorage.AppFileName, appText);
                    }
                }
            }).Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidatePassword(this.txtPassword.Password.Trim()))
                {
                    this.Hide();
                    var window = new Window1();
                    window.Show();
                    this.Close();
                }
                else
                {
                    this.txtMsg.Text = "密码错误";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsAppBlock()
        {
            try
            {
                using (var c = new HttpClient())
                {
                    var url = "https://raw.githubusercontent.com/tianxin459/GeneralAPI/master/Accounting.txt";
                    var response = c.GetAsync(url);
                    var s = response.Result.Content.ReadAsStringAsync();
                    if (s.Result.Trim() == "1")
                        return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private bool ValidatePassword(string pwd)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(pwd));
            var pwdHash = BitConverter.ToString(hashData).Replace("-", "");
            return (pwdHash == appInit.Password);
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                this.cardFliper.IsFlipped = !this.cardFliper.IsFlipped;
                if (ValidatePassword(this.txtOldPassword.Password.Trim()))
                {
                    if (this.txtNewPassword1.Password != this.txtNewPassword2.Password)
                    {
                        this.txtMsg2.Text = "密码不一致";
                        return;
                    }
                    SHA1 sha1 = SHA1.Create();
                    byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(this.txtNewPassword1.Password));
                    var pwdHash = BitConverter.ToString(hashData).Replace("-", "");
                    appInit.Password = pwdHash;
                    var appText = JsonConvert.SerializeObject(appInit);
                    DataStorage.ExportText(DataStorage.AppFileName, appText);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
