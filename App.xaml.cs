using Accounting.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace Accounting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    CheckAdministrator();
        //    //如果不是管理员，程序会直接退出，并使用管理员身份重新运行。
        //    StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
        //}
        private void CheckAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,
                // so instead we launch the app as administrator in a new process.
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // Shut down the current process
                Environment.Exit(0);
            }
        }

        public const string GLOBAL_MEMBERS = "Members";
        public const string GLOBAL_SELECTEDMEMBERS = "SelectedMembers";
        public const string GLOBAL_SELECTEDMEMBERSTACK = "SelectedMemberStack";
        //public static Member SelectedMember
        //{
        //    get
        //    {
        //        if (!Application.Current.Properties.Contains(GLOBAL_SELECTEDMEMBERS))
        //            return null;
        //        return Application.Current.Properties[GLOBAL_SELECTEDMEMBERS] as Member;
        //    }
        //    set
        //    {
        //        Application.Current.Properties[GLOBAL_SELECTEDMEMBERS] = value;
        //    }
        //}

        

        //public static Stack<Member> SelectedMemberStack
        //{
        //    get
        //    {
        //        if (!Application.Current.Properties.Contains(GLOBAL_SELECTEDMEMBERSTACK))
        //            return new Stack<Member>();
        //        return Application.Current.Properties[GLOBAL_SELECTEDMEMBERSTACK] as Stack<Member>;
        //    }
        //    set
        //    {
        //        Application.Current.Properties[GLOBAL_SELECTEDMEMBERSTACK] = value;
        //    }
        //}

        public static Member SelectedMember { get; set; } = new Member();
        public static List<Member> Members { get; set; } = new List<Member>();
        public static ConcurrentStack<Member> SelectedMemberStack { get; set; } = new ConcurrentStack<Member>();

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message+"\n"+e.Exception.StackTrace,"错误信息请发送截图至：onlyheart@qq.com");
        }
    }

}
