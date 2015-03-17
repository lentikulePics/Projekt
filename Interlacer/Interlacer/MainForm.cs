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
using GfxlibWrapper;

namespace Interlacer
{
    public partial class MainForm : Form
    {
        private Settings settings;

        public MainForm()
        {
            //prozatimni reseni, pak bude potreba dodat retezce z recource filu
            SettingOptions settingOptions = new SettingOptions();
            settingOptions.languageOptions = new List<StringValuePair<String>>
            {
                new StringValuePair<String>("Čeština", "cs-CZ"),
                new StringValuePair<String>("Angličtina", "en")
            };
            settingOptions.unitsOptions = new List<StringValuePair<Units>>
            {
                new StringValuePair<Units>("cm", Units.Cm),
                new StringValuePair<Units>("mm", Units.Mm),
                new StringValuePair<Units>("palce", Units.In)
            };
            settingOptions.resolutionUnitsOptions = new List<StringValuePair<Units>>
            {
                new StringValuePair<Units>("DPI, LPI", Units.In),
                new StringValuePair<Units>("DPCM, LPCM", Units.Cm)
            };
            settings = new Settings(settingOptions);
            settings.SetSelectedLanguageIndex(0);
            settings.SetSelectedUnitsIndex(0);
            settings.SetSelectedResolutionUnitsIndex(0);

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

            /*picListViewEx.Items.Add("adsfasdsd");
            picListViewEx.Items.Add("dsadsfasdfasdfsfd");
            picListViewEx.Items.Add("ad");*/
            
        }

        private void interlaceButton_Click(object sender, EventArgs e)
        {
            List<Picture> pics = new List<Picture>
            {
                new Picture("pics/red.jpg"),
                new Picture("pics/blue.jpg"),
                new Picture("pics/green.jpg")
            };

            InterlacingData intData = new InterlacingData();
            intData.SetUnits(Units.Cm);
            intData.SetResolutionUnits(Units.In);
            intData.SetWidth(21);
            intData.SetHeight(29.7);
            intData.SetPictureResolution(1200);
            intData.SetLenticuleDensity(39.85);
            intData.SetInitialResizeFilter(FilterType.Cubic);
            intData.SetFinalResampleFilter(FilterType.Cubic);
            LineData lineData = new LineData();
            lineData.SetUnits(Units.Cm);
            lineData.SetTop(true);
            lineData.SetRight(true);
            lineData.SetBottom(true);
            lineData.SetLeft(true);
            lineData.SetIndent(0.5);
            lineData.SetFrameWidth(1);
            lineData.SetLineThickness(1);
            lineData.SetBackgroundColor(Color.White);
            lineData.SetLineColor(Color.Black);
            PictureContainer picCon = new PictureContainer(pics, intData, lineData, interlaceProgressBar);

            picCon.CheckPictures();
            picCon.Interlace();
            picCon.GetResult().Save("result.tif");
            picCon.GetResult().Destroy();
            MessageBox.Show("Done!");
            //PictureContainer pc = new PictureContainer(progressBar, label, co dal??);

            //Thread t = new Thread(pc.interlace);
            //t.start();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void předvolbyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this, settings);
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

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
