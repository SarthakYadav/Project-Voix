using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_Voix
{
    static class Init
    {

        public static AutoResetEvent waitHandle = new AutoResetEvent(false);
        public static AutoResetEvent waitHandle2 = new AutoResetEvent(false);
        public static void StartInit()
        {
            MainWindow.initStopwatch.Start();

            if (MainWindow.ct.IsCancellationRequested)
                return;

            
            Task.Run(() =>
            {
                DataStore.AddUser();
            });
            waitHandle2.WaitOne();
            SpeechRecognitionEngine speechEngine = new SpeechRecognitionEngine();
            Stopwatch watch = new Stopwatch();
            DataStore.DisplayCurrentUser();


            watch.Start();

            GrammarFeeder.GrammarLoader(ref speechEngine);      //call to the public method of GrammarFeeder class 
            List<Grammar> grammarList = new List<Grammar>(speechEngine.Grammars);
            GrammarManipulator.RegisterWithManipulator(ref speechEngine);
            

            ProgramManager.InitializeManager();
            watch.Stop();
            waitHandle.WaitOne();

            speechEngine.SetInputToDefaultAudioDevice();

            MainWindow.initStopwatch.Stop();
            Console.WriteLine("time taken in the Init instantiator {0} ms", MainWindow.initStopwatch.ElapsedMilliseconds);
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);



            Console.ReadLine();         //keeps the console open during debugging

            foreach (var item in DataStore.RecentCommands)
            {
                Console.WriteLine(item);
            }

            DataStore.SaveUserSettings();

            foreach (var item in DataStore.Users)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }

    }
}
