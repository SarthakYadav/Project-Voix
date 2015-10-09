/*
    description: Executable class
           -class that contains the data of a single Executable type file found in the StartMenu which can be executed using the
            Open_Type CommandList, and the corresponding commands which are recognizable by the S.R.E. 
    
    date created: -03/10/15

    log:-
    *No Updates Done*

    Listed Public Methods:
           1. override string ToString(): ovverides the System.Object.ToString()
           2. CompareTo(Executable other): Implementation of the IComparable<Executable> interface
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Voix
{
    class Executable:IComparable<Executable>
    {

        string executableLoc;                       //effective address of the Executable
        string executableCommand;                   //resp command name for the executable which is recognized by the SRE
        ProcessStartInfo processInfo;                            //associated process with the given commands
        Process process;
        #region Properties

        public Process ExecutableProcess
        {
            get { return process; }
            set { process = value; }
        }

        public string TargetAddress
        {
            get { return executableLoc; }
            set { executableLoc = value;}
        }

        public ProcessStartInfo PInfo
        {
            get { return processInfo; }
            set
            {
                if(value!=null)
                    processInfo = value;
            }
        }

        public string CommandName
        {
            get { return executableCommand; }
            set
            {
                if (!(value.Contains(".lnk") || value.Contains(".exe") || value.Contains('\\')))
                    executableCommand = value;
                else
                {
                    throw new Exception("Invalid Command format");
                }
            }
        }

        #endregion

        #region Constructor
        public Executable(string loc, string command)
        {
            TargetAddress = @loc;
            CommandName = command;
            process = null;
            processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.WindowStyle = ProcessWindowStyle.Normal;
        }
        public Executable() { }
        #endregion

        #region Methods
        public override string ToString()
        {
            return String.Format("Name of command : {0}\t\tLocation of the command : {1}", CommandName, TargetAddress);
        }

        public int CompareTo(Executable other)              //Implementation of the IComparable<Executable> interface
        {
            if (other != null)
            {
                if (this.CommandName.CompareTo(other.CommandName) > 0)
                    return 1;
                if (this.CommandName.CompareTo(other.CommandName) < 0)
                    return -1;
                else
                    return 0;
            }
            else
                throw new NotImplementedException();
        }
        #endregion

    }
}
