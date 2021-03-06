﻿/*
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
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Project_Voix
{
    static class ResponseGenerator
    {
        
        #region Private Methods
        static int RandomizeResponse(List<string> list)
        {
            /*
                For randomizing the response provided for the Greetings style and Open/Search type commands
            */
            
            int i = 0;
            if (list != null)
            {
                Random rand = new Random();
                i = rand.Next(0, list.Count);
            }

            else
                throw new ArgumentException("Null reference. Check your arguments");
            return i;
        }

        static public async void SendForSynthesis(string resp)
        {
            /*
                Imitation of a method that would send Synthesis commands over to the Synthesizer thread
            */

            await Task.Factory.StartNew(new Action<object>(Speaker.Synthesizer), resp);
        }

        #endregion

        #region GenerateResponse Event Handlers
        public static void PrimaryGrammar_ResponseHandler(Response resp)
        {
            
            resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
            string responseText = "";

            if(resp.CommandType==CommandType.Primary)
            {
                if (resp.RecognizedPhrase.Contains("wake up") | resp.RecognizedPhrase.Contains("resume recognition"))
                    responseText = " Awake . Recognition Resumed . ";
                else if (resp.RecognizedPhrase.Contains("sleep") | resp.RecognizedPhrase.Contains("pause"))
                    responseText = " Asleep . Recognition Halted . ";
                else if (resp.RecognizedPhrase.Contains("already paused"))
                    responseText="Recognition is already paused . Dont sweat it ";
                else
                    responseText="Recognition is already in progress . ";

                SendForSynthesis(responseText);
            }
        }

        public static void BasicGrammar_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to Basic Grammar type commands
            
            */
            string responseText = "";
            string demoString = "Hello Sir / Ma'am and welcome to Voix . I am Tars, your personal Assistant . "+" lets go ahead and take a quick peek into the Program"+
                " In the UI home screen, you can see the official logo of the program . Nice Right ? "+
                "On the top right you can see a box which says LOG , this would be our Recent command Logger . "+
                "It isnt something fancy as of now , but don't worry , we'll be making the UI better very shortly . "+
                "Below the Log box , you can see two sliders , namely Synthesizer volume and Synthesizer Rate . As the name suggests ,  "+
                " the Synthesizer Volume slider adjusts the volume of the Text To Speech Volume , whereas the Synthesizer Rate slider adjusts the rate "+
                " at which the Text To Speech Engine Speaks . "+
                "Now to the left, you can see a small arrow pointing towards the left , it is an expander , click it . "+
                "Now that reveals a nice container , which displays the details of the current User . "+
                "And right below that , you can see the Select User and Add User buttons, which open new windows that provide the "+
                " respective functionality , without blocking the Main UI thread . "
                +" now , fun facts , the Program extensively uses Multithreading , in order to provide fast and smooth User Experience , with "
                +" 4 main threads that are always running , making full use of multicore processors , "
                +" as well as additional threads wherever intensive tasks are to be performed , keeping context switching to the bare minimum "+
                " in order to maintain maximum performance . "+
                " I hope you enjoyed the demo . Tars signing off . ";
            resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
            if (resp.CommandType == CommandType.Basic)
            {
                
                if (resp.RecognizedPhrase.Contains("system controls"))
                    responseText = "Starting System Controls";
                else if (resp.RecognizedPhrase.Contains("system power options"))
                    responseText = "Starting System Power options";
                else if (resp.RecognizedPhrase.Contains("give a demo"))
                {
                    responseText = demoString;
                }
                else
                    throw new InvalidOperationException("Unknown Operation.");

                SendForSynthesis(responseText);
            }
            else if (resp.CommandType == CommandType.Open)     //"Open", "Execute", "Run", "Intialize", "Start"
            {
                int responseIndex = RandomizeResponse(resp.OpenSearchResponses);
                responseText = resp.OpenSearchResponses[responseIndex];
                SendForSynthesis(responseText);
            }
            else if(resp.CommandType == CommandType.Search)
            {
                responseText = "Opening my friend Google Chrome for searching the web";
            }
            else
                throw new InvalidOperationException("Wrong CommandType of the respective Response object");

        }

        static public void NonOperational_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to NonOperational type commands
                
            */
            string response= "";
            if (resp.CommandType != CommandType.NonOperational)
                throw new InvalidOperationException("Wrong CommandType for the input response class");
            else
            {
                resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
                if (resp.RecognizedPhrase.Contains("what is the time") | resp.RecognizedPhrase.Contains("tell me the time"))
                {
                    response = "It is " + DateTime.Now.ToString("hh:mm tt", System.Globalization.DateTimeFormatInfo.InvariantInfo) + " " + resp.AcknowledgementGender;
                }
                if (resp.RecognizedPhrase.Contains("what is the day"))
                {
                    response = "It is " + DateTime.Now.DayOfWeek.ToString() + " " + resp.AcknowledgementGender;
                }
                if (resp.RecognizedPhrase.Contains("what is the date"))
                {
                    response = "It is " + DateTime.Now.ToLongDateString() + " " + resp.AcknowledgementGender;
                }

                if (resp.RecognizedPhrase.Contains("hello"))
                {
                    int greetingIndex = RandomizeResponse(resp.Greetings);
                    response = resp.Greetings[greetingIndex];
                }
                SendForSynthesis(response);
            }
        }

        public static void ResponseBox_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to ResponseBox type commands
            */
            string response = "";
            resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
            if (resp.RecognizedPhrase == "user dialog ok . ")
                response = "User Response Box Ok ";
            else if (resp.RecognizedPhrase=="ok")
                 response = "Running the given program";
            else if (resp.RecognizedPhrase.Contains("cancel"))
                response = "Response box is now closing";
            else
                response = "Rephrase the command again";


            SendForSynthesis(response);
        }

        public static void CloseProgram_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to CloseProgram type commands
            */
            string response = "";
            if (resp.CommandType != CommandType.CloseProgram)
                throw new InvalidOperationException("Wrong CommandType of the respective Response argument");
            else
            {
                resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
                if (resp.RecognizedPhrase.Contains("tars close"))
                    resp.RecognizedPhrase=resp.RecognizedPhrase.Replace("tars close", "");
                if (resp.RecognizedPhrase.Contains("close"))
                    resp.RecognizedPhrase = resp.RecognizedPhrase.Replace("close", "");

                response = "Closing program" + resp.RecognizedPhrase;
                SendForSynthesis(response);
            }
        }

        public static void UI_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to UI type commands
            */
            string response = "";
            if (resp.CommandType == CommandType.UI)
            {
                resp.RecognizedPhrase = resp.RecognizedPhrase.ToLower();
                if (resp.RecognizedPhrase.Contains("add user"))
                    response = "Opening Add User Dialog";
                else if (resp.RecognizedPhrase.Contains("select user"))
                    response = "Opening Select User Dialog";
                else if (resp.RecognizedPhrase.Contains("expand expander"))
                    response = "expander expanding";
                else if (resp.RecognizedPhrase.Contains("increase synthesizer volume"))
                    response = "increasing synthesis volume";
                else if (resp.RecognizedPhrase.Contains("increase synthesizer rate"))
                    response = "increasing synthesis rate";
                else if (resp.RecognizedPhrase.Contains("decrease synthesizer volume"))
                    response = "decreasing synthesis volume";
                else if (resp.RecognizedPhrase.Contains("decrease synthesizer rate"))
                    response = "decreasing synthesis rate";
                else
                    response = "Are you sure you wish to Quit the program ? ";
                SendForSynthesis(response);
            }
            else
                throw new InvalidOperationException("Wrong CommandType of the respective Response argument");

        }

        public static void Open_SearchType_ResponseHandler(Response resp)
        {
            /*
                Event handler for Responses corresponding to Open_Search type commands
            */
            // if (resp.CommandType!=CommandType.Open|| resp.CommandType != CommandType.Search)
            //   throw new InvalidOperationException("Wrong CommandType of the respective Response argument");
            //else
            string response = "";

            {
                if (resp.RecognizedPhrase.Contains("Tars"))
                    resp.RecognizedPhrase.Remove(0, 5);
                else
                {
                    response = "Speak Ok to " +resp.CommandType.ToString() + resp.RecognizedPhrase;
                    SendForSynthesis(response);
                }
            }
        }
        #endregion
    }
}
