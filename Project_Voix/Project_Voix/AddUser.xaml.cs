/*
    AddUser class
            - Creates a new Window that provides the functionality to Add a new User

    date created: -20/10/15                                 Author- Sarthak

    log:-
    * No Updates Done*

     UI Elements and their Functionality:
           -addImgBtn: Custom Add image button which sets an image to the user 
           -addUserOk: The OK button
           -addUserCancel: the cancel Button

    Public Methods:
           -OpenAddUserDialog(): opens a New AddUser Type window ON A SEPARATE THREAD OF EXECUTION, so that the main UI thread is not blocked
*/

using Microsoft.Win32;
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
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        //UserSettings currentUser;
        public AddUser()
        {
            InitializeComponent();
            
        }
        
        private void addImgBtnClick(object sender, RoutedEventArgs e)
        {
            /*
                Sets the Background image for the Image Button
            */
            BitmapImage userImage = null;
            OpenFileDialog openImageFile = new OpenFileDialog();
            openImageFile.Title = "User Image Selection";
            openImageFile.Filter = "Image Files (*.jpg,*.gif,*.png,*.jpe,*.bmp)|*.jpg;*.gif;*.png;*.jpe;*.bmp";
            openImageFile.FilterIndex = 1;
            openImageFile.Multiselect = false;
            if (openImageFile.ShowDialog() == true)
            {
                userImage = new BitmapImage(new Uri(openImageFile.FileName));
                imgBrush.ImageSource = userImage;
            }
        }

        private void addUserOkClick(object sender, RoutedEventArgs e)
        {
            DataStore.AddNewUser(userNameTextBox.Text, userGenderListbox.Text, assistantNameTextbox.Text, voiceGenderSelection.Text, voiceAgeSelection.Text);
            //if (imgBrush.ImageSource != null)
              //  DataStore.CurrentUser.UserImage =(BitmapImage) imgBrush.ImageSource;
            this.Close();
        }

        private void addUserCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void OpenAddUserDialog()
        {
            Thread thread = new Thread(() =>
              {
                  AddUser adduser = new AddUser();
                  adduser.Closed += (sender, e) => 
                  {
                      adduser.Dispatcher.InvokeShutdown();
                      DataStore.handle1.Set();
                  };
                  adduser.Show();
                  System.Windows.Threading.Dispatcher.Run();
              });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }
    }
}
