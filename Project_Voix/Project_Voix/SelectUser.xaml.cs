/*
    SelectUser class
            - Creates a new Window that provides the functionality to set the CUrrent User from a set of Pre-Existing Users

    date created: -20/10/15                                 Author- Sarthak

    log:-
    * No Updates Done*

     UI Elements and their Functionality:
           - usersListBox: The listBox which binds to the List of users 
           -btnOk: The OK button
           -btnCancel: the cancel Button

    Public Methods:
           -OpenUserSelectWindow(): opens a New SelectUser Type window ON A SEPARATE THREAD OF EXECUTION, so that the main UI thread is not blocked
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_Voix
{
    /// <summary>
    /// Interaction logic for SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window
    {
        List<UserSettings> listOfUsers;                                             //to get the list of users from the Static class DataStore's Users property

        public SelectUser()
        {
            InitializeComponent();
            this.DataContext = this;
            listOfUsers = DataStore.Users;
            foreach (var item in listOfUsers)
            {
                usersListBox.Items.Add(item);
            }
        }
        
        public static void OpenUserSelectWindow()
        {
            /*
                Starts a new SelectUser window on separate thread of execution 
                hence not blocking the Main UI
            */
            Thread thread = new Thread(() =>
              {
                  SelectUser su = new SelectUser();
                  su.Closed += (sender, e) => 
                  {
                      su.Dispatcher.InvokeShutdown();
                      DataStore.handle1.Set();                                      //sets the handle in DataStore
                  };
                  su.Show();
                  System.Windows.Threading.Dispatcher.Run();                            //MAkes the system Dispatcher to Run the thread
              });
            thread.SetApartmentState(ApartmentState.STA);                               //STA apartment state is necessary for Wpf components to run on
            thread.Start();
        }

        private void btnOkClick(object sender, RoutedEventArgs e)
        {
            //handler for the Ok button
            UserSettings user = usersListBox.SelectedItem as UserSettings;              //the selected item from the usersListBox control is casted as a UserSettings object
            Console.WriteLine("Detected user is {0}", user);
            DataStore.LoadUser(user.Username);                                          //which is then Loaded by the DataStore as the Current User
            DataStore.DisplayCurrentUser();
            GrammarFeeder.SetAssistantName(user.AssistantName);
            
            this.Close();
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            //handler for the Cancel button
            this.Close();
        }
    }
}
