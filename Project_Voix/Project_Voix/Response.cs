/*
    description: Response class
           -class that represents the various kinds of possible response 
    
    date created: -11/10/15

    log:-
    *No Updates Done*

    Listed Public Methods:
           *NONE*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Voix
{
    class Response
    {

        #region Fields

        static string acknowledgementGender;                       //to store the underlying acknowledgement based on the gender of the user
        int hourOfDay = 0;
        List<string> open_searchResponses;                          //to contain list of open_search type responses
        List<string> greetings;                                     //to contain list of open_search type responses

        #endregion

        #region Properties
        public int HourOfDay
        {
            get { return hourOfDay; }
            set
            {
                if (value <= 24 & value >= 0)
                    hourOfDay = value;
            }
        }
        public CommandType CommandType { get; set; }
        public string RecognizedPhrase { get; set; }
        string TimeOfDay { get; set; }
        public string SynthesisOutput { get; set; }
        public string AcknowledgementGender
        {
            get { return acknowledgementGender; }
            set { acknowledgementGender = value; }
        }
        public List<string> Greetings
        {
            get { return greetings; }
        }
        public List<string> OpenSearchResponses
        {
            get { return open_searchResponses; }
        }
        #endregion

        #region Private Methods
        private void SetTimeOfDay()
        {
            if (HourOfDay < 6 & HourOfDay > 0)
                TimeOfDay = "late night";
            if (HourOfDay >= 6 & HourOfDay <= 11)
                TimeOfDay = "morning";
            if (HourOfDay >= 12 & HourOfDay < 18)
                TimeOfDay = "afternoon";
            if (HourOfDay >= 18 & HourOfDay <= 21)
                TimeOfDay = "evening";
            else
                TimeOfDay = "night";
        }

        private void GenerateGreetingsList()
        {
            greetings = new List<string>()
                {
                "Good " + this.TimeOfDay + " " + acknowledgementGender,
                "A very good " + this.TimeOfDay + " " + acknowledgementGender,
                "Hello " + acknowledgementGender + " How are you this beautiful " + this.TimeOfDay,
                "Good " + this.TimeOfDay + " " + acknowledgementGender + " How is your day going",
                "Greetings from Tars" + acknowledgementGender + " And a very good" + this.TimeOfDay
                };
        }

        private void GenerateOpen_SearchResponseList()
        {
            open_searchResponses = new List<string>()
            {
                this.CommandType.ToString() +"What "+acknowledgementGender,
                "What do you wish to " +this.CommandType.ToString()+" "+acknowledgementGender,
                "What can I "+this.CommandType.ToString()+" for you "+acknowledgementGender+" ?",
                "Please Speak what you wish to "+this.CommandType.ToString()+" "+acknowledgementGender
            };
        }
        #endregion

        #region Constructors
        static Response()
        {
            acknowledgementGender = DataStore.GetUserAcknowledgement();    //or getting settings from the GeneralSettings
        }

        public Response() { }

        public Response(CommandType commandTypeSpecifier, int hourOfDay, string recognisedPhrase)
        {
            CommandType = commandTypeSpecifier;
            HourOfDay = hourOfDay;
            RecognizedPhrase = recognisedPhrase;
            SetTimeOfDay();
            if (CommandType == CommandType.Open | CommandType == CommandType.Search)
                GenerateOpen_SearchResponseList();

            if (CommandType == CommandType.NonOperational)
            {
                GenerateGreetingsList();
            }

        }
        #endregion
    }
}
