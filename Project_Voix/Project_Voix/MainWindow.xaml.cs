﻿/*
    MainWindow class
            - The main UI window

    date created: -15/10/15                                 Author- Sarthak

    log:-
        Update 1: 20/10/15  Author: Sarthak         Description: Closing in towards completion of UI, added new custom controls
        Update 2: 25/10/15  Author: Sarthak         Description: Completed the most bare basic UI Infrastructure, which AddUser and SelectUser functionality

        latest update: 25/10/2015     Update 2           Author: Sarthak

     UI Elements and their Functionality:(sans the layout details)
           -commandLog textbox: shows the command log (basic here)
           -userImage: the current User's image is displayed
           -UserDetails TextBLock group: group of textboxes that display the details of the current user

    Public Methods:
           -OpenUserSelectWindow(): opens a New SelectUser Type window ON A SEPARATE THREAD OF EXECUTION, so that the main UI thread is not blocked
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Voix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event SetSynthesisVolume VolumeChanged;
        public static event SetSynthesisRate RateChanged;
        static public Stopwatch initStopwatch;
        public CancellationTokenSource cancelToken = new CancellationTokenSource();
        static public CancellationToken ct;
        
        public MainWindow()
        {
            initStopwatch = new Stopwatch();
            Stopwatch stop = new Stopwatch();
            stop.Start();
            InitializeComponent();
            ct = cancelToken.Token;                                                     //token that signals to holds up the Init.StartInit thread 
            Task initRun = new Task(new Action(Init.ProgramInit),cancelToken.Token);
            this.Closed += (sender, e) =>
            {
                Console.WriteLine(ct.CanBeCanceled.ToString());
                cancelToken.Cancel();                                                   //when this ui is closed, hold up the Init.StartInit thread
            };
            initRun.Start();           //starts the Init.StartInit method, which is the main recognizer thread, asynchronously
            stop.Stop();
            DataStore.SetUserNow += DataStore_SetUserNow;                           //event handler for DataStore's SetUserNow event, which notifies when User is set
            GrammarFeeder.writeToTextBox += GrammarFeeder_writeToTextBox;          //event handler for GrammarFeeder's writeToTextBox event
        }

        private void DataStore_SetUserNow(UserSettings user)
        {
            /*
                Invokes Dispatcher object of each UIELEMENT and sets the respective value

            */

            currentUserName.Dispatcher.InvokeAsync(() => { currentUserName.Text = user.Username; });
            currUserAssitantName.Dispatcher.InvokeAsync(() => { currUserAssitantName.Text = user.AssistantName; });
            currUserGender.Dispatcher.InvokeAsync(()=>{ currUserGender.Text = user.Gender.ToString(); });
            currUserVoiceGender.Dispatcher.InvokeAsync(() => { currUserVoiceGender.Text = user.SynthesizerVoiceGender.ToString(); });
            userImage.Dispatcher.InvokeAsync(() => 
            {
                var bitmap = new BitmapImage(new Uri(user.ImageSource));                    //new Bitmap Image using the ImageSource of the current user
                userImage.Source = bitmap;                                                  //sets the source of the userImage control with the bitmap image
            });
            
        }

        private void GrammarFeeder_writeToTextBox(string logUpdate)                                     
        {
            commandLog.Dispatcher.InvokeAsync(()=> { commandLog.Text += string.Format("\n{0}", logUpdate); });          //used to access A UI element from another thread
        }
       
        

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            /*
                UI blocking AddUser Window
            */
            AddUser userTab = new AddUser();
            userTab.Show();
        }

        private void selectUserClick(object sender, RoutedEventArgs e)
        {
            /*
                Non thread blocking Function to SelectUser window. would be made Synchronous
            */
            SelectUser.OpenUserSelectWindow();

        }

        private void volSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /*
                fired when the volSlider value changes
            */
            var slider = sender as Slider;
            try
            {
                VolumeChanged((int)slider.Value);                   //throws the VolumeChanged Event
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format(" message : {0}\n Source : {1}\n Stack : {2} ", ex.Message, ex.Source, ex.StackTrace));
            }
            Console.WriteLine((int)slider.Value);
            
        }

        private void rateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /*
                fired when the rateSlider value changes
            */
            var slider = sender as Slider;
            try
            {
                RateChanged((int)slider.Value);                     //throws the RateChanged event
            }
            catch (Exception ex)
            {
                Console.WriteLine(" message : {0}\n Source : {1}\n Stack : {2} ", ex.Message, ex.Source, ex.StackTrace);
            }
        }
    }
}
