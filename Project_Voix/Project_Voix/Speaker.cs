﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows;

namespace Project_Voix
{
    static class Speaker
    {


        static SpeechSynthesizer mainSynthesizer = null;
        public static void Synthesizer(object resp)
        {
            Console.WriteLine("Synthesizer Method on thread : {0}", Thread.CurrentThread.ManagedThreadId);
            string response = null;
            try
            {
                response = resp as string;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Generated by {0}", e.Source);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            mainSynthesizer = new SpeechSynthesizer();
            mainSynthesizer.Speak(response);
        }
    }
}