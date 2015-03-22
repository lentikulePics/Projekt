using System;
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
    static class Localization
    {
        /// <summary>
        /// Udava, na jaky jazyk je program nastaven
        /// </summary>
        public static string currentLanguage = "cs-CZ";

        public static ComponentResourceManager resourcesMain = new ComponentResourceManager(typeof(MainForm));
        public static ComponentResourceManager resourcesSettings = new ComponentResourceManager(typeof(SettingsForm));
        public static ComponentResourceManager resourcesStrings;

        /// <summary>
        /// Meni nastaveni kultury programu podle promenne currentLanguage
        /// </summary>
        public static void changeCulture()
        {
            if(currentLanguage == "cs-CZ")
            {
                resourcesStrings = new ComponentResourceManager(typeof(StringRes_CZ));
            }
            else if (currentLanguage == "en")
            {
                resourcesStrings = new ComponentResourceManager(typeof(StringRes_EN));
            }
           
            Thread.CurrentThread.CurrentCulture = new CultureInfo(currentLanguage);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentLanguage);
        }

        /// <summary>
        /// Rekurzivne projde vsechny komponenty formu a nastavi jim aktualne pouzivany jazyk
        /// </summary>
        /// <param name="parent">Kopmonenta pres kterou se bude iterovat</param>
        public static void iterateOverControls(Control parent, ComponentResourceManager res)
        {

            foreach (Control c in parent.Controls)
            {
                res.ApplyResources(c, c.Name, new CultureInfo(Localization.currentLanguage));
                if (c.GetType() == typeof(GroupBox))
                {
                    iterateOverControls(c, res);
                }

                if (c.GetType() == typeof(MenuStrip))
                {
                    foreach (ToolStripMenuItem it in ((MenuStrip)c).Items)
                    {
                        res.ApplyResources(it, it.Name, new CultureInfo(Localization.currentLanguage));

                        foreach (ToolStripItem f in it.DropDownItems)
                        {
                            if (typeof(ToolStripSeparator) != f.GetType())
                                res.ApplyResources(f, f.Name, new CultureInfo(Localization.currentLanguage));

                        }
                    }
                }
                else
                {
                    iterateOverControls(c, res);
                }
            }
        }
    }
}
