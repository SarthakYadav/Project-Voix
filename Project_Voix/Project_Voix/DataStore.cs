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
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Project_Voix
{
    
    
    public static class DataStore
    {
        public static AutoResetEvent handle1 = new AutoResetEvent(false);

        #region Fields
        static bool NoStoredUser = false;
        static bool IsUserSet = false;
        static List<UserSettings> listOfUsers = new List<UserSettings>();                   //store the list of users
                                                                                            //static StringBuilder dump;
        static StreamWriter writer = new StreamWriter(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix\Dump\Dump.txt", true);
        static StreamWriter writer1 = new StreamWriter(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix\Dump\ErrorLog.txt", true);

        static UserSettings currentUser = null;                                             //stores the current user
        static Queue<string> recentCommands = new Queue<string>();                          //stores a Queue of recent Commands (max 10)
        public static event SetUser SetUserNow;
        public static event SynthesisGenderChange VoiceGenderChanged;
        #endregion

        static DataStore()
        {
            //dump = new StringBuilder(string.Format("Message Dump \t Time stamp : {0}",DateTime.Now.ToString()));
            //errLog = new StringBuilder(string.Format("Error Log \t Time stamp : {0}", DateTime.Now.ToString()));
        }
        
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

        #region Private Methods
        
        #endregion
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
            SetUserNow(CurrentUser);
            IsUserSet = true;
            //System.IO.File.WriteAllText(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix\Dump\ErrorLog.txt", Errorlog.ToString());
            //System.IO.File.WriteAllText(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix\Dump\Dump.txt", Dump.ToString());
            Init.waitHandle2.Set();
        }
        
        public static void AddNewUser(string username,string userGender,string assistantName,string voiceGender,string moviesFolder,string imageUri)
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

            if (moviesFolder != "")
                currentUser.Movies = moviesFolder;

            if (imageUri != "")
                currentUser.ImageSource = imageUri;

            listOfUsers.Add(currentUser);
           
            SetUserNow(currentUser);
            try
            {
                VoiceGenderChanged(currentUser.SynthesizerVoiceGender);
            }
            catch(Exception ex)
            {
                AddToErrorLog(string.Format("message : {0}\n Source : {1}\n StackTrace : {2}",ex.Message,ex.Source,ex.StackTrace));
            }
            //currentUser = user;
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
                
                AddToErrorLog(string.Format("{0}\n Please ensure that there is a stored User File",e.Message));
            }
        }
        public static void DisplayCurrentUser()
        {
            AddToMessageDump(string.Format("Current user is : {0}", currentUser));
        }

        public static void LoadUser(string userName)
        {
            currentUser = UserSettings.GetSettings(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix",userName);
            try
            {
                SetUserNow(currentUser);
                VoiceGenderChanged(currentUser.SynthesizerVoiceGender);
            }
            catch (Exception ex)
            {
                AddToErrorLog(string.Format(" message : {0}\n Source : {1}\n Stack : {2} ", ex.Message, ex.Source, ex.StackTrace));
            }
            
        }

        static public void AddToMessageDump(string text)
        {
            try
            {
                Task.Run(() =>
                {
                    writer.Write(string.Format("\n\n" + text + "\n Time Stamp : {0}", DateTime.Now.ToString()));
                    writer.Flush();
                    //writer.Close();
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
        }
        static public void AddToErrorLog(string text)
        {
            try
            {
                Task.Run(() =>
                {
                    writer1.Write(string.Format("\n" + text + "\n Time Stamp : {0}", DateTime.Now.ToString()));
                    writer1.Flush();
                    //writer1.Close();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }    
        }
        #endregion
    }
}
