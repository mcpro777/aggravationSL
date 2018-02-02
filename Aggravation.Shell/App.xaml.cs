using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;

namespace Aggravation.Shell
{
    public partial class App : Application
    {
        private IDictionary<String, String> _initParams;
        private IEventAggregator _eventAggregator;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this._initParams = e.InitParams;

            System.IO.Stream s = this.GetType().Assembly.GetManifestResourceStream("Aggravation.Shell.Catalog.xaml");
            String catalog = new System.IO.StreamReader(s).ReadToEnd();

            this.InitializeBootstrapper(catalog);
        }

        public void InitializeBootstrapper(string catalog)
        {
            var bootstrapper = new Bootstrapper(_initParams, catalog);
            bootstrapper.Run();

            this._eventAggregator = bootstrapper.Container.Resolve<IEventAggregator>();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
