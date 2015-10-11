﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Voix
{
    public enum UserGender                  //enum that sets the gender of the user, and correspondingly sets how Tars acknowledges you. i.e Sir or Ma'am
    {
        Male,
        Female
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
}