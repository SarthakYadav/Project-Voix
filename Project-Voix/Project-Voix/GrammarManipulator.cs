/*
    description: GrammerManipulator class
           -static class
           -it handles the enabling/disabling and changing the priority of the Grammars loaded into a S.R.E 
    
    date created: -03/10/15

    log:-
    *No Updates Done*

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
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        #region DefaultPriorities
        //private-set type properties to store the defalt priorities of the respective Grammars
        static private int DefaultUiPriority { get; set; }
        static private int DefaultBasicPriority { get; set; }
        static private int DefaultResponseBoxPriority { get; set; }
        static private int DefaultNonOperativePriority { get; set; }
        static private int DefaultOpenTypePriority { get;  set; }
        #endregion

        #region Private Methods
        static private void IsRegistered(ref SpeechRecognitionEngine sre)
        {
            if (registeredEngine != sre)
            {
                throw new Exception(string.Format("The given SpeechRecognitionEngine object {0} is not registered.", sre));
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
                        g.Enabled = false;
                        DefaultUiPriority = g.Priority;
                        break;

                    default:
                        Console.WriteLine("Grammar doesnt exits");
                        break;
                }
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
            listOfGrammars = GetGrammarList(ref registeredEngine);
            SetDefaultGrammarStates();
        }
        #region Manipulation on ResponseBoxes
        static public void ResponseBoxLoaded(ref SpeechRecognitionEngine sre)
        {
            /*
                the actions that are to be executed when the Response Box is loaded
            */
            IsRegistered(ref sre);           
            
            listOfGrammars[indexOfResponseBox].Enabled = true;
            listOfGrammars[indexOfResponseBox].Priority = highPriority;

            if (/*ResponseBoxType==Open_Type*/true)
            {
                listOfGrammars[indexOfOpenType].Enabled = true;
                listOfGrammars[indexOfOpenType].Priority = highPriority;
            }
            //similarly for search type
            DeloadNonOperativeCommands(ref sre);
        }

        static public void ResponseBoxDeloaded(ref SpeechRecognitionEngine sre)
        {
            /*
                Actions to be executed when the Response Box is unloaded
            */
            IsRegistered(ref sre);
            listOfGrammars[indexOfResponseBox].Enabled = false;
            listOfGrammars[indexOfResponseBox].Priority = DefaultResponseBoxPriority;
            if (/*ResponseBoxType==Open_Type*/true)
            {
                listOfGrammars[indexOfOpenType].Enabled = false;
                listOfGrammars[indexOfOpenType].Priority = DefaultOpenTypePriority;
            }
            //similarly for Search type
            LoadNonOperativeCommands(ref sre);
        }
        #endregion

        #region Manipulation on UI loading
        static public void UILoaded(ref SpeechRecognitionEngine sre)
        {
            /*
                Actions to be executed when the UI is loaded
            */
            IsRegistered(ref sre);
            listOfGrammars[indexOfUi].Enabled = true;
            listOfGrammars[indexOfUi].Priority = highPriority;
            DeloadNonOperativeCommands(ref sre);
        }
        static public void UIDeloaded(ref SpeechRecognitionEngine sre)
        {
            /*
                Actions to be executed when the UI is deloaded
            */
            IsRegistered(ref sre);
            listOfGrammars[indexOfUi].Enabled = false;
            listOfGrammars[indexOfUi].Priority = DefaultUiPriority;
            LoadNonOperativeCommands(ref sre);
        }
        #endregion


        #region Manipulation on Non Operative Commands
        static public void LoadNonOperativeCommands(ref SpeechRecognitionEngine sre)
        {
            /*
                By default the NonOperative commands are enabled.
                This is to execute when any of the Deloaded functions are to be executed of Grammar Manipulation 
                It's priority remains fixed for most of the cases
            */
            IsRegistered(ref sre);
            listOfGrammars[indexOfNonOperative].Enabled = true;
        }

        static public void DeloadNonOperativeCommands(ref SpeechRecognitionEngine sre)
        {
            /*
                By default the NonOperative commands are enabled.
                This is to execute in all other instances apart from the default instance Nonoperative Commands are to be deloaded
                It's priority remains fixed for most of the cases
            */
            IsRegistered(ref sre);
            listOfGrammars[indexOfNonOperative].Enabled = false;
        }
        #endregion
        #endregion
    }
}
