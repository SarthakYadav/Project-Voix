/*
    description: static class Utilities
           -static class
           -contains methods for accomplishing various misc tasks throughout the program 
    
    date created: -03/10/15

    log:-
    *No Updates Done*

    Listed Public Methods:
           1. StartMenuFilesList(): returns the list of files in the Start menu Directory of the System
           2. CommandList(): for description see the Method Body
           3. CommandName(string str): returns the Effective Command name by string manipulation. See method body for details
           4. ShortCutTargetFinder(string path) method: returns the target file location of a given shortcut file
           5. ShortcutTargetList():  generates a list of target files for respective shortcuts in the StartMenu
                                     uses the ShortCutTargetFinder(string path) method.
          
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Voix
{
    static class Utilities
    { 
        #region Private Methods
        private static string ShortCutTargetFinder(string path)
        {
            /* finds target path of a .lnk shortcut file
            prerequisites: 
            COM object: Windows Script Host Object Mode
            */
            if (System.IO.File.Exists(path))
            {
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell(); //Create a new WshShell Interface
                IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path); //Link the interface to our shortcut
                return link.TargetPath;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        #endregion

        #region Public Methods
        public static string[] ShortcutTargetList()
        {
            /*
                Returns the complete list of target files to all the shortcut files that are present in the Start Menu
            */
            string[] filesList = StartMenuFilesList();
            string[] targetList = new string[filesList.Length];

            for (int i = 0; i < filesList.Length; ++i)
            {
                targetList[i] = ShortCutTargetFinder(filesList[i]);
            }
            return targetList;
        }

        public static string[] StartMenuFilesList()
        {
            /*
                returns list of shortcut files present in the Start Menu
            */
            string startMenuLoc = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);         //gets the location of the Start menu on current machine
            string[] filesList = Directory.GetFiles(startMenuLoc, "*.lnk", SearchOption.AllDirectories);        //a string array consisting of shortcut files(*.lnk) in the start menu directory
            return filesList;
        }

        public static string[] CommandList()
        {
            /*
                returns list of Commands which are used by the Open_type command grammar as well as 
                the ProgramManager class to store recognizable commands for respective executables
            */
            string[] filesList = StartMenuFilesList();
            string[] programCommands = new string[filesList.Length];
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < filesList.Length; ++i)
            {
                programCommands[i] = CommandName(filesList[i]);            //gets the command name for the given program in the filesList to be used for recognition
            }
            watch.Stop();
            DataStore.AddToMessageDump(string.Format("In command list generation , milliseconds taken : {0}", watch.ElapsedTicks));
            return programCommands;
        }
        public static string CommandName(string str)
        {
            //returns a substring from given string from last '/' character to the end of the string and replaces the ".lnk" with a blankspace
            return str.Substring(str.LastIndexOf('\\') + 1).Replace(".lnk", "");
        }
        #endregion
    }
}
