/*
    description: GrammerFeeder class
                -static class
                -Grammar creation,loading  the grammar into the S.R.E ,updating and compiling to .cfg(not added as of now,after testing) 
    
    date created: 28/09/15

    log:-
       Update 1: 2/10/2015          Author: Sarthak 
       Update 2: 9/10/2015          Author: Sarthak         Description: Added Grammar CloseProgramGrammar, Added Asynchronous Support


       latest update: 9/10/2015     Update 2            Author: Sarthak
    
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
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Project_Voix
{
    static class GrammarFeeder
    {
        #region Fields
        static GrammarBuilder optionalComponent = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);
        static Choices programChoices = null;
        static string[] programCommands = null;
        #endregion

        #region Public Methods

        public static void GrammarLoader(ref SpeechRecognitionEngine speechEngine)            //only method publically available
        {
            // main method that loads the S.R.E with all the grammars
            Grammar basicGrammar =Task.Factory.StartNew<Grammar>(new Func<Grammar>(BasicGrammar)).Result;
            basicGrammar.Name = "basicGrammar";
            basicGrammar.Priority = 10;
            basicGrammar.SpeechRecognized += BasicGrammar_SpeechRecognized;


            Grammar open_typeGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(OpenCommandGrammar)).Result;
            //Grammar open_typeGrammar = OpenCommandGrammar();
            open_typeGrammar.Name = "open_typeGrammar";
            open_typeGrammar.Priority = 8;
            open_typeGrammar.SpeechRecognized += Open_typeGrammar_SpeechRecognized;

            Grammar responseBoxGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(ResponseBoxSelection)).Result;
            //Grammar responseBoxGrammar = ResponseBoxSelection();
            responseBoxGrammar.Name = "responseBoxGrammar";
            responseBoxGrammar.Priority = 5;
            responseBoxGrammar.SpeechRecognized += ResponseBoxGrammar_SpeechRecognized;

            Grammar nonOperative = Task.Factory.StartNew<Grammar>(new Func<Grammar>(NonOperative)).Result;
            //Grammar nonOperative = NonOperative();
            nonOperative.Name = "NonOperativeCommands";
            nonOperative.Priority = 4;
            nonOperative.SpeechRecognized += NonOperative_SpeechRecognized;

            Grammar uiGrammar = Task.Run<Grammar>(new Func<Grammar>(UIGrammar)).Result;
            //Grammar uiGrammar = UIGrammar();
            uiGrammar.Name = "UIGrammar";
            uiGrammar.Priority = 3;
            uiGrammar.SpeechRecognized += UiGrammar_SpeechRecognized;

            Grammar closeProgramGrammar = Task.Factory.StartNew<Grammar>(new Func<Grammar>(CloseProgramGrammar)).Result;
            closeProgramGrammar.Name = "closeProgramGrammar";
            closeProgramGrammar.Priority = 6;
            closeProgramGrammar.SpeechRecognized += CloseProgramGrammar_SpeechRecognized;

            //loading all the grammars into the S.R.E
            speechEngine.LoadGrammarAsync(basicGrammar);
            speechEngine.LoadGrammarAsync(open_typeGrammar);
            speechEngine.LoadGrammarAsync(responseBoxGrammar);
            speechEngine.LoadGrammarAsync(nonOperative);
            speechEngine.LoadGrammarAsync(uiGrammar);
            speechEngine.LoadGrammarAsync(closeProgramGrammar);       
        }

        #endregion

        #region Private methods
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
            reanalyzeCommand.Append("Reanalyze");
            reanalyzeCommand.Append("the given input", 0, 1);

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

            //StreamWriter strWriter = new StreamWriter(@"H:\voix\txtFromOpenCommandGrammar.txt");

            //foreach (var item in programCommands)
            //{
            //    strWriter.WriteLine(item);
            //}
            //strWriter.Close();
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
                    1. Tars(optional) Refresh
                    2. Tars(optional) Quit
            */
            //Console.WriteLine("UI grammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(optionalComponent);
            Choices choiceUI = new Choices(new GrammarBuilder[] { "Refresh", "Quit" });
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

            Choices choice = new Choices(question1, question2);
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
                    1. Wake Up
                    2. Sleep
                    3. System Shutdown
                    4. System Restart
                    5. Search_Type command
                    6. Open_Type command
            */

            //Console.WriteLine("basic Grammar is on thread {0}",Thread.CurrentThread.ManagedThreadId);
            GrammarBuilder systemCommand = new GrammarBuilder("System");
            Choices sysPowerChoices = new Choices(new GrammarBuilder[] { "Shutdown", "Restart" });
            systemCommand.Append(sysPowerChoices);                  // System choices become System Shutdown/Restart

            Choices basicChoices = new Choices(new GrammarBuilder[] { "Wake Up", "Sleep" });
            basicChoices.Add(systemCommand);                        //basic choices become Wake up,Sleep,System Shutdown/Restart
            
            //creating for open_type and search_type respectively
            Choices search_typeChoices = new Choices(new GrammarBuilder[] { "Find", "Search", "Look for" });
            Choices open_typeChoices = new Choices(new GrammarBuilder[] { "Open", "Execute", "Run", "Intialize", "Start" });

            //all choices become a combination of Basic CHoices,Open_Type and Search_type choices
            Choices allchoices = new Choices(basicChoices,search_typeChoices, open_typeChoices);

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(optionalComponent);
            gb.Append(allchoices);                      // the grammar prototype becomes "(optional)Tars plus all choices"

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
        private static void BasicGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
                Console.WriteLine(e.Result.Text);

            //throw new NotImplementedException();
        }

        private static void Open_typeGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
            {
                Console.WriteLine(e.Result.Text);
                ProgramManager.SendOpenCommand(e.Result.Text);
            }
        }

        private static void NonOperative_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null)
                Console.WriteLine(e.Result.Text);
        }

        private static void CloseProgramGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(e.Result!=null)
                ProgramManager.SendCloseCommand(e.Result.Text);
            //throw new NotImplementedException();
        }

        private static void UiGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void ResponseBoxGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
