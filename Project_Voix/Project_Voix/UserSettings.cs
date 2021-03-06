﻿/*
    description: UserSettings class
           -class that represents the various users that use Voix and stores their values as binary files
    
    date created: -12/10/15

    log:-
    *No Updates Done*

    Listed Public Methods:
           *NONE*
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Project_Voix
{
    [Serializable]
    public class UserSettings
    {

        #region Fields
        string programPetname = "Tars";                             //default is Tars
        UserGender userGender;                        //default is Male
        VoiceGender synthVoiceGender = VoiceGender.Male;            //default is Male
        string userName;
        string moviesFolderLoc = "";
        int synthVol = 100;
        int synthRate = 0;
        string userImageSource;                                         //stores the source address of the User Image
        #endregion

        #region Properties

        public string Movies
        {
            get { return moviesFolderLoc; }
            set { moviesFolderLoc = value; }
        }

        public string ImageSource
        {
            get { return userImageSource; }
            set { userImageSource = value; }
        }
        public string Username
        {
            get { return userName; }
            set
            {
                if (value != "")
                    userName = value;
                else
                    throw new NotSupportedException("The username field cannot be empty");
            }
        }

        public UserGender Gender
        {
            get { return userGender; }
            set { userGender = value; }
        }

        public VoiceGender SynthesizerVoiceGender
        {
            get { return synthVoiceGender; }
            set { synthVoiceGender = value; }
        }

        

        public int SynthesizerVolume
        {
            get { return synthVol; }
            set
            {
                if (value != 0)
                    synthVol = value;
                else
                    throw new InvalidOperationException("The volume cannot be zero");
            }
        }

        public int SynthesizerRate
        {
            get { return synthRate; }
            set
            {
                if (value <= 10 & value >= -10)
                    synthRate = value;
                else
                    throw new InvalidOperationException("The rate of the Synthesizer can only lie between 10 and -10 ,inclusive");
            }
        }

        public string AssistantName                                     //to set the petname of the program other than Tars
        {
            get { return programPetname; }
            set
            {
                if (value != "")
                    programPetname = value;
                else
                    throw new NotSupportedException("The petname of the digital assistant cannot be empty");
            }
        }
        #endregion

        #region Constructor
        public UserSettings(string username, UserGender usergender, string assistantName = "Tars", VoiceGender voiceGender = VoiceGender.Male, int synthVol = 100, int synthRate = 0)
        {
            Username = username;
            Gender = usergender;
            AssistantName = assistantName;
            SynthesizerVoiceGender = voiceGender;
            //Movies = moviesFolder;
            SynthesizerVolume = synthVol;
            SynthesizerRate = synthRate;
        }
        #endregion

        #region Static Public Methods
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        static void WriteSettingsToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            //Directory.CreateDirectory(filePath);
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }


        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        static T ReadFromBinaryFile<T>(string filePath)
        {

            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
        #endregion

        #region Public Methods
        public async void WriteSettings(string location = @"C:\Users\HEWLETT PACKARD\Documents\Project Voix")
        {
            Directory.CreateDirectory(location);
            await Task.Run(() =>
            {
                WriteSettingsToBinaryFile<UserSettings>(location + @"\" + this.Username.ToLower() + ".file", this, false);
            });
        }

        public static UserSettings GetSettings(string filePath, string userName = "")
        {
            if (userName !="")
            {
                filePath +=  @"\" + userName.ToLower() + ".file";
                Console.WriteLine("filepath is {0}",filePath);
            }
            return ReadFromBinaryFile<UserSettings>(filePath);
        }

        public override string ToString()
        {
            return string.Format("Username : {0}\t Gender : {1}", this.Username, this.Gender);
        }
        #endregion
    }
}
