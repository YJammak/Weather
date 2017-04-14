using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using NLog;

namespace WeatherCalendar
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create and show the application's main window
            //MainWindow window = new MainWindow();

            this.InitializeComponent();
        }

        public void Activate()
        {
            // Reactivate application's main window
            //this.MainWindow.Show();
            var w = this.MainWindow as MainWindow;

            if (w == null)
                return;

            HolidayHelper.Instance.LoadHolidayInfo();
            CountDownHelper.Instance.Load();
            NotesHelper.Instance.Load();

            w.UpdateWeather();
        }
    }

    class SingleInstanceManager : WindowsFormsApplicationBase
    {
        App app;

        private Logger logger = LogManager.GetCurrentClassLogger();

        public SingleInstanceManager()
        {
            this.IsSingleInstance = true;
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            // First time app is launched
            app = new App();
            app.DispatcherUnhandledException += (sender, args) =>
            {
                logger.Error(args.Exception, "程序崩溃");
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                logger.Error(args.ExceptionObject as Exception, "程序崩溃");
            };

            app.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            // Subsequent launches
            base.OnStartupNextInstance(eventArgs);
            app.Activate();
        }
    }
}
