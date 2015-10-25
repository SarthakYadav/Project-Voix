/*
    description: DataStore class
           -class stores Shared UserData and Keeps track of Recent Commands Recognized by the Engine
    date created: -14/10/15

    log:-
    * No Updates Done*

     Listed Public Methods:
           * NONE*
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace Project_Voix
{
    public static class DataStore
    {
        public static AutoResetEvent handle1 = new AutoResetEvent(false);

        #region Fields
        static bool NoStoredUser = false;
        static List<UserSettings> listOfUsers = new List<UserSettings>();                   //store the list of users
        static UserSettings currentUser = null;                                             //stores the current user
        static Queue<string> recentCommands = new Queue<string>();                          //stores a Queue of recent Commands (max 10)
        #endregion

        public static Queue<string> RecentCommands                                          //read only type public Property encapsulating recentCommands
        {
            get { return recentCommands; }
        }

        public static List<UserSettings> Users
        {
            get { return listOfUsers; }
        }

        public static UserSettings CurrentUser
        {
            get { return currentUser; }
        }
        #region Public Methods
        public static void StartDataStoreManager()
        {
            
            LoadUserSettings();
            if (NoStoredUser)
            {
                AddUser.OpenAddUserDialog();
                handle1.WaitOne();
            }
            else
            {
                SelectUser.OpenUserSelectWindow();
                handle1.WaitOne();
            }
            Init.waitHandle2.Set();
        }
        public static void AddNewUser(string username,string userGender,string assistantName,string voiceGender,string voiceAge)
        {
            UserGender _userGender;
            VoiceGender _voiceGender;
            if (userGender == "Male")
                _userGender = UserGender.Male;
            else
                _userGender = UserGender.Female;

            if (voiceGender == "Male")
                _voiceGender = VoiceGender.Male;
            else if (voiceGender == "Female")
                _voiceGender = VoiceGender.Female;
            else
                _voiceGender = VoiceGender.Neutral;

            currentUser = new UserSettings(username,_userGender,assistantName,_voiceGender);

            if (voiceAge == "Adult")
                currentUser.SynthesizerVoiceAge = VoiceAge.Adult;
            else if (voiceAge == "Child")
                currentUser.SynthesizerVoiceAge = VoiceAge.Child;
            else if (voiceAge == "Senior")
                currentUser.SynthesizerVoiceAge = VoiceAge.Senior;
            else
                currentUser.SynthesizerVoiceAge = VoiceAge.Teen;

            listOfUsers.Add(currentUser);
            SaveUserSettings();
            
        }
        public static void AddRecentCommand(string phrase)
        {

            if (recentCommands.Count > 10)
                recentCommands.Dequeue();

            recentCommands.Enqueue(phrase);
        }

        public static string ReturnAssistantName()
        {
            return currentUser.AssistantName;
        }

        public static string GetUserAcknowledgement()
        {
            if (currentUser.Gender == UserGender.Male)
                return "Sir";
            else
                return "Maam";
        }
        public static void SaveUserSettings()
        {
            foreach (var item in listOfUsers)
            {
                item.WriteSettings(location: @"C:\Users\HEWLETT PACKARD\Documents\Project Voix\");
            }
        }

        public static void LoadUserSettings()
        {
            string _name = "";
            try
            {
                var items = Directory.GetFiles(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix", "*.file");
                if (items.Length > 0)
                {
                    foreach (var item in items)
                    {
                        _name=item.Substring(item.LastIndexOf('\\') + 1).Replace(".file", "");
                        listOfUsers.Add(UserSettings.GetSettings(item, _name));
                    }
                }
                else
                    NoStoredUser = true;
            }
            catch(Exception e)
            {
                
                MessageBox.Show(string.Format("{0}\n Please ensure that there is a stored User File",e.Message));
            }
        }
        public static void DisplayCurrentUser()
        {
            Console.WriteLine("Current user is : {0}", currentUser);
        }

        public static void LoadUser(string userName)
        {
            currentUser = UserSettings.GetSettings(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix",userName);
        }

        #endregion
    }
}
