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
        //[STAThread]
        public static void StartInit()
        {
            //DataStore.LoadUserSettings();

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
            //GrammarManipulator.LoadNonOperativeCommands(ref speechEngine);

            ProgramManager.InitializeManager();
            watch.Stop();

            //Console.WriteLine("total ticks {0}", watch.ElapsedMilliseconds);


            waitHandle.WaitOne();

            //ResponseGenerator.NonOperational_ResponseHandler(new Response(CommandType.NonOperational,DateTime.Now.TimeOfDay.Hours,"hello Tars"));
            ProgramManager.SendOpenCommand("Notepad++");
            //ResponseGenerator.CloseProgram_ResponseHandler(new Response(CommandType.CloseProgram, DateTime.Now.TimeOfDay.Hours, "Notepad++"));
            //Thread.Sleep(2000);
            ProgramManager.SendCloseCommand("Tars Close Notepad++");


            //Console.WriteLine("Doin it now5");
            //ResponseGenerator.NonOperational_ResponseHandler("tell me the time");
            speechEngine.SetInputToDefaultAudioDevice();
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
