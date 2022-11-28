using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MeetingsSync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Log.Instance.WriteInformation("Starting MeetingsSync service.");

            //TestForm testForm = new TestForm();
            //testForm.ShowDialog();

            MeetingsSyncService service = new MeetingsSyncService();

            AppDomain.CurrentDomain.UnhandledException += service.HandleUnhandledException;

            if ( service.getSettings().RunForeground )
            {
                service.debugStart(args);
                System.Threading.Thread.Sleep(600000);
                service.debugStop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
