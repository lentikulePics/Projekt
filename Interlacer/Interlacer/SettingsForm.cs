using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interlacer
{
    public partial class SettingsForm : Form
    {
        private MainForm parent;

        public SettingsForm(MainForm parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            languageCombobox.SelectedIndex = 0;

            comboBox1.Items.Add(new KeyValuePair<String, Units>("cm", Units.Cm));
            comboBox1.Items.Add(new KeyValuePair<String, Units>("mm", Units.Mm));
            comboBox1.Items.Add(new KeyValuePair<String, Units>("in", Units.In));

            comboBox2.Items.Add(new KeyValuePair<String, Units>("DPI / LPI", Units.In));
            comboBox2.Items.Add(new KeyValuePair<String, Units>("DPCM / LPCM", Units.Cm));

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
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

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
