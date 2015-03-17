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
        private Settings settings;

        public SettingsForm(MainForm parent, Settings settings)
        {
            InitializeComponent();
            this.parent = parent;
            this.settings = settings;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            languageCombobox.Items.Add(settings.GetSettingOptions().languageOptions[0]);
            languageCombobox.Items.Add(settings.GetSettingOptions().languageOptions[1]);

            comboBox1.Items.Add(settings.GetSettingOptions().unitsOptions[0]);
            comboBox1.Items.Add(settings.GetSettingOptions().unitsOptions[1]);
            comboBox1.Items.Add(settings.GetSettingOptions().unitsOptions[2]);

            comboBox2.Items.Add(settings.GetSettingOptions().resolutionUnitsOptions[0]);
            comboBox2.Items.Add(settings.GetSettingOptions().resolutionUnitsOptions[1]);

            comboBox1.SelectedIndex = settings.GetSelectedUnitsIndex();
            comboBox2.SelectedIndex = settings.GetSelectedResolutionUnitsIndex();
            languageCombobox.SelectedIndex = settings.GetSelectedLanguageIndex();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            String selectedLanguage = ((StringValuePair<String>)languageCombobox.SelectedItem).value;
            parent.changeLanguage(selectedLanguage);

            settings.SetSelectedLanguageIndex(languageCombobox.SelectedIndex);
            settings.SetSelectedUnitsIndex(comboBox1.SelectedIndex);
            settings.SetSelectedResolutionUnitsIndex(comboBox2.SelectedIndex);
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
