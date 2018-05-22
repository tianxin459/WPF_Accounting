using Accounting.Model;
using Accounting.Util;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
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
    /// Interaction logic for BonusConfigDialog.xaml
    /// </summary>
    public partial class BonusConfigDialog : UserControl
    {
        public Button SaveButton { get { return this.btnSave; } set { this.btnSave = value; } }

        public BonusConfigDialog()
        {
            InitializeComponent();
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            var configJson = DataStorage.LoadAppData("Config.json");
            //List<decimal> d = JsonConvert.DeserializeObject<List<decimal>>(configJson);
            //Member.BonusBase = d;
            this.txtBonusConfig.Text = configJson;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataStorage.ExportText("Config.json", this.txtBonusConfig.Text);
                
                List<decimal> d = JsonConvert.DeserializeObject<List<decimal>>(this.txtBonusConfig.Text);
                Member.BonusBase = d;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
