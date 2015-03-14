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
    public partial class Form1 : Form
    {
        public Form1()
        {
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
            foreach (Control c in this.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
                resources.ApplyResources(c, c.Name, new CultureInfo(lang));
            }
        }

        private void loop(Control parent)
        {

            /*foreach (Control c in parent.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.TextChanged += new EventHandler(C_TextChanged);
                }
                else
                {
                    AddTextChangedHandler(c);
                }
            }*/

            
        }
    }
}
