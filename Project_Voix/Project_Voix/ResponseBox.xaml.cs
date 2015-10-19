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
using System.Windows.Threading;

namespace Project_Voix
{
    /// <summary>
    /// Interaction logic for ResponseBox.xaml
    /// </summary>
    public partial class ResponseBox : Window
    {
        Dispatcher currentDispatcher;
        private ResponseBox()
        {
            InitializeComponent();
            GrammarFeeder.RespBoxRecogDisplay += GrammarFeeder_RespBoxRecogDisplay;
            GrammarFeeder.CloseResponseBoxEvent += GrammarFeeder_CloseResponseBoxEvent;
        }

        private void GrammarFeeder_CloseResponseBoxEvent()
        {
            //currentDispatcher = Dispatcher.CurrentDispatcher;

            //this.btnOkClick(null, null);
            this.Dispatcher.InvokeAsync(() => { this.Close(); });
        }

        private void GrammarFeeder_RespBoxRecogDisplay(string recogPhrase)
        {
            if(recogPhrase!=null)
            {
                try
                {
                    recogWordDisplay.Dispatcher.InvokeAsync(() => { recogWordDisplay.Text = recogPhrase; });
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Exception message : {0}\n Exception stack : {1}",e.Message,e.StackTrace));
                }
            }
           
        }
        
        static public void CreateResponseBox()
        {
            Thread thread = new Thread(() =>
            {
                ResponseBox respBox = new ResponseBox();
                respBox.Loaded += (sender, e) =>
                {
                    GrammarManipulator.ResponseBoxLoaded();
                };
                respBox.Closed += (sender, e) =>
                {
                    GrammarManipulator.ResponseBoxDeloaded();
                    respBox.Dispatcher.InvokeShutdown();                        //so by closing the window the thread is shutdown as well. otherwise we get ghost thread which doesnt allow th application to exit
                };
                respBox.Show();
                System.Windows.Threading.Dispatcher.Run();                      //for calling the dispatcher of the current window/uielement  needed for running
            });

            thread.SetApartmentState(ApartmentState.STA);                       //STA state in needed by wpf
            thread.Start();                                                     //starts the thread
        }

        private void btnOkClick(object sender, RoutedEventArgs e)
        {
            //start the recognized process here
            this.Close();
        }

        private void btnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
