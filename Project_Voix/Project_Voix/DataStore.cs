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
using System.Threading.Tasks;


namespace Project_Voix
{
    static class DataStore
    {


        #region Fields
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

        #region Public Methods
        public static void AddUser()
        {
            //opens a new AddUser Dialog Box here

            //Console.WriteLine("New user dialog open. Add details for entering users");

            //UserGender uGender;
            //Console.WriteLine("Press 1 to enter a new user and set him as current user.");
            //Console.WriteLine("Press 2 to load an existing user.");
            //int choice = int.Parse(Console.ReadLine());
            //Console.WriteLine("Enter user name");
            //string userName = Console.ReadLine();
            //switch (choice)
            //{
            //    case 2:
            //        LoadUser(userName);
            //        break;
            //    case 1:
            //        Console.WriteLine("Enter the user's gender (Male or Female in the exact way) : ");
            //        string gender = Console.ReadLine();

            //        if (gender.ToLower() == "male")
            //            uGender = UserGender.Male;
            //        else
            //            uGender = UserGender.Female;

            //        currentUser = new UserSettings(userName, uGender);
            //        listOfUsers.Add(currentUser);
            //        break;
            //    default:
            //        throw new InvalidOperationException("Unknown choice");
            //}
            currentUser = new UserSettings("Sarthak Yadav",UserGender.Male);
            listOfUsers.Add(currentUser);
            Init.waitHandle2.Set();
        }
        public static void AddRecentCommand(string phrase)
        {

            if (recentCommands.Count > 10)
                recentCommands.Dequeue();

            recentCommands.Enqueue(phrase);
        }

        public static string ReturnAssistantName()
        {
            return "Tars";
            //return currentUser.AssistantName;
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

            var items = Directory.GetFiles(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix", "*.file");
            if(items.Length!=0)
            {
                foreach (var item in items)
                {
                    listOfUsers.Add(UserSettings.GetSettings(item));
                }
            }
            

        }
        public static void DisplayCurrentUser()
        {
            Console.WriteLine("Current user is : {0}", currentUser);
        }

        public static void LoadUser(string userName)
        {
            currentUser = UserSettings.GetSettings(@"C:\Users\HEWLETT PACKARD\Documents\Project Voix", "Sarthak Yadav");
        }
        #endregion
    }
}
