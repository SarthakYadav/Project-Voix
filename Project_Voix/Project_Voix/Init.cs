/*
    static class Init
            - contains the heart of the program. The main thread which consists of the SpeechRecognitionEngine

     date created: 10/10/2015

     log:-
       Update 1: 15/10/2015          Author: Sarthak         Description: Fixed UI thread inaccessibility problems
       Update 2: 20/10/2015          Author: Sarthak         Description: Added support for DataStore Tasks

       latest update: 20/10/2015     Update 2            Author: Sarthak

     Public MEthods:
        - ProgramInit: starts the process of Recognition and fires up the required services

*/

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
        static SpeechRecognitionEngine speechEngine;

        static public SpeechRecognitionEngine Recognizer
        {
            get { return speechEngine; }
            set { speechEngine = value; }
        }
        public static AutoResetEvent waitHandle = new AutoResetEvent(false);
        public static AutoResetEvent waitHandle2 = new AutoResetEvent(false);
        
        public static void ProgramInit()
        {
            MainWindow.initStopwatch.Start();

            if (MainWindow.ct.IsCancellationRequested)
                return;


            Task.Run(() =>
            {
                DataStore.StartDataStoreManager();
            });

            waitHandle2.WaitOne();
            speechEngine = new SpeechRecognitionEngine();
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
            DataStore.AddToMessageDump(string.Format("time taken in the Init instantiator {0} ms", MainWindow.initStopwatch.ElapsedMilliseconds));
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public static void PauseRecog()
        {
            Recognizer.RecognizeAsyncStop();
        }
        public static void ResumeRecog()
        {
            Recognizer.RecognizeAsync();
        }
    }
}
