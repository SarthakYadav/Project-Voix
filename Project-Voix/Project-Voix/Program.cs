/*
    description: Old console testing component
    date created: 28/09/15

    log:-
        last update: 2/10/2015      Author: Sarthak
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Project_Voix
{
    class Program
    {
        public static AutoResetEvent waitHandle = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            SpeechRecognitionEngine speechEngine = new SpeechRecognitionEngine();
            Console.WriteLine("Main invoked on thread {0}",Thread.CurrentThread.ManagedThreadId);

            GrammarFeeder.GrammarLoader(ref speechEngine);      //call to the public method of GrammarFeeder class 
            
            List<Grammar> grammarList=new List<Grammar>(speechEngine.Grammars);
            

            GrammarManipulator.RegisterWithManipulator(ref speechEngine);
            GrammarManipulator.LoadNonOperativeCommands(ref speechEngine);
            Thread t = new Thread(new ThreadStart(ProgramManager.InitializeManager));
            t.Name = "ProgramManagerInitializerThread";
            t.Start();


            //ProgramManager.initManager init=new ProgramManager.initManager(ProgramManager.InitializeManager);
            //IAsyncResult iresult = init.BeginInvoke(new AsyncCallback(ProgramManager.initComplete), null);





            //ProgramManager.SendOpenCommand("7-Zip File Manager");

            //ProgramManager.ShowRunningExecutables();
            //ProgramManager.SendCloseCommand("7-Zip File Manager");
            //ProgramManager.ShowRunningExecutables();
            //ProgramManager.executableRecognized += ProgramManager_executableRecognized;
            
            speechEngine.SetInputToDefaultAudioDevice();
           
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            waitHandle.WaitOne();
            //speechEngine.SpeechRecognized += SrEngine_SpeechRecognizedHandler;

            Console.ReadLine();         //keeps the console open during debugging
        }

        

        //event handler for SpeechRecognized event
        public static void BasicGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("In the basic grammar recognized handler");
            Console.WriteLine(e.Result.Text);
            //throw new NotImplementedException();
        }
    }
}