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
        public static AutoResetEvent waitHandle = new AutoResetEvent(false);
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => { Init.StartInit(); });

        //    Task.Run(() =>
        //    {
        //        DataStore.LoadUserSettings();
        //        SpeechRecognitionEngine speechEngine = new SpeechRecognitionEngine();
        //        Stopwatch watch = new Stopwatch();
        //        DataStore.DisplayCurrentUser();


        //        watch.Start();
        //        try
        //        {
        //            GrammarFeeder.GrammarLoader(ref speechEngine);      //call to the public method of GrammarFeeder class 
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(string.Format("The message is {0} and sent by {1} and this is the stack traced {2}", e.Message, e.Source, e.StackTrace));
        //            MessageBox.Show(string.Format("The message is {0} and sent by {1} and this is the stack traced {2}", e.InnerException.Message, e.InnerException.Source, e.InnerException.StackTrace));
        //            //MessageBox.Show(e.InnerException.InnerException.Message);
        //        }

        //        List<Grammar> grammarList = new List<Grammar>(speechEngine.Grammars);
        //        GrammarManipulator.RegisterWithManipulator(ref speechEngine);
        //        //GrammarManipulator.LoadNonOperativeCommands(ref speechEngine);

        //        ProgramManager.InitializeManager();
        //        watch.Stop();

        //        //Console.WriteLine("total ticks {0}", watch.ElapsedMilliseconds);


        //        waitHandle.WaitOne();

        //        //ResponseGenerator.NonOperational_ResponseHandler(new Response(CommandType.NonOperational,DateTime.Now.TimeOfDay.Hours,"hello Tars"));
        //        ProgramManager.SendOpenCommand("Notepad++");
        //        //ResponseGenerator.CloseProgram_ResponseHandler(new Response(CommandType.CloseProgram, DateTime.Now.TimeOfDay.Hours, "Notepad++"));
        //        //Thread.Sleep(2000);
        //        ProgramManager.SendCloseCommand("Tars Close Notepad++");


        //        //Console.WriteLine("Doin it now5");
        //        //ResponseGenerator.NonOperational_ResponseHandler("tell me the time");
        //        speechEngine.SetInputToDefaultAudioDevice();
        //        speechEngine.RecognizeAsync(RecognizeMode.Multiple);



        //        //Console.ReadLine();         //keeps the console open during debugging

        //        foreach (var item in DataStore.RecentCommands)
        //        {
        //            Console.WriteLine(item);
        //        }

        //        DataStore.SaveUserSettings();


        //        //foreach (var item in DataStore.Users)
        //        //{
        //        //  Console.WriteLine(item);
        //        //}
        //        //Console.ReadLine();
        //    });
        }
        //static void Start()
        //{
          //  Init.StartInit();
        //}
    }
}
