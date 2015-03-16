using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUINavrh
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            // Sets the UI culture to French (France)
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("cs-CZ");

            InitializeComponent();
            unitsComboBox.SelectedItem = unitsComboBox.Items[0];
            interpol1ComboBox.SelectedItem = interpol1ComboBox.Items[0];
            interpol2ComboBox.SelectedItem = interpol2ComboBox.Items[0];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs");
            picListViewEx.Items.Add("adsfasdsd");
            picListViewEx.Items.Add("dsadsfasdfasdfsfd");
            picListViewEx.Items.Add("ad");
            
        }

        private void interlaceButton_Click(object sender, EventArgs e)
        {
            //PictureContainer pc = new PictureContainer(progressBar, label, co dal??);

            //Thread t = new Thread(pc.interlace);
            //t.start();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void předvolbyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this);
            settingsForm.ShowDialog();
        }

        public void changeLanguage(String lang)
        {
            loop(this, lang);
        }

        private void loop(Control parent, string lang)
        {

            foreach (Control c in parent.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));    
                if (c.GetType() == typeof(GroupBox))
                {
                      loop(c, lang);
                }
                else
                {
                    loop(c, lang);
                }
            }            
        }
    }
}
