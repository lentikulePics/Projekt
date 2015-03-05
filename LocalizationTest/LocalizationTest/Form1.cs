using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace LocalizationTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("English");
            comboBox1.Items.Add("Spanish");
            comboBox1.Items.Add("French");
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "English")
            {
                changeLanguage("en");
            }
            else if (comboBox1.SelectedItem.ToString() == "Spanish")
            {
                changeLanguage("es-ES");
            }

            else if (comboBox1.SelectedItem.ToString() == "French")
            {
                changeLanguage("fr-FR");
            }
        }

        private void changeLanguage(String lang)
        {
            foreach (Control c in this.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
                resources.ApplyResources(c, c.Name, new CultureInfo(lang));
            }
        }
    }
}
