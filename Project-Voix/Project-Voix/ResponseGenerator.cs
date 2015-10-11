/*
    description: ResponseGenerator class
           -works on Response objects
           - Manages creation of appopriate responses, handling of response_generation events and sending apt responses for synthesis
    
    date created: -11/10/15

    log:-
    *No Updates Done*

    Listed Public Methods:
           1. Event Handlers for all Response events
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
namespace Project_Voix
{
    static class ResponseGenerator
    {
        static SpeechSynthesizer synth = new SpeechSynthesizer();

        #region Private Methods
        static int RandomizeResponse(List<string> list)
        {
            /*
                For randomizing the response provided for the Greetings style and Open/Search type commands
            */
            int i=0;
            if (list != null)
            {
                Random rand = new Random();
                i = rand.Next(0, list.Count);
            }
            
            else
                throw new ArgumentException("Null reference. Check your arguments");
            return i;
        }

        static void SendForSynthesis(string command)
        {
            /*
                Imitation of a method that would send Synthesis commands over to the Synthesizer thread
            */
            synth.Speak(command);
        }

        #endregion


        #region GenerateResponse Event Handlers

        public static void BasicGrammar_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to Basic Grammar type commands
            
            */

            resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
            if (resp.CommandType == CommandType.Basic)
            {
                if (resp.RecognizedPhrase.Contains("wake up"))
                    SendForSynthesis("Tars Awake");
                if (resp.RecognizedPhrase.Contains("sleep"))
                    SendForSynthesis("Tars Asleep");
                if (resp.RecognizedPhrase.Contains("System Shutdown"))
                    SendForSynthesis("System shutdown requested");
                if (resp.RecognizedPhrase.Contains("System Restart"))
                    SendForSynthesis("System restart requested");
            }
            else if (resp.CommandType == CommandType.Open | resp.CommandType == CommandType.Search)     //"Open", "Execute", "Run", "Intialize", "Start"
            {
                int responseIndex = RandomizeResponse(resp.OpenSearchResponses);
                resp.SynthesisOutput = resp.OpenSearchResponses[responseIndex];
            }
            else
                throw new InvalidOperationException("Wrong CommandType of the respective Response object");

        }

        static public void NonOperational_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to NonOperational type commands
                
            */
            if (resp.CommandType != CommandType.NonOperational)
                throw new InvalidOperationException("Wrong CommandType for the input response class");
            else
            {
                resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
                if (resp.RecognizedPhrase.Contains("what is the time") | resp.RecognizedPhrase.Contains("tell me the time"))
                {
                    resp.SynthesisOutput = "It is " + DateTime.Now.ToString("hh:mm tt", System.Globalization.DateTimeFormatInfo.InvariantInfo) + " " + resp.AcknowledgementGender;
                }
                if (resp.RecognizedPhrase.Contains("what is the day"))
                {
                    resp.SynthesisOutput = "It is" + DateTime.Now.DayOfWeek.ToString() + " " + resp.AcknowledgementGender;
                }
                if (resp.RecognizedPhrase.Contains("what is the date"))
                {
                    resp.SynthesisOutput = "It is" + DateTime.Now.ToLongDateString() + " " + resp.AcknowledgementGender;
                }

                if (resp.RecognizedPhrase.Contains("hello"))
                {
                    int greetingIndex = RandomizeResponse(resp.Greetings);
                    Console.WriteLine("Randomized index is : {0}", greetingIndex);
                    resp.SynthesisOutput = resp.Greetings[greetingIndex];
                }

                SendForSynthesis(resp.SynthesisOutput);
            }
        }

        public static void ResponseBox_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to ResponseBox type commands
            */
            if (resp.RecognizedPhrase.ToLower().Contains("reanalyze"))
                resp.SynthesisOutput = "Rephrase the command again";

            SendForSynthesis(resp.SynthesisOutput);
        }

        public static void CloseProgram_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to CloseProgram type commands
            */
            if (resp.CommandType != CommandType.CloseProgram)
                throw new InvalidOperationException("Wrong CommandType of the respective Response argument");
            else
            {
                resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
                resp.SynthesisOutput = "Closing program" + resp.RecognizedPhrase.Replace("close", "");
                SendForSynthesis(resp.SynthesisOutput);
            }
        }

        public static void UI_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to UI type commands
            */
            if (resp.CommandType == CommandType.UI)
            {
                resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
                if (resp.RecognizedPhrase.Contains("refresh"))
                    resp.SynthesisOutput = "Refreshing UI";
                if (resp.RecognizedPhrase.Contains("quit"))
                    resp.SynthesisOutput = "Closing main Program";

                SendForSynthesis(resp.SynthesisOutput);
            }
            else
                throw new InvalidOperationException("Wrong CommandType of the respective Response argument"); 

        }

        public static void Open_SearchType_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to Open_Search type commands
            */
            if (resp.CommandType!=CommandType.Open| resp.CommandType != CommandType.Search)
                throw new InvalidOperationException("Wrong CommandType of the respective Response argument");
            else
            {
                resp.SynthesisOutput = resp.CommandType.ToString()+"ing" + resp.RecognizedPhrase;
                SendForSynthesis(resp.SynthesisOutput);
            }
        }
        #endregion
    }
}