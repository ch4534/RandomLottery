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
using System.Windows.Shapes;

namespace RandomLottery
{
    /// <summary>
    /// ModelWnd.xaml 的交互逻辑
    /// </summary>
    public partial class ModelWnd : Window
    {
        public ModelWnd()
        {
            InitializeComponent();
        }

        private void NavigateToMain(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string btnName = btn.Name;
            switch (btnName)
            {
                case "BilibiliBtn":
                    MainWindow.mLiveType = MainWindow.LiveType.Bilibili;
                    this.DialogResult = true;
                    break;

                case "ZhanqiBtn":
                    MainWindow.mLiveType = MainWindow.LiveType.ZhanQi;
                    this.DialogResult = true;
                    break;

                default:
                    this.DialogResult = false;
                    break;
            }

            this.Close();
        }
    }
}
