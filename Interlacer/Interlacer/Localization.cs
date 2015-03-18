﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;

namespace Interlacer
{
    class Localization
    {
        /// <summary>
        /// Udava, na jaky jazyk je program nastaven
        /// </summary>
        public static string currentLanguage = "cs-CZ";
        
        public static ComponentResourceManager resources;

        /// <summary>
        /// Meni nastaveni kultury programu podle promenne currentLanguage
        /// </summary>
        public static void changeCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(currentLanguage);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentLanguage);
        }

        /// <summary>
        /// Rekurzivne projde vsechny komponenty formu a nastavi jim aktualne pouzivany jazyk
        /// </summary>
        /// <param name="parent">Kopmonenta pres kterou se bude iterovat</param>
        public static void iterateOverControls(Control parent)
        {            

            foreach (Control c in parent.Controls)
            {
                resources.ApplyResources(c, c.Name, new CultureInfo(Localization.currentLanguage));
                if (c.GetType() == typeof(GroupBox))
                {
                    iterateOverControls(c);
                }
                else
                {
                    iterateOverControls(c);
                }
            }
        }
    }
}
