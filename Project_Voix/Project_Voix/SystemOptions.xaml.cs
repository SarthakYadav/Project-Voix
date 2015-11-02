/*
    description: SystemOptions class
           -class is the dialog which provides various Power Options
           
    date created: -2/11/15                  Author:Sarthak

    log:-
    * No Updates Done*

     Listed Public Methods:
           * NONE*

*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_Voix
{
    /// <summary>
    /// Interaction logic for SystemOptions.xaml
    /// </summary>
    public partial class SystemOptions : Window
    {
        static string fileName = "";
        static string arguments = "";


        public SystemOptions()
        {
            InitializeComponent();
        }

        public static void StartSystemProcess()
        {
            ProcessStartInfo info = new ProcessStartInfo(fileName, arguments);
            Process process = Process.Start(info);
        }
        public static void OpenSystemCommandsDialog()
        {
            /*
                Starts a new SystemOptions dialog on a separate UI thread
            */
            Thread thread = new Thread(() =>
            {
                SystemOptions sysControl = new SystemOptions();
                sysControl.Closed += (sender, e) =>
                {
                    sysControl.Dispatcher.InvokeShutdown();
                    DataStore.handle1.Set();
                };
                sysControl.Show();
                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void shutdownBtn_Click(object sender, RoutedEventArgs e)
        {
            fileName = "shutdown.exe";
            arguments = "-s";

            ResponseGenerator.SendForSynthesis("System shut down initiated");
            Thread.Sleep(5000);
            StartSystemProcess();
        }

        private void restartBtn_Click(object sender, RoutedEventArgs e)
        {
            fileName = "shutdown.exe";
            arguments = "-r";
            ResponseGenerator.SendForSynthesis("System Restart is now Initiated . Hope you saved all the work");
            Thread.Sleep(5000);
            StartSystemProcess();
        }

        private void sleepBtn_Click(object sender, RoutedEventArgs e)
        {
            fileName = "Rundll32.exe";
            arguments = "powrprof.dll, SetSuspendState 0,1,0";
            ResponseGenerator.SendForSynthesis("Going to sleep");
            Thread.Sleep(5000);
            StartSystemProcess();
        }

        private void logOffBtn_Click(object sender, RoutedEventArgs e)
        {
            fileName = "shutdown.exe";
            arguments = "-l";
            ResponseGenerator.SendForSynthesis("Logging off");
            Thread.Sleep(5000);
            StartSystemProcess();
        }

        private void lockBtn_Click(object sender, RoutedEventArgs e)
        {
            fileName = "Rundll32.exe";
            arguments = "User32.dll, LockWorkStation";
            ResponseGenerator.SendForSynthesis("Locking the current user");
            Thread.Sleep(5000);
            StartSystemProcess();
        }

        private void hibernateBtn_Click(object sender, RoutedEventArgs e)
        {
            fileName = @"%windir%\system32\rundll32.exe";
            arguments = "PowrProf.dll, SetSuspendState";
            ResponseGenerator.SendForSynthesis("Hibernating");
            Thread.Sleep(5000);
            StartSystemProcess();
        }
    }
}
