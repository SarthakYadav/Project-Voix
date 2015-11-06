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
using System.IO;
using System.Windows;

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
            try
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
            catch (OutOfMemoryException outOfMemEx)
            {
                DataStore.AddToErrorLog(string.Format("Out of memory exception occured.---------\n Message : {0} \n StackTrace : {1}", outOfMemEx.Message, outOfMemEx.StackTrace));
                Task.Run(() =>
                {
                    MessageBox.Show("Critical Exception occured. {0} \n Application Restart Recommended", outOfMemEx.Message);
                });
            }
            catch (IOException ioEx)
            {
                DataStore.AddToErrorLog(string.Format("exception occured.---------\n Message : {0} \n StackTrace : {1}", ioEx.Message, ioEx.StackTrace));
            }
            catch (InvalidOperationException invalidOpEx)
            {
                DataStore.AddToErrorLog(string.Format("An exception occured.---------\n Message : {0} \n StackTrace : {1}",invalidOpEx.Message,invalidOpEx.StackTrace));
            }
            catch (NotSupportedException nsEx)
            {
                DataStore.AddToErrorLog(string.Format("An exception occured.---------\n Message : {0} \n StackTrace : {1}", nsEx.Message, nsEx.StackTrace));
            }
            catch (NullReferenceException nullRefEx)
            {
                DataStore.AddToErrorLog(string.Format("An exception occured.---------\n Message : {0} \n StackTrace : {1}",nullRefEx.Message, nullRefEx.StackTrace));
            }
            catch (WaitHandleCannotBeOpenedException waitHandleExcep)
            {
                DataStore.AddToErrorLog(string.Format("Critical Exception occured.---------\n Message : {0} \n StackTrace : {1}", waitHandleExcep.Message,waitHandleExcep.StackTrace));
                Task.Run(() =>
                {
                    MessageBox.Show("Critical Exception occured. {0} \n Application Restart Recommended", waitHandleExcep.Message);
                });
            }
            catch (ThreadStartException tsEx)
            {
                DataStore.AddToErrorLog(string.Format("Critical Exception occured.---------\n Message : {0} \n StackTrace : {1}", tsEx.Message, tsEx.StackTrace));
                Task.Run(() =>
                {
                    MessageBox.Show("Critical Exception occured. {0} \n Application Restart Recommended (if no crash occured)", tsEx.Message);
                });
            }
            catch(ApplicationException appEx)
            {
                DataStore.AddToErrorLog(string.Format("Application level Exception occured.---------\n Message : {0} \n StackTrace : {1}", appEx.Message, appEx.StackTrace));
                Task.Run(() =>
                {
                    MessageBox.Show("Application level Exception occured. {0} \n Application Restart Recommended (if no crash occured)", appEx.Message);
                });
            }
            catch (SystemException systemEx)
            {
                DataStore.AddToErrorLog(string.Format("System Level Exception occured.---------\n Message : {0} \n StackTrace : {1}", systemEx.Message, systemEx.StackTrace));
                Task.Run(() =>
                {
                    MessageBox.Show("System level Exception occured. {0} \n Non recoverable \n Application Restart Recommended (if no crash occured)", systemEx.Message);
                });
            }
            catch (Exception e)
            {
                DataStore.AddToErrorLog(string.Format("Exception occured.---------\n Message : {0} \n StackTrace : {1}", e.Message,e.StackTrace));

            }


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
