﻿/*
    description: Grammar Creation Program
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
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Project_Voix
{
    class Program
    {
        public static AutoResetEvent waitHandle = new AutoResetEvent(false);
        public static void Main(string[] args)
        {
            SpeechRecognitionEngine speechEngine = new SpeechRecognitionEngine();
            Stopwatch watch = new Stopwatch();
            UserSettings uSettings = new UserSettings("Sarthak Yadav",UserGender.Male);
            uSettings.WriteSettings(@"H:\voix");


            watch.Start();

            //Action<object> act = new Action<object>(loadGrammar);
            //Task.Factory.StartNew(act,speechEngine as object);

            GrammarFeeder.GrammarLoader(ref speechEngine);      //call to the public method of GrammarFeeder class 
            
            List<Grammar> grammarList=new List<Grammar>(speechEngine.Grammars);
            GrammarManipulator.RegisterWithManipulator(ref speechEngine);
            //GrammarManipulator.LoadNonOperativeCommands(ref speechEngine);
           
            ProgramManager.InitializeManager();
            watch.Stop();

            Console.WriteLine("total ticks {0}", watch.ElapsedMilliseconds);


            waitHandle.WaitOne();

            //ResponseGenerator.NonOperational_ResponseHandler(new Response(CommandType.NonOperational,DateTime.Now.TimeOfDay.Hours,"hello Tars"));
            //ProgramManager.SendOpenCommand("Notepad++");
            //ResponseGenerator.CloseProgram_ResponseHandler(new Response(CommandType.CloseProgram, DateTime.Now.TimeOfDay.Hours, "Close Notepad++"));
            //ProgramManager.SendCloseCommand("Close Notepad++");


            //Console.WriteLine("Doin it now5");
            //ResponseGenerator.NonOperational_ResponseHandler("tell me the time");
            speechEngine.SetInputToDefaultAudioDevice();
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
           
            //speechEngine.SpeechRecognized += SrEngine_SpeechRecognizedHandler;

            Console.ReadLine();         //keeps the console open during debugging
        }


        //event handler for SpeechRecognized event
        //public static void BasicGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    Console.WriteLine("In the basic grammar recognized handler");
        //    Console.WriteLine(e.Result.Text);
        //    //throw new NotImplementedException();
        //}
    }
}