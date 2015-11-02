/*
    MainWindow class
            - The main UI window

    date created: -15/10/15                                 Author- Sarthak

    log:-
        Update 1: 20/10/15  Author: Sarthak         Description: Closing in towards completion of UI, added new custom controls
        Update 2: 25/10/15  Author: Sarthak         Description: Completed the most bare basic UI Infrastructure, which AddUser and SelectUser functionality
        update 3: 30/10/15  Author: Sarthak         Description: Added UIgrammar being auto loaded and auto deloaded when the application is selected or not
        
        latest update: 25/10/2015     Update 3           Author: Sarthak

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
            #region Non Essential Stuff
            initStopwatch = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            #endregion

            InitializeComponent();

            //expanderCntrl.IsExpanded = true;
            #region Event Handlers of the Mainwindow itself
            this.Closed += (sender, e) =>                                   //fires up when the UI Closes
            {
                Console.WriteLine(ct.CanBeCanceled.ToString());
                cancelToken.Cancel();                                                   //when this ui is closed, hold up the Init.StartInit thread
            };

            this.Closing += (sender, e) =>                                              //occurs as soon as this.Close() occurs
              {
                  MessageBoxResult result = MessageBox.Show("Do you really want to quit?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                  if (result == MessageBoxResult.No)
                  {
                      e.Cancel = true;
                  }
              };
            this.Activated += (sender, e) =>
            {
                /*
                    Fires up when the UI is "InFocus" i.e. is the active window 
                */
                try
                {
                    GrammarManipulator.UILoaded();                                      //Loads the UIGrammar 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            this.Deactivated += (sender, e) =>
            {
                /*
                    Fires up when the UI has "LostFocus" i.e. is no longer the active window
                */
                try
                {
                    GrammarManipulator.UIDeloaded();                                    //unloads the UI grammar
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };
            #endregion

            #region Additional Stuff
            Task initRun = new Task(new Action(Init.ProgramInit), cancelToken.Token);
            initRun.Start();           //starts the Init.StartInit method, which is the main recognizer thread, asynchronously
            sw.Stop();
            #endregion

            #region Event Handlers for Events of External CLasses
            DataStore.SetUserNow += DataStore_SetUserNow;                           //event handler for DataStore's SetUserNow event, which notifies when User is set
            GrammarFeeder.writeToTextBox += GrammarFeeder_writeToTextBox;          //event handler for GrammarFeeder's writeToTextBox event
            GrammarFeeder.UpdateSlider += GrammarFeeder_UpdateSlider;               //event handler for GrammarFeeder's UpdateSlider event
            GrammarFeeder.CloseMainWindow += GrammarFeeder_CloseMainWindow;         //Close Window Event
            GrammarFeeder.ExpandIt += GrammarFeeder_ExpandIt;                           //ExpanderExpanding event
            #endregion

            #region Dumping some Data
            Task.Run(() => { DataStore.AddToMessageDump(string.Format("Time elapsed during loading of main window : {0}", sw.ElapsedMilliseconds)); });
            logoRectangle.ToolTip = "This is the official logo of Project Voix ." +
                "\n As opposed to the program itself, the logo is copyright protected." +
                " Any unauthorised use is subjected to copyright infringement actions ." + "\n Designed by Grafoholics (tm).";
            #endregion
        }

        #region Private Methods and Event Handlers

        private void GrammarFeeder_ExpandIt()
        {
            expanderCntrl.Dispatcher.InvokeAsync(() => { expanderCntrl.IsExpanded = true; });
        }

        private void GrammarFeeder_CloseMainWindow()
        {
            /*
                occurs when Exit Command is recognized
            */
            this.Dispatcher.Invoke(() => { this.Close(); });
        }

        private void GrammarFeeder_UpdateSlider(int i, string sliderType)
        {
            /*
                Handles the Update slider event.
                Any changes made to the slider values are AUTOMATICALLY PICKED UP
                by their own ValueChanged Events
            */
            if (sliderType == "volume")                                                             //checks if the sliderType is volume and performs apt actions
                volSlider.Dispatcher.InvokeAsync(() => { volSlider.Value += i; });                  
            else
                rateSlider.Dispatcher.InvokeAsync(() => { rateSlider.Value += i; });                //since there are only two types of slider, performs actions on the rate slider
        }

        private void DataStore_SetUserNow(UserSettings user)
        {
            /*
                Invokes Dispatcher object of each UIELEMENT and sets the respective value

            */
            try
            {
                currentUserName.Dispatcher.InvokeAsync(() => { currentUserName.Text = user.Username; });
                currUserAssitantName.Dispatcher.InvokeAsync(() => { currUserAssitantName.Text = user.AssistantName; });
                currUserGender.Dispatcher.InvokeAsync(() => { currUserGender.Text = user.Gender.ToString(); });
                currUserVoiceGender.Dispatcher.InvokeAsync(() => { currUserVoiceGender.Text = user.SynthesizerVoiceGender.ToString(); });
                if (user.ImageSource != "" && user.ImageSource != null)
                    userImage.Dispatcher.InvokeAsync(() =>
                    {
                        var bitmap = new BitmapImage(new Uri(user.ImageSource));                    //new Bitmap Image using the ImageSource of the current user
                        userImage.Source = bitmap;                                                  //sets the source of the userImage control with the bitmap image
                    });
            }
            catch (Exception exception)
            {
                DataStore.AddToErrorLog(string.Format("Main exception {0}\nMain exception stack trace {1}\nInner exception {2}\ninner Exception stack trace {3}", exception.Message, exception.StackTrace, exception.InnerException.Message, exception.InnerException.StackTrace));
            }
        }

        private void GrammarFeeder_writeToTextBox(string logUpdate)                                     
        {
            try
            {
                commandLog.Dispatcher.InvokeAsync(() => { commandLog.Text += string.Format("\n{0}", logUpdate); });          //used to access A UI element from another thread
            }
            catch (Exception exception)
            {
                DataStore.AddToErrorLog(string.Format("Main exception {0}\nMain exception stack trace {1}\nInner exception {2}\ninner Exception stack trace {3}", exception.Message, exception.StackTrace, exception.InnerException.Message, exception.InnerException.StackTrace));
            }
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
                DataStore.AddToErrorLog(string.Format(" message : {0}\n Source : {1}\n Stack : {2} ", ex.Message, ex.Source, ex.StackTrace));
            }
            //Console.WriteLine((int)slider.Value);
            
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
                DataStore.AddToErrorLog(string.Format(" message : {0}\n Source : {1}\n Stack : {2} ", ex.Message, ex.Source, ex.StackTrace));
            }
        }

        private void btnPauseRecog_Click(object sender, RoutedEventArgs e)
        {
            Init.PauseRecog();
        }

        private void btnResumeRecog_Click(object sender, RoutedEventArgs e)
        {
            Init.ResumeRecog();
        }
        #endregion
    }
}
