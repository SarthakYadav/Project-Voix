﻿/*
    description: ProgramManager class
           -static class
           -stores exhaustive list of Executable objects,which are executed from the OPEN_TYPE commands
           -Manipulation and Necessary Management operations 
    
    date created: -03/10/15

    log:-
    *No Updates Done*

    Listed Public Methods:
           
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_Voix
{
    class ProgramManager
    {
        #region Fields
        static List<Executable> executablesList = new List<Executable>();
        static List<Executable> runningExecutables = new List<Executable>();
        #endregion

        #region Private Methods

        static int GetIndexOfCommand(string command,ref List<Executable> list)
        {
            int i = 0;
            if(list!=null & command!="")
            {
                i = list.BinarySearch(new Executable("",command));
                return i;
            }
            else
               throw new InvalidDataException("The argument passed is invalid");
        }
        static void InitProcess(Executable ex)
        {
            string prevfileName = ex.PInfo.FileName;
            try
            {
                ex.ExecutableProcess = Process.Start(ex.PInfo);
            }
            catch (Exception e)
            {
                if (e.Message == "The system cannot find the path specified")
                {
                    
                    try
                    {
                        ex.PInfo.FileName = ex.PInfo.FileName.Substring(ex.PInfo.FileName.LastIndexOf('\\') + 1);
                        ex.ExecutableProcess = Process.Start(ex.PInfo as ProcessStartInfo);
                    }
                    catch (Exception exp)
                    {
                        
                        if (ex.PInfo.FileName.Contains("System32"))
                        {
                            ex.PInfo.FileName = prevfileName;
                            ex.PInfo.FileName.Replace("System32", "Sysnative");
                            ex.ExecutableProcess = Process.Start(ex.PInfo);
                        }
                        else
                            System.Windows.MessageBox.Show(exp.Message);
                    }
                }
            }
            if (ex.ExecutableProcess != null && ex != null)
                runningExecutables.Add(ex);
        }

        static void StartProcess(int i)
        {
            Executable ex = executablesList[i] as Executable;
            string targetAddress = executablesList[i].TargetAddress;
            string commandName = executablesList[i].CommandName;
            ex.PInfo.FileName=targetAddress;
            InitProcess(ex);
        }

        private static void CloseProcess(int i)
        {
            if (i < 0)
                throw new Exception("Invalid index of requested close command. Recheck the command and make sure it corresponds to a running Executable");
                Executable ex = runningExecutables[i] as Executable;
                string name = ex.PInfo.FileName;
                name = name.Substring(name.LastIndexOf('\\') + 1);                      //derives substring from last occurence of '/' to the end
                name = name.Remove(name.LastIndexOf('.'));                              //deletes from last occurence of '.'
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);              //to convert the first letter of name to lowercase
                Console.WriteLine(name);
                if (runningExecutables[i].ExecutableProcess.ProcessName.CompareTo(name)==0)
                {
                    runningExecutables[i].ExecutableProcess.Kill();
                    runningExecutables.RemoveAt(i);
                }
        }
        #endregion

        #region Public Methods
       
        static async public void InitializeManager()
        {
            await Task.Run(() =>
            {
                //object obj = new object();
                //Console.WriteLine("InitializeManager invoked on thread {0}", Thread.CurrentThread.ManagedThreadId);

                string[] commandList = Utilities.CommandList();
                string[] locationList = Utilities.ShortcutTargetList();

            {
                executablesList.Capacity = commandList.Length;
                for (int i = 0; i < commandList.Length; i++)
                {
                    executablesList.Add(new Executable(locationList[i], commandList[i]));
                    executablesList[i].PInfo.FileName = locationList[i];
                }
                executablesList.Sort();

                Console.WriteLine("InitializeManager() successful");
                Program.waitHandle.Set();
            }
            });


            //await Task.Run(() =>
            //{
            //    StreamWriter strWriter = new StreamWriter(@"H:\voix\txtFromInitManager.txt");

            //    foreach (var item in executablesList)
            //    {
            //        strWriter.WriteLine(item);
            //    }
            //    strWriter.Close();
                
            //});
        }

        static public void ShowRunningExecutables()
        {
            if (runningExecutables.Count != 0)
                foreach (var item in runningExecutables)
                    Console.WriteLine(item);

            else
                Console.WriteLine("The list is empty");
        }

        static public void SendOpenCommand(string command)
        {
            if (command.Contains("Tars"))
            {
                command = command.Remove(0, 5);
            }
            int i = GetIndexOfCommand(command,ref executablesList);
            StartProcess(i);
        }

        static public void SendCloseCommand(string command)
        {
            if (command.Contains("Tars Close"))
            {
                command = command.Remove(0,11);
            }
            int i = GetIndexOfCommand(command,ref runningExecutables);
            CloseProcess(i);
        }
        #endregion
    }
}