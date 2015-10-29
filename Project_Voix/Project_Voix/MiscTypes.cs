/*
    description: MiscTypes code file
            -contains the various publically available common non-class types that are used by multiple components of the program
            - contains UserGender Enum,CommandType Enum and GenerateResponse delegate types
    date created: -11/10/15                     Author: Sarthak

    log:-
    *No Updates Done*

    Listed Public Methods:
            *None*
*/

using System;
using System.IO;
using System.Speech;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project_Voix
{
    public enum UserGender                  //enum that sets the gender of the user, and correspondingly sets how Tars acknowledges you. i.e Sir or Ma'am
    {
        Male,
        Female
    }

    public enum ResponseType
    {
        Open,
        Search
    }
    public enum CommandType                 //enum that specifies the command type that has been given
    {
        Open,
        Search,
        NonOperational,
        UI,
        Basic,
        CloseProgram,
        ResponseBox,
    }

    
    delegate void GenerateResponse(Response response);              // delegate for signatures of Response events

    public delegate void UpdateLog(string logUpdate);                      //delegate to update the text box with recognized text
    delegate void StartResponseBox(string respBoxType);

    delegate void ShowOpenTypeRecog(string recogPhrase);
    public delegate void SetUser(UserSettings user);
    delegate void CloseResponseBox();
    public delegate void SetSynthesisVolume(int vol);
    public delegate void SetSynthesisRate(int rate);
    public delegate void ChangeSlider(int i,string sliderType);                     //delegate for event that involves changing the slider in MainWindow
    public delegate void SynthesisGenderChange(VoiceGender voiceGender);            //delegate for event that detects change in VoiceGender selection
}