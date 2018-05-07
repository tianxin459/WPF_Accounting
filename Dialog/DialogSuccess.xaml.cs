using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Accounting.Dialog
{
    /// <summary>
    /// Interaction logic for DialogSuccess.xaml
    /// </summary>
    public partial class DialogSuccess : UserControl
    {
        public string Message { get; set; }
        public DialogSuccess(string message = "message")
        {
            Message = message;
            InitializeComponent();
        }
    }
}
