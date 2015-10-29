/*
    description: GrammerManipulator class
           -static class
           -it handles the enabling/disabling and changing the priority of the Grammars loaded into a S.R.E 
    
    date created: -03/10/15

    log:-
        update 1 : 09/10/2015       Added functionality pertaining to CloseProgramGrammar

    Listed Public Methods:
           1. ResponseBoxLoaded and ResponseBoxDeloaded Pair 
                        ->for manipulation of ResponseBox grammar and the respective Open_type 
                        or Search_type grammar depending upon the ResponseBox type
           2. UILoaded and UiDeloaded Pair
                        ->for manipulation of UI grammar
           3. LoadNonOperativeCommands and DeloadNonOperativeCommands Pair
                        ->for manipulation of NonOperative grammar
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;
using System.Threading;

namespace Project_Voix
{
    static class GrammarManipulator
    {

        #region Fields
        static List<Grammar> listOfGrammars;
        const int highPriority = 15;
        static SpeechRecognitionEngine registeredEngine;
        #endregion

        #region Indexes
        static int indexOfBasic;
        static int indexOfUi;
        static int indexOfResponseBox;
        static int indexOfNonOperative;
        static int indexOfOpenType;
        static int indexOfcloseProgramGrammar;
        #endregion

        #region DefaultPriorities
        //private-set type properties to store the defalt priorities of the respective Grammars
        static private int DefaultUiPriority { get; set; }
        static private int DefaultBasicPriority { get; set; }
        static private int DefaultResponseBoxPriority { get; set; }
        static private int DefaultNonOperativePriority { get; set; }
        static private int DefaultOpenTypePriority { get; set; }
        static public int DefaultCloseProgramGrammarPriority { get; set; }
        #endregion

        #region Private Methods
        static private void IsRegistered()
        {
            if (registeredEngine == null)
            {
                throw new Exception(string.Format("The given SpeechRecognitionEngine object is not registered."));
            }
        }

        static private List<Grammar> GetGrammarList(ref SpeechRecognitionEngine speechEngine)
        {
            return new List<Grammar>(speechEngine.Grammars);
        }

        static private void SetDefaultGrammarStates()
        {

            foreach (Grammar g in listOfGrammars)
            {
                switch (g.Name)
                {
                    case "basicGrammar":
                        indexOfBasic = listOfGrammars.IndexOf(g);
                        g.Enabled = true;
                        DefaultBasicPriority = g.Priority;
                        break;
                    case "open_typeGrammar":
                        indexOfOpenType = listOfGrammars.IndexOf(g);
                        g.Enabled = false;
                        DefaultOpenTypePriority = g.Priority;
                        break;
                    case "responseBoxGrammar":
                        indexOfResponseBox = listOfGrammars.IndexOf(g);
                        g.Enabled = false;
                        DefaultResponseBoxPriority = g.Priority;
                        break;
                    case "NonOperativeCommands":
                        indexOfNonOperative = listOfGrammars.IndexOf(g);
                        g.Enabled = true;
                        DefaultNonOperativePriority = g.Priority;
                        break;
                    case "UIGrammar":
                        indexOfUi = listOfGrammars.IndexOf(g);
                        g.Enabled = true;
                        DefaultUiPriority = g.Priority;
                        break;
                    case "closeProgramGrammar":
                        indexOfcloseProgramGrammar = listOfGrammars.IndexOf(g);
                        g.Enabled = false;
                        DefaultCloseProgramGrammarPriority = g.Priority;
                        break;
                    default:
                        DataStore.AddToErrorLog("Grammar doesnt exits");
                        break;
                }
            }
        }
        private static void RegisteredEngine_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            DataStore.AddToMessageDump("SRE Update reached:");

            string qualifier;
            List<Grammar> grammars = new List<Grammar>(registeredEngine.Grammars);
            foreach (Grammar g in grammars)
            {
                qualifier = (g.Enabled) ? "enabled" : "disabled";
                DataStore.AddToMessageDump(string.Format("{0} grammar is loaded and {1}.",g.Name, qualifier));
            }

        }
        
        #endregion

        #region Public Methods

        static public void RegisterWithManipulator(ref SpeechRecognitionEngine speechEngine)
        {
            /*
                used by the S.R.E to allow manipulation of it's Grammars with the GrammarManipulator class
            */

            registeredEngine = speechEngine;
            registeredEngine.RecognizerUpdateReached += RegisteredEngine_RecognizerUpdateReached;
            listOfGrammars = GetGrammarList(ref registeredEngine);
            Task.Run(() =>
            {
                SetDefaultGrammarStates();
            });
        }

        #region Manipulation of Open_typeGrammar
        static public void EnableOpenGrammar()
        {
            //Console.WriteLine("In enable Open Grammar");
            //registeredEngine.RequestRecognizerUpdate();
            listOfGrammars[indexOfOpenType].Enabled = true;
            listOfGrammars[indexOfOpenType].Priority = highPriority;
            
        }

        static public void DisableOpenGrammar()
        {
            //IsRegistered();
            //registeredEngine.RequestRecognizerUpdate();
            //Console.WriteLine("In disable Open Grammar");

            listOfGrammars[indexOfOpenType].Enabled = false;
            listOfGrammars[indexOfOpenType].Priority = DefaultOpenTypePriority;
            
        }
        #endregion


        #region Manipulation on ResponseBoxes

        static public void ResponseBoxLoaded()
        {
            /*
                the actions that are to be executed when the Response Box is loaded
            */

            //MessageBox.Show("in GrammarManipulator.ResponseBoxLoaded()");
            IsRegistered();
            //Task.Run(() => { MessageBox.Show("in ResponseBoxLoaded"); });
            registeredEngine.RequestRecognizerUpdate();
            listOfGrammars[indexOfResponseBox].Enabled = true;
            listOfGrammars[indexOfResponseBox].Priority = highPriority;
            if (/*ResponseBoxType==Open_Type*/true)
            {
                listOfGrammars[indexOfBasic].Enabled = false;
                EnableOpenGrammar();
                if (listOfGrammars[indexOfcloseProgramGrammar].Enabled == true)
                    DisableCloseGrammar();
                if (listOfGrammars[indexOfNonOperative].Enabled == true)
                    DisableNonOperativeCommands();
            }

        }

        static public void ResponseBoxDeloaded()
        {
            /*
                Actions to be executed when the Response Box is unloaded
            */
            //MessageBox.Show("In GrammarManipulator.ResponseBoxDeloaded()");
            IsRegistered();
            registeredEngine.RequestRecognizerUpdate();

            listOfGrammars[indexOfBasic].Enabled = true;
            listOfGrammars[indexOfResponseBox].Enabled = false;
            listOfGrammars[indexOfResponseBox].Priority = DefaultResponseBoxPriority;
            if (/*ResponseBoxType==Open_Type*/true)
            {
                DisableOpenGrammar();
                //EnableCloseGrammar();
                if (listOfGrammars[indexOfNonOperative].Enabled == false)
                    EnableNonOperativeCommands();
            }
            //similarly for Search type

        }

        
        #endregion

        #region Manipulation on CloseGrammar
        static public void EnableCloseGrammar()
        {
            //Console.WriteLine("In enableCloseGrammar");
            //IsRegistered();
            registeredEngine.RequestRecognizerUpdate();

            listOfGrammars[indexOfcloseProgramGrammar].Enabled = true;
            listOfGrammars[indexOfcloseProgramGrammar].Priority = highPriority;

        }
        public static void DisableCloseGrammar()
        {
            IsRegistered();
            //Console.WriteLine("In disableCloseGrammar");
            registeredEngine.RequestRecognizerUpdate();

            listOfGrammars[indexOfcloseProgramGrammar].Enabled = false;
            listOfGrammars[indexOfcloseProgramGrammar].Priority = DefaultCloseProgramGrammarPriority;
            
        }
        #endregion

        #region Manipulation on UI loading
        static public void UILoaded()
        {
            /*
                Actions to be executed when the UI is loaded
            */

            //IsRegistered();
            //Console.WriteLine("In UILoaded");
            //registeredEngine.RequestRecognizerUpdate();

            listOfGrammars[indexOfUi].Enabled = true;
            listOfGrammars[indexOfUi].Priority = highPriority;

            if (listOfGrammars[indexOfNonOperative].Enabled == true)
                DisableNonOperativeCommands();
            

        }
        static public void UIDeloaded()
        {
            /*
                Actions to be executed when the UI is deloaded
            */
            //Console.WriteLine("In ui deloaded");
            IsRegistered();
            //registeredEngine.RequestRecognizerUpdate();
            listOfGrammars[indexOfUi].Enabled = false;
            listOfGrammars[indexOfUi].Priority = DefaultUiPriority;

            if (listOfGrammars[indexOfNonOperative].Enabled == true)
                EnableNonOperativeCommands();
            
        }
        #endregion


        #region Manipulation of Non Operative Commands
        static public void EnableNonOperativeCommands()
        {
            /*
                By default the NonOperative commands are enabled.
                This is to execute when any of the Deloaded functions are to be executed of Grammar Manipulation 
                It's priority remains fixed for most of the cases
            */
            //Console.WriteLine("In enableNonOperativeGrammar");
            IsRegistered();
            registeredEngine.RequestRecognizerUpdate();

            listOfGrammars[indexOfNonOperative].Enabled = true;

            //if (listOfGrammars[indexOfcloseProgramGrammar].Enabled == true)
              //  DisableCloseGrammar();
            
        }

        static public void DisableNonOperativeCommands()
        {
            /*
                By default the NonOperative commands are enabled.
                This is to execute in all other instances apart from the default instance Nonoperative Commands are to be deloaded
                It's priority remains fixed for most of the cases
            */
            //Console.WriteLine("In disableNonOperativeCOmmands");
            IsRegistered();
            registeredEngine.RequestRecognizerUpdate();
            listOfGrammars[indexOfNonOperative].Enabled = false;
         

        }
        #endregion
        #endregion
    }
}
