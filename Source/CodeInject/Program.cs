﻿using CodeInject;
using CodeInject.Actors;
using CodeInject.MemoryTools;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace ISpace
{

    public class IClass
    {
        public unsafe static int IMain(string args)
        {
             cBot cBot = new cBot();
             cBot.ShowDialog();
 
            return 0;
        }

        [DllImport("kernel32")]
        static extern bool AllocConsole();
    }
}
