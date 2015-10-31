/*
    description: GrammarFeeder class
                -static class
                -Grammar creation,loading  the grammar into the S.R.E ,updating and compiling to .cfg(not added as of now,after testing) 
    
    date created: 28/09/15

    log:-
       Update 1: 2/10/2015          Author: Sarthak 
       Update 2: 9/10/2015          Author: Sarthak         Description: Added Grammar CloseProgramGrammar, Added Asynchronous Support
       Update 3: 11/10/2015         Author: Sarthak         Description: Added EventHandlers for the resp Grammar types
       update 4: 30/10/2015         Author: Sarthak         Description: Added all commands in the UI grammar and updated Respective Handlers

       latest update: 11/10/2015     Update 4          
    
    Listed Methods:
    1. public method GrammarLoader() takes reference of S.R.E. and loads all the required grammars into the engine
    2. method ResponseBoxSelection() returns grammar for the responseboxes
    3. method OpenCommandGrammar() returns grammar for the recognition of available Programs in the Start Menu
    4. method UIGrammar() returns a grammar for the Recognition of available commands when in the program GUI
    5. method NonOperative() returns a grammar for Non-operation oriented functionality of the program
    6. method BasicGrammar() returns the most basic set of available command choices that are available and active all the time
    7. method CloseProgramGrammar() returns a grammar that enables closing the currently running Executables by matching their resp commandName
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Speech.Recognition.SrgsGrammar;
using System.Windows;
using System.Xml;

namespace Project_Voix
{
    static class GrammarFeeder
    {
        #region Fields
        static bool RecogInProgress = true;
        static GrammarBuilder optionalComponent = null; 
        static Choices programChoices = null;
        static string[] programCommands = null;
        static SpeechRecognitionEngine speechEngine = null;
        static string openRecogPhrase;
        //static string moviesPath;
        //static ResponseBox resp = null;
             #endregion
        
        static GrammarFeeder()
        {
            //optionalComponent = new GrammarBuilder(new GrammarBuilder(DataStore.CurrentUser.AssistantName), 0, 1);
            openRecogPhrase = "";
        }

        static public event ChangeSlider UpdateSlider;
        static public event GenerateResponse BasicResponse;
        static public event GenerateResponse Open_SearchTypeResponse;
        static public event GenerateResponse ResponseBoxResponse;
        static public event GenerateResponse CloseProgramResponse;
        static public event GenerateResponse NonOperativeResponse;
        static public event GenerateResponse UIResponse;
        static public event GenerateResponse PrimaryResponse;

        static public event UpdateLog writeToTextBox;
        static public event StartResponseBox CallResponseBox;
        static public event ShowOpenTypeRecog RespBoxRecogDisplay;
        static public event CloseResponseBox CloseResponseBoxEvent;
        #region Public Methods
        public static void GrammarLoader(ref SpeechRecognitionEngine sre)            //only method publically available
        {

            // main method that loads the S.R.E with all the grammars
            speechEngine = sre;
            #region Grammar for Basic Commands
            Grammar basicGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(BasicGrammar)).Result;
            basicGrammar.Name = "basicGrammar";
            basicGrammar.Priority = 10;
            basicGrammar.SpeechRecognized += BasicGrammar_SpeechRecognized;
            BasicResponse += ResponseGenerator.BasicGrammar_ResponseHandler;
            #endregion

            #region Grammar for Primary Commands
            Grammar primaryGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(PrimaryGrammar)).Result;
            primaryGrammar.Name = "primaryGrammar";
            primaryGrammar.Priority = 12;
            primaryGrammar.SpeechRecognized += PrimaryGrammar_SpeechRecognized;
            PrimaryResponse += ResponseGenerator.PrimaryGrammar_ResponseHandler;
            #endregion

            #region Grammar for Open Type commands
            Grammar open_typeGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(OpenCommandGrammar)).Result;
            open_typeGrammar.Name = "open_typeGrammar";
            open_typeGrammar.Priority = 8;
            open_typeGrammar.SpeechRecognized += Open_typeGrammar_SpeechRecognized;
            Open_SearchTypeResponse += ResponseGenerator.Open_SearchType_ResponseHandler;
            #endregion

            #region Grammar for Response Box
            Grammar responseBoxGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(ResponseBoxSelection)).Result;
            responseBoxGrammar.Name = "responseBoxGrammar";
            responseBoxGrammar.Priority = 5;
            responseBoxGrammar.SpeechRecognized += ResponseBoxGrammar_SpeechRecognized;
            ResponseBoxResponse += ResponseGenerator.ResponseBox_ResponseHandler;
            #endregion

            #region Grammar for NonOperative Commands
            Grammar nonOperative = Task.Factory.StartNew<Grammar>(new Func<Grammar>(NonOperative)).Result;
            nonOperative.Name = "NonOperativeCommands";
            nonOperative.Priority = 4;
            nonOperative.SpeechRecognized += NonOperative_SpeechRecognized;
            NonOperativeResponse += ResponseGenerator.NonOperational_ResponseHandler;
            #endregion

            #region Grammar for UI commands
            Grammar uiGrammar = Task.Run<Grammar>(new Func<Grammar>(UIGrammar)).Result;
            uiGrammar.Name = "UIGrammar";
            uiGrammar.Priority = 3;
            uiGrammar.SpeechRecognized += UiGrammar_SpeechRecognized;
            UIResponse += ResponseGenerator.UI_ResponseHandler;
            #endregion

            #region Grammar for Close command
            Grammar closeProgramGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(CloseProgramGrammar)).Result;
            closeProgramGrammar.Name = "closeProgramGrammar";
            closeProgramGrammar.Priority = 6;
            closeProgramGrammar.SpeechRecognized += CloseProgramGrammar_SpeechRecognized;
            CloseProgramResponse += ResponseGenerator.CloseProgram_ResponseHandler;
            #endregion

            #region Loading all Grammars in the SRE
            //loading all the grammars into the S.R.E
            speechEngine.LoadGrammarAsync(primaryGrammar);
            speechEngine.LoadGrammarAsync(basicGrammar);
            speechEngine.LoadGrammarAsync(open_typeGrammar);
            speechEngine.LoadGrammarAsync(responseBoxGrammar);
            speechEngine.LoadGrammarAsync(nonOperative);
            speechEngine.LoadGrammarAsync(uiGrammar);
            speechEngine.LoadGrammarAsync(closeProgramGrammar);
            #endregion
        }

        

        public static void SetAssistantName(string name)
        {
            optionalComponent = new GrammarBuilder(new GrammarBuilder(name), 0, 1);
        }
        //public static void SetMoviesPath(string path)
        //{
        //    moviesPath = path;
        //}
        #endregion

        #region Private methods
        private static Grammar PrimaryGrammar()
        {
            /*
                This is the Outermost, highest Priority grammar which is always enabled no matter what
                It has commands for halting recognition by disabling all other grammars and storing their state 
                and re-enabling recognition by restoring the states of the various grammars 
                at the time through restoring their states as what they were prior to halting.
            */
            Choices haltChoice = new Choices(new GrammarBuilder[] { "Sleep", "Pause" });                            //commands to halt/pause recognition
            Choices resumeChoices = new Choices(new GrammarBuilder[] { "Resume Recognition", "Wake up" });          //commands to resume recognition
            Choices allchoices = new Choices(haltChoice,resumeChoices);
            GrammarBuilder primaryGrammarBuilder = GrammarBuilder.Add(optionalComponent, allchoices);

            //creating an Srgs compliant .xml grammar file
            //SrgsDocument responseBoxDoc = new SrgsDocument(primaryGrammarBuilder);
            //XmlWriter writer = XmlWriter.Create(@"H:\voix\PrimaryGrammar.xml");
            //responseBoxDoc.WriteSrgs(writer);
            //writer.Close();
            return new Grammar(primaryGrammarBuilder);
        }

        //private static Grammar PlayMoviesGrammar()
        //{
        //    /*
        //        to be loaded only if the user specifies a Folder where he stores his Movies
        //    */

        //    string[] moviesCommands;
        //    Choices movieChoices = new Choices();
        //    return null;
        //}
        private static Grammar ResponseBoxSelection()
        {
            /* To create grammar of the Response box's selection options
                list of commands:
                    1. Ok
                    2. Cancel
                    3. Reanalyze +the given input
            */
            //Console.WriteLine("REsponseBox Grammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            GrammarBuilder reanalyzeCommand = new GrammarBuilder();
            reanalyzeCommand.Append("Reinitialize");
            reanalyzeCommand.Append("Recognition", 0, 1);

            Choices ResponseBoxChoices = new Choices(new GrammarBuilder[] { "Ok", "Cancel", reanalyzeCommand });

            GrammarBuilder ResponseBoxBuilder = GrammarBuilder.Add(optionalComponent, ResponseBoxChoices);

            //creating an Srgs compliant .xml grammar file
            //SrgsDocument responseBoxDoc = new SrgsDocument(ResponseBoxBuilder);
            //XmlWriter writer = XmlWriter.Create(@"H:\voix\responseBoxSelectionGrammar.xml");
            //responseBoxDoc.WriteSrgs(writer);
            //writer.Close();

            return new Grammar(ResponseBoxBuilder);
        }

        //grammar that recognizes the Open_Type command parameter   (the program to open)
        private static Grammar OpenCommandGrammar()
        {
            /*
                Returns the grammar which consists of Identifier names for the Programs available in start menu of a system
                which can be recognized by the S.R.E whilst in the open_type response box
            */

            //Console.WriteLine("OpenCommandGrammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            programCommands = Utilities.CommandList();


            programChoices = new Choices(programCommands);
            GrammarBuilder programGrammar = new GrammarBuilder();
            programGrammar.Append(optionalComponent);
            programGrammar.Append(programChoices);

            ////Srgs file creation
            //SrgsDocument doc = new SrgsDocument(programGrammar);
            //XmlWriter writer = XmlWriter.Create(@"H:\voix\open_typeGrammar.xml");
            //doc.WriteSrgs(writer);
            //writer.Close();

            return new Grammar(programGrammar);

        }
        private static Grammar UIGrammar()
        {
            /* creates grammar that is to be loaded only when UI is selected
                list:
                    1. Add User
                    2. Select User
                    3. Expand Expander
                    4. Increase Synthesizer Volume
                    5. Increase Synthesizer Rate 
                    6. Decrease Synthesizer Volume
                    7. Decrease Sythesizer Rate
                    8. Exit
            */
            //Console.WriteLine("UI grammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(optionalComponent);
            Choices choiceUI = new Choices();
            choiceUI.Add(new GrammarBuilder[] 
            {
                "Add User","Select User",
                "Expand Expander","Increase Synthesizer Volume",
                "Increase Synthesizer Rate","Decrease Synthesizer Volume","Decrease Synthesizer Rate",
                "Exit"
            });
            gb.Append(choiceUI);

            //to save the grammar as an SrgsDocument
            //SrgsDocument basicGrammar = new SrgsDocument(gb);
            //System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\UICommands.xml");
            //basicGrammar.WriteSrgs(writer);
            //writer.Close();

            return new Grammar(gb);

        }

        private static Grammar NonOperative()
        {

            /*
                Grammar for non operative commands
                list:
                    1. Greeting
                            -Hello Tars(optional)
                    2. Questions
                            - Tars(optional) What is the day/date today(optional)
                            - tars(optional) What/Tell is/me the time
            */
            //Console.WriteLine("NonOperative Grammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            //GrammarBuilder optional = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);
            GrammarBuilder todayOptional = new GrammarBuilder(new GrammarBuilder("Today"), 0, 1);
            Choices day_date = new Choices(new GrammarBuilder[] { "day", "date" });

            GrammarBuilder greeting = new GrammarBuilder("Hello");
            greeting.Append(optionalComponent);

            GrammarBuilder question1 = new GrammarBuilder();
            question1.Append(optionalComponent);
            //question1.Append(new GrammarBuilder("please", 0, 1));
            question1.Append("what is the");
            question1.Append(day_date);
            question1.Append(todayOptional);

            GrammarBuilder question2 = new GrammarBuilder();
            question2.Append(optionalComponent);
            //question1.AppendWildcard();

            question2.Append(new GrammarBuilder("please", 0, 1));
            question2.Append(new Choices(new GrammarBuilder[] { "what is", "tell me" }));
            question2.Append("the time");

            Choices choice = new Choices(greeting, question1, question2);
            GrammarBuilder gb = new GrammarBuilder(choice);
            //to save the grammar as an SrgsDocument
            //SrgsDocument basicGrammar = new SrgsDocument(gb);
            //System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\NonOperativeCommands.xml");
            //basicGrammar.WriteSrgs(writer);
            //writer.Close();

            return new Grammar(gb);
        }
        private static Grammar CloseProgramGrammar()
        {
            /*
                Grammar to close a running Executable depending on the command name recognized

            */

            GrammarBuilder closeProgramGrammar = new GrammarBuilder();
            closeProgramGrammar.Append(optionalComponent);
            closeProgramGrammar.Append("Close");
            closeProgramGrammar.Append(programChoices);

            //to save the grammar as an SrgsDocument
            //SrgsDocument basicGrammar = new SrgsDocument(closeProgramGrammar);
            //System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\CloseProgramGrammar.xml");
            //basicGrammar.WriteSrgs(writer);
            //writer.Close();

            return new Grammar(closeProgramGrammar);

        }
        private static Grammar BasicGrammar()
        {
            /*
                    Most fundamental operational commands of the program

                    command list:
                    (optional) Tars
                    1. System Shutdown
                    2. System Restart
                    3. Search_Type command
                    4. Open_Type command
            */

            //Console.WriteLine("basic Grammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            GrammarBuilder systemCommand = new GrammarBuilder("System");
            Choices sysPowerChoices = new Choices(new GrammarBuilder[] { "Shutdown", "Restart" });
            systemCommand.Append(sysPowerChoices);                  // System choices become System Shutdown/Restart

            //Choices basicChoices = new Choices(new GrammarBuilder[] { "Wake Up", "Sleep" });
            //basicChoices.Add(systemCommand);                        //basic choices become Wake up,Sleep,System Shutdown/Restart

            //creating for open_type and search_type respectively
            Choices search_typeChoices = new Choices(new GrammarBuilder[] {"Search the web", "Search the internet" });
            Choices open_typeChoices = new Choices(new GrammarBuilder[] { "Open", "Execute", "Run", "Intialize", "Start" });

            //all choices become a combination of Basic CHoices,Open_Type and Search_type choices
            Choices allchoices = new Choices(search_typeChoices, open_typeChoices);

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(optionalComponent);
            gb.Append(allchoices);                      // the grammar prototype becomes "(optional)Tars plus all choices"
            //gb.Append("demo");
            //to save the grammar as an SrgsDocument
            //SrgsDocument basicGrammar = new SrgsDocument(gb);
            //System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\basicCommands.xml");
            //basicGrammar.WriteSrgs(writer);
            //writer.Close();

            //finally creating and returning the grammar
            return new Grammar(gb);
        }

        #endregion

        #region Grammar Event Handlers
        private static void PrimaryGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
            if (e.Result!=null)
            {
                Task.Run(() =>
                {
                    writeToTextBox(e.Result.Text);
                });
                DataStore.AddRecentCommand(e.Result.Text);
                try
                {
                    if (e.Result.Text.Contains("Sleep") | e.Result.Text.Contains("Pause"))
                    {
                        if (RecogInProgress == true)
                        {
                            GrammarManipulator.HaltRecognition();
                            PrimaryResponse(new Response(CommandType.Primary, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
                            RecogInProgress = false;
                        }
                        else
                            PrimaryResponse(new Response(CommandType.Primary, DateTime.Now.TimeOfDay.Hours, "already paused"));
                    }

                    else if (e.Result.Text.Contains("Resume Recognition") | e.Result.Text.Contains("Wake up"))
                    {
                        if (RecogInProgress == false)
                        {
                            GrammarManipulator.ResumeRecognition();
                            PrimaryResponse(new Response(CommandType.Primary, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
                            RecogInProgress = true;
                        }
                        else
                            PrimaryResponse(new Response(CommandType.Primary, DateTime.Now.TimeOfDay.Hours, "recognition already in progress"));
                    }
                    else
                        throw new Exception("Unknown recognition by PrimaryGrammar ");
                }
                catch (Exception ex)
                {
                    DataStore.AddToErrorLog(ex.Message);
                }
            }
        }

        private static void BasicGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                DataStore.AddRecentCommand(e.Result.Text);
                DataStore.AddToMessageDump(e.Result.Text);
                if (e.Result.Text.Contains("Open") | e.Result.Text.Contains("Execute") | e.Result.Text.Contains("Run") | e.Result.Text.Contains("Intialize") | e.Result.Text.Contains("Start"))
                {
                    Task.Run(() =>
                    {
                        writeToTextBox(e.Result.Text);
                    });

                    try
                    {
                        ResponseBox.CreateResponseBox();
                    }

                    catch (Exception ex)
                    {
                        DataStore.AddToErrorLog(string.Format("Main exception {0}\nMain exception stack trace {1}\nInner exception {2}\ninner Exception stack trace {3}", ex.Message, ex.StackTrace, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }

                    try
                    {
                        BasicResponse(new Response(CommandType.Open, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
                    }
                    catch (Exception excep)
                    {
                        DataStore.AddToErrorLog(string.Format("Main exception {0}\nMain exception stack trace {1}\nInner exception {2}\ninner Exception stack trace {3}", excep.Message, excep.StackTrace, excep.InnerException.Message, excep.InnerException.StackTrace));
                    }
                }
                else if (e.Result.Text.Contains("Search the web") | e.Result.Text.Contains("Search the internet"))
                {
                    BasicResponse(new Response(CommandType.Search, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
                    ProgramManager.SendOpenCommand("Google Chrome");
                }
                else
                {
                    BasicResponse(new Response(CommandType.Basic, DateTime.Now.TimeOfDay.Hours, e.Result.Text));

                }
            }
        }

        private static void Open_typeGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                openRecogPhrase = e.Result.Text;

                Task.Run(() =>
                {
                    writeToTextBox(e.Result.Text);
                });
                DataStore.AddRecentCommand(e.Result.Text);
                try
                {
                    RespBoxRecogDisplay(openRecogPhrase);
                }
                catch (Exception exception)
                {
                    DataStore.AddToErrorLog(exception.Message);
                }
                Open_SearchTypeResponse(new Response(CommandType.Open, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
            }
        }

        private static void NonOperative_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                DataStore.AddRecentCommand(e.Result.Text);
                writeToTextBox(e.Result.Text);
                try
                {
                    NonOperativeResponse(new Response(CommandType.NonOperational, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
                }
                catch(Exception exception)
                {
                    DataStore.AddToErrorLog(string.Format("Main exception {0}\nMain exception stack trace {1}\nInner exception {2}\ninner Exception stack trace {3}", exception.Message, exception.StackTrace, exception.InnerException.Message, exception.InnerException.StackTrace));
                }
            }
        }

        private static void CloseProgramGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                writeToTextBox(e.Result.Text);
                DataStore.AddRecentCommand(e.Result.Text);
                ProgramManager.SendCloseCommand(e.Result.Text);
                CloseProgramResponse(new Response(CommandType.CloseProgram, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
            }
        }

        private static void UiGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                Task.Run(() =>
                {
                    writeToTextBox(e.Result.Text);
                });
                
                DataStore.AddRecentCommand(e.Result.Text);
                try
                {
                    if (e.Result.Text == "Add User")
                        AddUser.OpenAddUserDialog();                            //opens a new AddUser Dialog box on a new thread
                    else if (e.Result.Text == "Select User")
                        SelectUser.OpenUserSelectWindow();                      //opens a new SelectUser Dialog box on a new thread
                    else if (e.Result.Text == "Expand Expander")
                        ;
                    else if (e.Result.Text == "Increase Synthesizer Volume")
                        UpdateSlider(1,"volume");                               //INCREASES the Synthesizer Volume slider by 1 point
                    else if (e.Result.Text == "Increase Synthesizer Rate")
                        UpdateSlider(1,"rate");                                 //INCREASES the Synthesizer Rate Slider by 1 point
                    else if (e.Result.Text == "Decrease Synthesizer Volume")
                        UpdateSlider(-1,"volume");                              //DECREASES the Synthesizer Volume slider by 1 point
                    else if (e.Result.Text == "Decrease Synthesizer Rate")
                        UpdateSlider(-1,"rate");                                //DECREASES the Synthesizer Volume slider by 1 point
                    else if (e.Result.Text == "Exit")
                        ;
                    else
                        throw new Exception("Unknown recognition");
                }
                catch (Exception exception)
                {
                    DataStore.AddToErrorLog(string.Format("Exception occured---\n Message : {0}\n StackTrace : {1}",exception.Message,exception.StackTrace));
                }
                //do UI refreshing here
                UIResponse(new Response(CommandType.UI, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
            }
        }

        private static void ResponseBoxGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                Task.Run(() =>
                {
                    writeToTextBox(e.Result.Text);
                });
                
                DataStore.AddRecentCommand(e.Result.Text);

                if (e.Result.Text == "Ok")
                {
                    ProgramManager.SendOpenCommand(openRecogPhrase);
                    GrammarManipulator.EnableCloseGrammar();
                    CloseResponseBoxEvent();
                }
                else if(e.Result.Text=="Cancel")
                {
                    CloseResponseBoxEvent();
                }
                else
                {
                    writeToTextBox("");
                    //remove the text in the analyzed textbox
                }
                ResponseBoxResponse(new Response(CommandType.ResponseBox, DateTime.Now.TimeOfDay.Hours, e.Result.Text));
            }
        }
        #endregion
    }
}
