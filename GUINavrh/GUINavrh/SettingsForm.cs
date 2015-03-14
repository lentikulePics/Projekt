using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUINavrh
{
    public partial class SettingsForm : Form
    {
        private Form1 parent;

        public SettingsForm(Form1 parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            string selectedLanguage = languageCombobox.SelectedItem.ToString();

            if (selectedLanguage == "Čeština")
            {
                parent.changeLanguage("cs-CZ");
            }
            else if (selectedLanguage == "Angličtina")
            {
                parent.changeLanguage("en");
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
