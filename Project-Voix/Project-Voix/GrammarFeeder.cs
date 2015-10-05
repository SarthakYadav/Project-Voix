/*
    description: GrammerFeeder class
                -static class
                -Grammar creation,loading  the grammar into the S.R.E ,updating and compiling to .cfg(not added as of now,after testing) 
    
    date created: 28/09/15

    log:-
       Update1: 2/10/2015       Author: Sarthak 
       latest update: 2/10/2015      Author: Sarthak

    Listed Methods:
    1. public method GrammarLoader() takes reference of S.R.E. and loads all the required grammars into the engine
    2. method ResponseBoxSelection() returns grammar for the responseboxes
    3. method OpenCommandGrammar() returns grammar for the recognition of available Programs in the Start Menu
    4. method UIGrammar() returns a grammar for the Recognition of available commands when in the program GUI
    5. method NonOperative() returns a grammar for Non-operation oriented functionality of the program
    6. method BasicGrammar() returns the most basic set of available command choices that are available and active all the time
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Project_Voix
{
    static class GrammarFeeder
    {
        #region Public Methods
        
        public static void GrammarLoader(ref SpeechRecognitionEngine speechEngine)            //only method publically available
        {
            // main method that loads the S.R.E with all the grammars
            Grammar basicGrammar = BasicGrammar();
            basicGrammar.Name = "basicGrammar";
            basicGrammar.Priority = 10;
            basicGrammar.SpeechRecognized += BasicGrammar_SpeechRecognized;
            

            Grammar open_typeGrammar = OpenCommandGrammar();
            open_typeGrammar.Name = "open_typeGrammar";
            open_typeGrammar.Priority = 6;

            Grammar responseBoxGrammar = ResponseBoxSelection();
            responseBoxGrammar.Name = "responseBoxGrammar";
            responseBoxGrammar.Priority = 5;

            Grammar nonOperative = NonOperative();
            nonOperative.Name = "NonOperativeCommands";
            nonOperative.Priority = 4;
            
            Grammar uiGrammar = UIGrammar();
            uiGrammar.Name = "UIGrammar";
            uiGrammar.Priority = 3;

            speechEngine.LoadGrammarAsync(basicGrammar);
            speechEngine.LoadGrammarAsync(open_typeGrammar);
            speechEngine.LoadGrammarAsync(responseBoxGrammar);
            speechEngine.LoadGrammarAsync(nonOperative);
            speechEngine.LoadGrammarAsync(uiGrammar);            
        }

        private static void BasicGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
            //throw new NotImplementedException();
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
            GrammarBuilder reanalyzeCommand = new GrammarBuilder();
            reanalyzeCommand.Append("Reanalyze");
            reanalyzeCommand.Append("the given input", 0, 1);

            Choices ResponseBoxChoices = new Choices(new GrammarBuilder[] { "Ok", "Cancel", reanalyzeCommand });
            GrammarBuilder optional = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);

            GrammarBuilder ResponseBoxBuilder = GrammarBuilder.Add(optional, ResponseBoxChoices);

            //creating an Srgs compliant .xml grammar file
            SrgsDocument responseBoxDoc = new SrgsDocument(ResponseBoxBuilder);
            XmlWriter writer = XmlWriter.Create(@"H:\voix\responseBoxSelectionGrammar.xml");
            responseBoxDoc.WriteSrgs(writer);
            writer.Close();

            return new Grammar(ResponseBoxBuilder);
        }
        
        //grammar that recognizes the Open_Type command parameter   (the program to open)
        private static Grammar OpenCommandGrammar()
        {
            /*
                Returns the grammar which consists of Identifier names for the Programs available in start menu of a system
                which can be recognized by the S.R.E whilst in the open_type response box
            */
            string[] programCommands = Utilities.CommandList();
            Choices programChoices = new Choices(programCommands);
            GrammarBuilder programGrammar = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);
            programGrammar.Append(programChoices);

            //Srgs file creation
            SrgsDocument doc = new SrgsDocument(programGrammar);
            XmlWriter writer = XmlWriter.Create(@"H:\voix\open_typeGrammar.xml");
            doc.WriteSrgs(writer);
            writer.Close();

            return new Grammar(programGrammar);

        }
        private static Grammar UIGrammar()
        {
            /* creates grammar that is to be loaded only when UI is selected
                list:
                    1. Tars(optional) Refresh
                    2. Tars(optional) Quit
            */
            GrammarBuilder gb = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);
            Choices choiceUI = new Choices(new GrammarBuilder[] { "Refresh", "Quit" });
            gb.Append(choiceUI);

            //to save the grammar as an SrgsDocument
            SrgsDocument basicGrammar = new SrgsDocument(gb);
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\UICommands.xml");
            basicGrammar.WriteSrgs(writer);
            writer.Close();
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
            GrammarBuilder optional = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);
            GrammarBuilder todayOptional = new GrammarBuilder(new GrammarBuilder("Today"), 0, 1);
            Choices day_date = new Choices(new GrammarBuilder[] { "day", "date" });

            GrammarBuilder greeting = new GrammarBuilder("Hello");
            greeting.Append(optional);

            GrammarBuilder question1 = new GrammarBuilder();
            question1.Append(optional);
            //question1.Append(new GrammarBuilder("please", 0, 1));
            question1.Append("what is the");
            question1.Append(day_date);
            question1.Append(todayOptional);

            GrammarBuilder question2 = new GrammarBuilder();
            question2.Append(optional);
            //question1.AppendWildcard();

            question2.Append(new GrammarBuilder("please", 0, 1));
            question2.Append(new Choices(new GrammarBuilder[] { "what is", "tell me" }));
            question2.Append("the time");

            Choices choice = new Choices(question1, question2);
            GrammarBuilder gb = new GrammarBuilder(choice);
            //to save the grammar as an SrgsDocument
            SrgsDocument basicGrammar = new SrgsDocument(gb);
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\NonOperativeCommands.xml");
            basicGrammar.WriteSrgs(writer);
            writer.Close();

            return new Grammar(gb);
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

            GrammarBuilder gb = new GrammarBuilder(new GrammarBuilder("Tars"), 0, 1);
            gb.Append(allchoices);                      // the grammar prototype becomes "(optional)Tars plus all choices"

            //to save the grammar as an SrgsDocument
            SrgsDocument basicGrammar = new SrgsDocument(gb);
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(@"H:\voix\basicCommands.xml");
            basicGrammar.WriteSrgs(writer);
            writer.Close();

            //finally creating and returning the grammar
            return new Grammar(gb);
        }
        #endregion
    }
}
