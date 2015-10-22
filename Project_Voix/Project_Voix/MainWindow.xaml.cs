using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Voix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public Stopwatch initStopwatch;
        public CancellationTokenSource cancelToken = new CancellationTokenSource();
        static public CancellationToken ct;
        
        public MainWindow()
        {
            initStopwatch = new Stopwatch();
            Stopwatch stop = new Stopwatch();
            stop.Start();
            InitializeComponent();
            ct = cancelToken.Token;
            Task initRun = new Task(new Action(Init.StartInit),cancelToken.Token);
            this.Closed += (sender, e) =>
            {
                Console.WriteLine(ct.CanBeCanceled.ToString());
                cancelToken.Cancel();
            };
            initRun.Start();           //starts the Init.StartInit method, which is the main recognizer thread, asynchronously
            stop.Stop();
            GrammarFeeder_writeToTextBox(string.Format("time taken in the mainwindow instantiator {0} ms",stop.ElapsedMilliseconds));
            
            GrammarFeeder.writeToTextBox += GrammarFeeder_writeToTextBox;          //event handler for GrammarFeeder's writeToTextBox event
        }

        private void GrammarFeeder_writeToTextBox(string logUpdate)                                     
        {
            commandLog.Dispatcher.InvokeAsync(()=> { commandLog.Text += string.Format("\n{0}", logUpdate); });          //used to access A UI element from another thread
        }
    }
}
