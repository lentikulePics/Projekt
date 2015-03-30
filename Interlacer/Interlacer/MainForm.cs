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
        private ProjectData projectData = new ProjectData();
        private PreviewData previewData;
        private PictureInfoData infoData = new PictureInfoData();
        

        public MainForm()
        {
            InitializeComponent();

            Localization.changeCulture();
            Localization.iterateOverControls(this, Localization.resourcesMain);

            resetPictureInfo();

            previewData = new PreviewData(previewPicBox, previewPicBox.Image);

            lineColorButton.BackColor = Color.Black;
            backgroundColorButton.BackColor = Color.White;
            projectData.GetLineData().SetLineColor(Color.Black);
            projectData.GetLineData().SetBackgroundColor(Color.White);
            edgeRadioButton.Checked = true;
            projectData.GetLineData().SetLineThickness(1);
            actualPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Value);
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

        private void setPreview()
        {
            ListView.SelectedListViewItemCollection selectedItems = pictureListViewEx.SelectedItems;
            if (selectedItems.Count > 0)
                previewData.Show(selectedItems[0].SubItems[1].Text);
        }

        private void resetPictureInfo()
        {
            infoFilenameLabel.Text = "";
            infoDpiLabel.Text = "";
            infoWidthLabel.Text = "";
            infoHeightLabel.Text = "";
        }

        private void setPictureInfo()
        {
            ListView.SelectedListViewItemCollection selectedItems = pictureListViewEx.SelectedItems;
            if (selectedItems.Count > 0)
            {
                String path = selectedItems[0].SubItems[1].Text;
                Picture pic = infoData.GetInfo(path);
                infoDpiLabel.Text = "" + pic.GetXDpi();
                infoWidthLabel.Text = "" + pic.GetWidth();
                infoHeightLabel.Text = "" + pic.GetHeight();
                String[] strings = path.Split(new char[]{'\\'});
                String filename = strings[strings.Length - 1];
                infoFilenameLabel.Text = filename;
            }
        }

        private void setValuesFromPicture(Picture picture)
        {
            InterlacingData interlacingData = projectData.GetInterlacingData();
            if (interlacingData.GetPictureResolution() == 0)
            {
                double pictureResolution = UnitConverter.GetInFromUnits(picture.GetXDpi(), interlacingData.GetResolutionUnits());
                interlacingData.SetPictureResolution(pictureResolution);
            }
            if (interlacingData.GetWidth() == 0 && interlacingData.GetPictureResolution() != 0)
            {
                double width = UnitConverter.Transfer(picture.GetWidth() / interlacingData.GetDPI(), Units.In, interlacingData.GetUnits());
                interlacingData.SetWidth(width);
            }
            if (interlacingData.GetHeight() == 0 && interlacingData.GetPictureResolution() != 0)
            {
                double height = UnitConverter.Transfer(picture.GetHeight() / interlacingData.GetDPI(), Units.In, interlacingData.GetUnits());
                interlacingData.SetHeight(height);
            }
            if (interlacingData.GetLenticuleDensity() == 0 && pictureListViewEx.Items.Count != 0)
            {
                double density = interlacingData.GetPictureResolution() / pictureListViewEx.Items.Count;
                interlacingData.SetLenticuleDensity(density);
            }
            updateAllComponents();
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
            String filename;
            if (savePicFileDialog.ShowDialog() == DialogResult.OK)
                filename = savePicFileDialog.FileName;
            else return;

            List<Picture> picL = harvestPicList();
            PictureContainer picCon = new PictureContainer(picL, projectData.GetInterlacingData(), projectData.GetLineData(), interlaceProgressBar);
            picCon.CheckPictures();
            picCon.Interlace();
            Picture result = picCon.GetResult();
            result.Save(filename);
            result.Destroy();

            MessageBox.Show("Hotovo!");
            
            //PictureContainer pc = new PictureContainer(progressBar, label, co dal??);

            //Thread t = new Thread(pc.interlace);
            //t.start();
        }

        private void předvolbyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm = new SettingsForm(this, settings);
            settingsForm.ShowDialog();
        }

        private void changeMaxLineThickness()
        {
            lineThicknessTrackbar.Maximum = Math.Max(1, pictureListViewEx.Items.Count - 1);
            maxPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Maximum);
        }

        private void addPicButton_Click(object sender, EventArgs e)
        {
            addPicFileDialog.Multiselect = true;
            DialogResult result = addPicFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string[] chosenPictures = addPicFileDialog.FileNames;

                for (int i = 0; i < chosenPictures.Length; i++)
                {
                    var indeces = pictureListViewEx.SelectedIndices;
                    int selectedIndex;

                    // Pokud je vybrano vic, tak posuneme ten nad tim pod vybrane

                    // Pokud neni v listu nic vybrano
                    if (indeces.Count == 0)
                    {
                        selectedIndex = Convert.ToInt32(order - 1);
                    }
                    // Pokud je v listu neco vybrano
                    else
                    {
                        selectedIndex = Convert.ToInt32(indeces[0]) + 1;
                    }

                    int numOfPics = chosenPictures.Count();

                    //                    pictureListViewEx.Items.Add(Convert.ToString(order)).SubItems.Add(chosenPictures[i]);
                    pictureListViewEx.Items.Insert(selectedIndex, Convert.ToString(order)).SubItems.Add(chosenPictures[i]);

                    reOrder();

                    pictureListViewEx.Focus();
                    pictureListViewEx.Items[selectedIndex].Selected = false;

                    changeMaxLineThickness();
                }
                if (chosenPictures.Length > 0)
                {
                    Picture pic = new Picture(chosenPictures[0]);
                    pic.Ping();
                    setValuesFromPicture(pic);
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
               changeMaxLineThickness();
        }

        private void removePicButton_Click(object sender, EventArgs e)
        {
            int count = pictureListViewEx.SelectedItems.Count;
            for(int i = 0; i < count; i++) {
                pictureListViewEx.SelectedItems[0].Remove();
            }

            changeMaxLineThickness();
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
            var indeces = pictureListViewEx.SelectedIndices;

            // Pokud neni nic vybrano, return

            if (indeces.Count == 0)
            {
                return;
            }

            int selectedIndex = Convert.ToInt32(indeces[0]);

            pictureListViewEx.Focus();
            pictureListViewEx.Items[selectedIndex].Selected = true;

            if (selectedIndex == 0)
            {
                return;
            }

            string tmp = pictureListViewEx.Items[selectedIndex - 1].SubItems[1].Text;

            pictureListViewEx.Items[selectedIndex - 1].SubItems[1].Text = pictureListViewEx.Items[selectedIndex].SubItems[1].Text;
            pictureListViewEx.Items[selectedIndex].SubItems[1].Text = tmp;

            pictureListViewEx.Items[selectedIndex - 1].Selected = true;
            pictureListViewEx.Items[selectedIndex].Selected = false;
        }
        private void moveDownButton_Click(object sender, EventArgs e)
        {
            var indeces = pictureListViewEx.SelectedIndices;

            if (indeces.Count == 0)
            {
                return;
            }

            int selectedIndex = Convert.ToInt32(indeces[0]);

            pictureListViewEx.Focus();
            pictureListViewEx.Items[selectedIndex].Selected = true;

            if (selectedIndex == pictureListViewEx.Items.Count - 1)
            {
                return;
            }

            string tmp = pictureListViewEx.Items[selectedIndex + 1].SubItems[1].Text;

            pictureListViewEx.Items[selectedIndex + 1].SubItems[1].Text = pictureListViewEx.Items[selectedIndex].SubItems[1].Text;
            pictureListViewEx.Items[selectedIndex].SubItems[1].Text = tmp;

            pictureListViewEx.Items[selectedIndex + 1].Selected = true;
            pictureListViewEx.Items[selectedIndex].Selected = false;
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
            updateAllComponents();
        }

        private void heightNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetHeight(Convert.ToDouble(heightNumeric.Value));
            updateAllComponents();
        }

        private void updateAllComponents()
        {
            widthNumeric.Text = Convert.ToString(projectData.GetInterlacingData().GetWidth());
            heightNumeric.Text = Convert.ToString(projectData.GetInterlacingData().GetHeight());
            dpiNumeric.Text = Convert.ToString(projectData.GetInterlacingData().GetPictureResolution());
            lpiNumeric.Text = Convert.ToString(projectData.GetInterlacingData().GetLenticuleDensity());
            frameWidthNumeric.Text = Convert.ToString(projectData.GetLineData().GetFrameWidth());
            indentNumeric.Text = Convert.ToString(projectData.GetLineData().GetIndent());
            

            double pictureResolution = projectData.GetInterlacingData().GetPictureResolution();
            double lenticuleDensity = projectData.GetInterlacingData().GetLenticuleDensity();

            if (lenticuleDensity != 0)
            {
                picUnderLenTextBox.Text = Convert.ToString(pictureResolution / lenticuleDensity);
            }

            changeMaxLineThickness();
            actualPicsUnderLenLabel.Text = "" + projectData.GetLineData().GetLineThickness();

            if (Convert.ToInt32(actualPicsUnderLenLabel.Text) > lineThicknessTrackbar.Maximum)
            {
                lineThicknessTrackbar.Value = lineThicknessTrackbar.Maximum;
                actualPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Value);
            }

            widthInPixelsTextBox.Text = Convert.ToString((int)(projectData.GetInterlacingData().GetInchWidth() * projectData.GetInterlacingData().GetDPI()));
            heightInPixelsTextBox.Text = Convert.ToString((int)(projectData.GetInterlacingData().GetInchHeight() * projectData.GetInterlacingData().GetDPI()));

            resetPictureInfo();
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
            if (imagePreviewCheckBox.Checked)
                setPreview();
            setPictureInfo();
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
            projectData.GetInterlacingData().SetFinalResampleFilter(filter);
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

            updateAllComponents();
        }

        private void dpiNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetPictureResolution(Convert.ToDouble(dpiNumeric.Value));
            updateAllComponents();
        }

        private void lpiNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetLenticuleDensity(Convert.ToDouble(lpiNumeric.Value));
            updateAllComponents();
        }

        private void frameWidthNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetFrameWidth(Convert.ToDouble(frameWidthNumeric.Value));
        }

        private void indentNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetIndent(Convert.ToDouble(indentNumeric.Value));
        }

        private void topLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetTop(topLineCheckBox.Checked);
        }

        private void bottomLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetBottom(bottomLineCheckBox.Checked);
        }

        private void leftLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetLeft(leftLineCheckBox.Checked);
        }

        private void rightLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetRight(rightLineCheckBox.Checked);
        }

        private void centerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetCenterPosition(true);
        }

        private void edgeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetCenterPosition(false);
        }

        private void lineColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog lc = new ColorDialog();

            if(lc.ShowDialog() == DialogResult.OK) {
                lineColorButton.BackColor = lc.Color;
                projectData.GetLineData().SetLineColor(lc.Color);
            }
        }

        private void backgroundColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog bc = new ColorDialog();

            if(bc.ShowDialog() == DialogResult.OK) {
                backgroundColorButton.BackColor = bc.Color;
                projectData.GetLineData().SetBackgroundColor(bc.Color);
            }
        }

        private void lineThicknessTrackbar_ValueChanged(object sender, EventArgs e)
        {
            actualPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Value);
            projectData.GetLineData().SetLineThickness(lineThicknessTrackbar.Value);
        }

        private void verticalRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetDirection(Direction.Vertical);
        }

        private void horizontalRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetDirection(Direction.Horizontal);
        }

        private void copyPicButton_Click(object sender, EventArgs e)
        {
            changeMaxLineThickness();
        }

        private void imagePreviewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (imagePreviewCheckBox.Checked)
            {
                setPreview();
            }
            else
            {
                previewData.ShowDefaultImage();
            }
        }
    }
}
