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
        private SettingsForm settingsForm;

        private int order = 1;
        public ProjectData projectData = new ProjectData();

        public MainForm()
        {
            //Nastaveni jazyka na vychozi hodnotu(Cestina)
            Localization.changeCulture();

            InitializeComponent();

            //prozatimni reseni, pak bude potreba dodat retezce z recource filu
            SettingOptions settingOptions = new SettingOptions();
            settingOptions.languageOptions = new List<StringValuePair<String>>
            {
                new StringValuePair<String>(Localization.resourcesStrings.GetString("langCzech"), "cs-CZ"),
                new StringValuePair<String>(Localization.resourcesStrings.GetString("langEnglish"), "en")
            };
            settingOptions.unitsOptions = new List<StringValuePair<Units>>
            {
                new StringValuePair<Units>("cm", Units.Cm),
                new StringValuePair<Units>("mm", Units.Mm),
                new StringValuePair<Units>(Localization.resourcesMain.GetString("unitsInches"), Units.In)
            };
            settingOptions.resolutionUnitsOptions = new List<StringValuePair<Units>>
            {
                new StringValuePair<Units>("DPI, LPI", Units.In),
                new StringValuePair<Units>("DPCM, LPCM", Units.Cm)
            };

            interpol1ComboBox.Items.Add(new StringValuePair<FilterType>("Nejbližší soused", FilterType.None));
            interpol1ComboBox.Items.Add(new StringValuePair<FilterType>("Lineární", FilterType.Triangle));
            interpol1ComboBox.Items.Add(new StringValuePair<FilterType>("Kubický", FilterType.Cubic));
            interpol1ComboBox.Items.Add(new StringValuePair<FilterType>("Lanczos", FilterType.Lanczos));

            interpol2ComboBox.Items.Add(new StringValuePair<FilterType>("Nejbližší soused", FilterType.None));
            interpol2ComboBox.Items.Add(new StringValuePair<FilterType>("Lineární", FilterType.Triangle));
            interpol2ComboBox.Items.Add(new StringValuePair<FilterType>("Kubický", FilterType.Cubic));
            interpol2ComboBox.Items.Add(new StringValuePair<FilterType>("Lanczos", FilterType.Lanczos));

            

            settings = new Settings(settingOptions);
            settings.SetSelectedLanguageIndex(0);
            settings.SetSelectedUnitsIndex(0);
            settings.SetSelectedResolutionUnitsIndex(0);           

            //unitsComboBox.SelectedItem = unitsComboBox.Items[0];
            interpol1ComboBox.SelectedItem = interpol1ComboBox.Items[0];
            interpol2ComboBox.SelectedItem = interpol2ComboBox.Items[0];

            changeUnits();

            pictureListViewEx.MultiSelect = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*picListViewEx.Items.Add("adsfasdsd");
            picListViewEx.Items.Add("dsadsfasdfasdfsfd");
            picListViewEx.Items.Add("ad");*/            
        }

        /// <summary>
        /// 
        /// </summary>
        public void changeLanguage()
        {
            Localization.iterateOverControls(this, Localization.resourcesMain);
        }

        private void interlaceButton_Click(object sender, EventArgs e)
        {
            harvestPicList();

            /*List<Picture> pics = new List<Picture>
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
            MessageBox.Show("Done!");*/
            //PictureContainer pc = new PictureContainer(progressBar, label, co dal??);

            //Thread t = new Thread(pc.interlace);
            //t.start();
        }

        private void předvolbyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm = new SettingsForm(this, settings);
            settingsForm.ShowDialog();
        }        

        private void addPicButton_Click(object sender, EventArgs e)
        {
            addPicFileDialog.Multiselect = true;
            DialogResult result = addPicFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string [] chosenPictures = addPicFileDialog.FileNames;

                for (int i = 0; i < chosenPictures.Length; i++)
                {
                    pictureListViewEx.Items.Add(Convert.ToString(order)).SubItems.Add(chosenPictures[i]);
                    order += 1;
                }
            }
        }

        private List<Picture> harvestPicList()
        {
            String path;
            List<Picture> picList = new List<Picture>();

            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                path = pictureListViewEx.Items[i].SubItems[1].Text;
                picList.Add(new Picture(path));
            }
            return picList;
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
               pictureListViewEx.Items.Clear();
               order = 1;
        }

        private void removePicButton_Click(object sender, EventArgs e)
        {
            int count = pictureListViewEx.SelectedItems.Count;
            for(int i = 0; i < count; i++) {
                pictureListViewEx.SelectedItems[0].Remove();
            }

            reOrder();
        }

        private void reOrder()
        {
            order = 1;

            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                pictureListViewEx.Items[i].SubItems[0].Text = Convert.ToString(order);
                order += 1;
            }
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {

        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            List<String> tmp_paths = new List<String>();
            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                tmp_paths.Add(pictureListViewEx.Items[i].SubItems[1].Text);
            }

            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                pictureListViewEx.Items[i].SubItems[1].Text = tmp_paths[tmp_paths.Count - i - 1];
            }
        }

        private void widthNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetWidth(Convert.ToDouble(widthNumeric.Value));
        }

        private void heightNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetHeight(Convert.ToDouble(heightNumeric.Value));
        }

        private void keepRatioCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().KeepAspectRatio(keepRatioCheckbox.Checked);
        }

        private void reorderTimer_Tick(object sender, EventArgs e)
        {
            //reOrder();
        }

        private void pictureListViewEx_DragDrop(object sender, DragEventArgs e)
        {
            //e.Effect = DragDropEffects.All;
            //reOrder();
        }

        private void pictureListViewEx_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //reOrder();
        }

        private void pokus(object sender, EventArgs e)
        {
            reOrder();
        }

        private void interpol1ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;

            FilterType filter = ((StringValuePair<FilterType>)c.SelectedItem).value;
            projectData.GetInterlacingData().SetInitialResizeFilter(filter);
        }

        private void interpol2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox) sender;

            FilterType filter = ((StringValuePair<FilterType>)c.SelectedItem).value;
            projectData.GetInterlacingData().SetInitialResizeFilter(filter);
        }

        public void changeUnits()
        {
            projectData.GetInterlacingData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
            projectData.GetLineData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
            projectData.GetInterlacingData().SetResolutionUnits(((StringValuePair<Units>)settings.GetSelectedResolutionUnits()).value);

            string measureUnits = settings.GetSelectedUnits().ToString();
            string[] resolutionUnits = settings.GetSelectedResolutionUnits().ToString().Split(new char[] { ',', ' ' });

            //musime splitnout

            unitsLabel.Text = measureUnits;
            unitsLabel2.Text = measureUnits;

            unitsLabel3.Text = measureUnits;
            unitsLabel4.Text = measureUnits;

            dpiLabel.Text = resolutionUnits[0];
            lpiLabel.Text = resolutionUnits[2];            
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}
