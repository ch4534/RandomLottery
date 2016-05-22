using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RandomLottery
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //[DllImport("user32", CharSet = CharSet.Unicode)]
        //static extern IntPtr FindWindow(string cls, string win);
        //[DllImport("user32")]
        //static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        //[DllImport("user32")]
        //static extern bool IsIconic(IntPtr hWnd);
        //[DllImport("user32")]
        //static extern bool OpenIcon(IntPtr hWnd);

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNew;
            var mutex = new Mutex(true, "SingletonInstance", out isNew);
            if (!isNew)
            {
               // ActivateOtherWindow();
                Application.Current.Shutdown();
            }
            else
            {
                Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                ModelWnd modelWnd = new ModelWnd();
                bool? modelResult = modelWnd.ShowDialog();
                if (modelResult.HasValue == true && modelResult.Value == true)
                {
                    base.OnStartup(e);
                    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                }
                else
                {
                    this.Shutdown();
                }
            }
        }

        //private static void ActivateOtherWindow()
        //{
        //    //var other_model = FindWindow(null, "MainWindow");
        //    var other = FindWindow("MainWindow", null);
        //    if (other != IntPtr.Zero)
        //    {
        //        SetForegroundWindow(other);
        //        if (IsIconic(other))
        //        {
        //            OpenIcon(other);
        //        }
        //    }
        //}
    }
}
