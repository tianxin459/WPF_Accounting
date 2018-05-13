using Accounting.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Accounting
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : NavigationWindow
    {
        public Window1()
        {
            InitializeComponent();
        }

        //protected override void onn

        private void NavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            
            if (Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;
                _navArgs = e;
                this.IsHitTestVisible = false;
                DoubleAnimation da = new DoubleAnimation(0.3d, new Duration(TimeSpan.FromMilliseconds(100)));
                da.Completed += FadeOutCompleted;
                this.BeginAnimation(OpacityProperty, da);
            }
            _allowDirectNavigation = false;
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            (sender as AnimationClock).Completed -= FadeOutCompleted;

            this.IsHitTestVisible = true;

            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null)
                    {
                        NavigationService.Navigate(_navArgs.Content);
                    }
                    else
                    {
                        NavigationService.Navigate(_navArgs.Uri);
                    }
                    break;
                case NavigationMode.Back:
                    NavigationService.GoBack();

                    if (App.SelectedMemberStack.Count() > 0)
                    {
                        Member m;
                        App.SelectedMemberStack.TryPop(out m);
                        App.SelectedMember = m;
                    }
                    break;

                case NavigationMode.Forward:
                    NavigationService.GoForward();
                    break;
                case NavigationMode.Refresh:
                    NavigationService.Refresh();
                    break;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (ThreadStart)delegate ()
                {
                    DoubleAnimation da = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(200)));
                    this.BeginAnimation(OpacityProperty, da);
                });
        }

        private bool _allowDirectNavigation = false;
        private NavigatingCancelEventArgs _navArgs = null;

        private void NavigationWindow_Navigated(object sender, NavigationEventArgs e)
        {
        }

        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth - 60;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 60;
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            //this.ResizeMode = System.Windows.ResizeMode.NoResize;
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //this.Topmost = true;

            this.Left = 30;
            this.Top = 30;
        }
    }
}
