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

namespace Accounting
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
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

        private bool ValidatePassword(string pwd)
        {
            var jsonData = DataStorage.LoadAppData(DataStorage.AppFileName);
            var objApp = JsonConvert.DeserializeObject<AppInit>(jsonData);
            SHA1 sha1 = SHA1.Create();
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(pwd));
            var pwdHash = BitConverter.ToString(hashData).Replace("-", "");
            return (pwdHash == objApp.Password);
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
                    var jsonData = DataStorage.LoadAppData(DataStorage.AppFileName);
                    var objApp = JsonConvert.DeserializeObject<AppInit>(jsonData);
                    SHA1 sha1 = SHA1.Create();
                    byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(this.txtNewPassword1.Password));
                    var pwdHash = BitConverter.ToString(hashData).Replace("-", "");
                    objApp.Password = pwdHash;
                    var appText = JsonConvert.SerializeObject(objApp);
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
