using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Project_Voix
{
    class MainWindowViewModel
    {
        private Dispatcher _currentDispatcher;
        Queue<string> _commands = null;

        public Queue<string> Commands
        {
            get { return _commands; }
            private set { _commands = value; }
        }
        public MainWindowViewModel()
        {
            Commands= DataStore.RecentCommands;
            
        }


    }
}
