﻿/*
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
        BitmapImage img = null;
        Image userImage;
        string imageUri;
        string moviesFolder;
        
        public AddUser()
        {
            InitializeComponent();
            userImage = new Image();
        }
        
        private void addImgBtnClick(object sender, RoutedEventArgs e)
        {
            /*
                Sets the Background image for the Image Button
            */
            try
            {
                OpenFileDialog openImageFile = new OpenFileDialog();
                openImageFile.Title = "User Image Selection";
                openImageFile.Filter = "Image Files (*.jpg,*.gif,*.png,*.jpe,*.bmp)|*.jpg;*.gif;*.png;*.jpe;*.bmp";
                openImageFile.FilterIndex = 1;
                openImageFile.Multiselect = false;
                if (openImageFile.ShowDialog() == true)
                {
                    imageUri = openImageFile.FileName;
                    img = new BitmapImage(new Uri(openImageFile.FileName));
                    imgBrush.ImageSource = img;

                }
            }
            catch (Exception ex)
            {
                Task.Run(() => { DataStore.AddToErrorLog(string.Format("An Exception Occured---\n Exception message : {0}\n Excetion StackTrace : {1}",ex.Message,ex.StackTrace)); });
            }
            
            
        }

        private void addUserOkClick(object sender, RoutedEventArgs e)
        {
            DataStore.AddNewUser(userNameTextBox.Text, userGenderListbox.Text, assistantNameTextbox.Text, voiceGenderSelection.Text, moviesFolderSelection.Text,imageUri);
            GrammarFeeder.SetAssistantName(assistantNameTextbox.Text);
            this.Close();
        }

        private void addUserCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void OpenAddUserDialog()
        {
            /*
                Starts a new Add user window on a separate UI thread
            */
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

        private void moviesFolderSelection_RightClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if(result== System.Windows.Forms.DialogResult.OK)
            {
                moviesFolder = dialog.SelectedPath;
            }
        }

        private void moviesFolderSelection_TextChanged(object sender, TextChangedEventArgs e)
        {
            var obj = sender as TextBox;
            moviesFolder = @obj.Text;
            DataStore.AddToMessageDump(string.Format("movies folder is : {0}",moviesFolder));
        }

    }
}
