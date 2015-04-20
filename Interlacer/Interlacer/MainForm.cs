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
using System.IO;

namespace Interlacer
{
    public partial class MainForm : Form
    {
        private const String settingsFilename = "settings";
        private Settings settings;
        private SettingsForm settingsForm;

        private int order = 1;
        private ProjectData projectData = new ProjectData();
        private PreviewData previewData;
        private PictureInfoData infoData = new PictureInfoData();

        private ToolTip t = new ToolTip();        

        public MainForm()
        {
            if (!GfxlibCommunicator.Test())
            {
                MessageBox.Show("Chyba při načítání grafické knihovny.\nPravděpodobnou příčinou je chybějící Visual C++ Redistributable");
                System.Environment.Exit(0);
            }
            InitializeComponent();
            reorderTimer.Stop();

            resetPictureInfo();

            previewData = new PreviewData(previewPicBox, previewPicBox.Image);

            lineColorButton.BackColor = Color.Black;
            backgroundColorButton.BackColor = Color.White;
            projectData.GetLineData().SetLineColor(Color.Black);
            projectData.GetLineData().SetBackgroundColor(Color.White);
            edgeRadioButton.Checked = true;
            projectData.GetLineData().SetLineThickness(1);
            actualPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Value);

            loadSettings();

            Localization.currentLanguage = settings.GetSelectedLanguage().value;
            Localization.changeCulture();
            Localization.iterateOverControls(this, Localization.resourcesMain);

            updateTexts();

            changeUnits();

            pictureListViewEx.MultiSelect = true;

            interpol1ComboBox.SelectedIndex = 0;
            interpol2ComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Zabrani blikani GUI pri prekreslovani
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void loadSettings()
        {
            settings = new Settings(createSettingOptions());
            try
            {
                settings.Load(settingsFilename);
            }
            catch
            {
                settings.SetSelectedLanguageIndex(0);
                settings.SetSelectedUnitsIndex(0);
                settings.SetSelectedResolutionUnitsIndex(0);
            }
        }

        private SettingOptions createSettingOptions()
        {
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
                new StringValuePair<Units>(Localization.resourcesStrings.GetString("unitsInches"), Units.In)
            };
            settingOptions.resolutionUnitsOptions = new List<StringValuePair<Units>>
            {
                new StringValuePair<Units>("DPI, LPI", Units.In),
                new StringValuePair<Units>("DPCM, LPCM", Units.Cm)
            };

            return settingOptions;
        }

        /// <summary>
        /// 
        /// </summary>
        public void updateTexts()
        {
            settings.SetSettingOptions(createSettingOptions());

            if (interpol1ComboBox.Items.Count == 0 && interpol2ComboBox.Items.Count == 0)
            {
                interpol1ComboBox.Items.Add("");
                interpol1ComboBox.Items.Add("");
                interpol1ComboBox.Items.Add("");
                interpol1ComboBox.Items.Add("");

                interpol2ComboBox.Items.Add("");
                interpol2ComboBox.Items.Add("");
                interpol2ComboBox.Items.Add("");
                interpol2ComboBox.Items.Add("");
            }            

            interpol1ComboBox.Items[0] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterNearestNeighbor"), FilterType.None);
            interpol1ComboBox.Items[1] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterLinear"), FilterType.Triangle);
            interpol1ComboBox.Items[2] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterCubic"), FilterType.Cubic);
            interpol1ComboBox.Items[3] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterLanczos"), FilterType.Lanczos);

            interpol2ComboBox.Items[0] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterNearestNeighbor"), FilterType.None);
            interpol2ComboBox.Items[1] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterLinear"), FilterType.Triangle);
            interpol2ComboBox.Items[2] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterCubic"), FilterType.Cubic);
            interpol2ComboBox.Items[3] = new StringValuePair<FilterType>(Localization.resourcesStrings.GetString("filterLanczos"), FilterType.Lanczos);


            //Nastavi tooltipy
           
            t.SetToolTip(addPicButton, Localization.resourcesStrings.GetString("addPicTooltip"));
            t.SetToolTip(removePicButton, Localization.resourcesStrings.GetString("removePicTooltip"));
            t.SetToolTip(copyPicButton, Localization.resourcesStrings.GetString("copyPicTooltip"));
            t.SetToolTip(moveUpButton, Localization.resourcesStrings.GetString("moveUpTooltip"));
            t.SetToolTip(moveDownButton, Localization.resourcesStrings.GetString("moveDownTooltip"));
            t.SetToolTip(clearAllButton, Localization.resourcesStrings.GetString("clearAllTooltip"));
            t.SetToolTip(revertButton, Localization.resourcesStrings.GetString("revertTooltip"));

            // Nastaveni sloupcu listview
            pictureListViewEx.Columns[0].Text = Localization.resourcesStrings.GetString("orderListView");
            pictureListViewEx.Columns[1].Text = Localization.resourcesStrings.GetString("pathListView");
        }

        private void setPreview()
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItems = pictureListViewEx.SelectedItems;
                if (selectedItems.Count > 0)
                    previewData.Show(selectedItems[0].SubItems[1].Text);
            }
            catch
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("previewError"));
                previewData.ShowDefaultImage();
            }
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
            try
            {
                ListView.SelectedListViewItemCollection selectedItems = pictureListViewEx.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    String path = selectedItems[0].SubItems[1].Text;
                    Picture pic = infoData.GetInfo(path);
                    infoDpiLabel.Text = "" + pic.GetXDpi();
                    infoWidthLabel.Text = "" + pic.GetWidth();
                    infoHeightLabel.Text = "" + pic.GetHeight();
                    String[] strings = path.Split(new char[] { '\\' });
                    String filename = strings[strings.Length - 1];
                    infoFilenameLabel.Text = filename;
                }
            }
            catch
            {
                //v pripade neuspesneho nacteni obrazku se v teto fazi pouze vymazou informace o obrazku
                resetPictureInfo();
            }
        }

        private void setValuesFromPicture(Picture picture)
        {
            InterlacingData interlacingData = projectData.GetInterlacingData();
            if (interlacingData.GetWidth() == 0 && picture.GetXDpi() != 0)
            {
                double width = UnitConverter.Transfer(picture.GetWidth() / picture.GetXDpi(), Units.In, interlacingData.GetUnits());
                interlacingData.SetWidth(width);
            }
            if (interlacingData.GetHeight() == 0 && picture.GetXDpi() != 0)
            {
                double height = UnitConverter.Transfer(picture.GetHeight() / picture.GetXDpi(), Units.In, interlacingData.GetUnits());
                interlacingData.SetHeight(height);
            }
            updateAllComponents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;            
        }

        /// <summary>
        /// 
        /// </summary>
        public void changeLanguage()
        {
            // Updatuje texty v komboboxech
            updateTexts();

            Localization.iterateOverControls(this, Localization.resourcesMain);
            Invalidate();
        }

        private Boolean checkExtension(String name)
        {
            String[] split = name.Split('.');
            int lastIndex = split.Length - 1;

            if (split[lastIndex].Equals("jpg") || split[lastIndex].Equals("png") || split[lastIndex].Equals("bmp") || split[lastIndex].Equals("tif"))
            {
                return true;
            }

            return false;
        }

        private void interlaceButton_Click(object sender, EventArgs e)
        {
            String filename;
            savePicFileDialog.Filter = "JPEG|*.jpg;*.jpeg|PNG|*.png|BMP|*.bmp|TIF|*.tif";
            savePicFileDialog.AddExtension = true;
            if (savePicFileDialog.ShowDialog() == DialogResult.OK) {
                

                filename = savePicFileDialog.FileName;
            } else return;

            if (checkExtension(filename))
                savePicFileDialog.AddExtension = false;


            List<Picture> picList = harvestPicList();
            if (picList.Count == 0)
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("emptyListError"));
                return;
            }
            PictureContainer picCon = new PictureContainer(picList, projectData.GetInterlacingData(), projectData.GetLineData(), interlaceProgressBar);
            try
            {
                if (!picCon.CheckPictures())
                {
                    DialogResult dialogResult = MessageBox.Show(Localization.resourcesStrings.GetString("imageDimensionError"), "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                        return;
                }
                picCon.Interlace();
            }
            catch (PictureLoadFailureException ex)
            {
                MessageBox.Show("Soubor " + ex.filename + " se nepodařilo otevřít.\nSoubor pravděpodobně neexistuje.");
                interlaceProgressBar.Value = 0;
                return;
            }
            catch (PictureWrongFormatException ex)
            {
                MessageBox.Show("Soubor " + ex.filename + " má chybný formát.");
                interlaceProgressBar.Value = 0;
                return;
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("memoryError"));
                interlaceProgressBar.Value = 0;
                return;
            }
            catch (PictureProcessException)
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("interlacingError"));
                interlaceProgressBar.Value = 0;
                return;
            }
            Picture result = picCon.GetResult();
            try
            {
                result.Save(filename);
            }
            catch (PictureSaveFailureException ex)
            {
                MessageBox.Show("Obrázek " + ex.filename + " se nepodařilo uložit.");
            }
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
            addPicFileDialog.Filter = "Image Files (*.jpeg, *.jpg, *.png, *.bmp, *.tif)|*.jpeg;*.jpg;*.png;*.bmp;*.tif";
            addPicFileDialog.FilterIndex = 1;

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

                    reorder();

                    pictureListViewEx.Focus();
                    pictureListViewEx.Items[selectedIndex].Selected = false;

                    changeMaxLineThickness();
                }
                if (chosenPictures.Length > 0)
                {
                    Picture pic = new Picture(chosenPictures[0]);
                    try
                    {
                        pic.Ping();
                    }
                    catch
                    {
                        return;  //pri neuspesnem nacteni obrazku se v teto fazi pouze nenastavi komponenty formulare
                    }
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
            projectData.GetInterlacingData().SetWidth(0);
            projectData.GetInterlacingData().SetHeight(0);
            changeMaxLineThickness();
            updateAllComponents();
            previewData.ShowDefaultImage();
        }

        private void removePicButton_Click(object sender, EventArgs e)
        {
            int count = pictureListViewEx.SelectedItems.Count;
            for(int i = 0; i < count; i++) {
                pictureListViewEx.SelectedItems[0].Remove();
            }

            changeMaxLineThickness();
            updateAllComponents();
            reorder();
        }

        private void reorder()
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

            if (projectData.GetInterlacingData().GetDirection() == Direction.Horizontal)
            {
                horizontalRadiobutton.Checked = true;
            }
            else
            {
                verticalRadiobutton.Checked = true;
            }

            for (int i = 0; i < interpol1ComboBox.Items.Count; i++)
                if (projectData.GetInterlacingData().GetInitialResizeFilter() == ((StringValuePair<FilterType>)interpol1ComboBox.Items[i]).value)
                    interpol1ComboBox.SelectedIndex = i;
            for (int i = 0; i < interpol2ComboBox.Items.Count; i++)
                if (projectData.GetInterlacingData().GetFinalResampleFilter() == ((StringValuePair<FilterType>)interpol2ComboBox.Items[i]).value)
                    interpol2ComboBox.SelectedIndex = i;

            lineColorButton.BackColor = projectData.GetLineData().GetLineColor();
            backgroundColorButton.BackColor = projectData.GetLineData().GetBackgroundColor();

            topLineCheckBox.Checked = projectData.GetLineData().GetTop();
            leftLineCheckBox.Checked = projectData.GetLineData().GetLeft();
            rightLineCheckBox.Checked = projectData.GetLineData().GetRight();
            bottomLineCheckBox.Checked = projectData.GetLineData().GetBottom();

            centerRadioButton.Checked = projectData.GetLineData().GetCenterPosition();
            edgeRadioButton.Checked = !projectData.GetLineData().GetCenterPosition();

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
            reorder();
            reorderTimer.Stop();
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
            var indeces = pictureListViewEx.SelectedIndices;

            if (indeces.Count > 0)
            {
                for (int i = 0; i < indeces.Count; i++)
                {
                    for (int j = 0; j < Convert.ToInt32(copyCountNumeric.Value); j++)
                    {
                        ListViewItem.ListViewSubItem item = pictureListViewEx.Items.Insert(indeces[i] + 1, Convert.ToString(order)).
                            SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.Text = pictureListViewEx.Items[indeces[i]].SubItems[1].Text;
                    }
                }

                reorder();
                changeMaxLineThickness();

                pictureListViewEx.Focus();
                for (int i = 0; i < indeces.Count; i++)
                {
                    pictureListViewEx.Items[indeces[i]].Selected = true;
                }
            }
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

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        /// <summary>
        /// Po dokonceni drag and dropu se zkontroluji koncovky cest k souborum,
        /// do listu se pridaji pouze pokud maji platny format (jpg, bmp, tiff, png)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
           if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                int lastIndnex = pictureListViewEx.Items.Count;

                //MessageBox.Show(lastIndnex+"");
                foreach (string path in filePaths)
                {
                    bool valid = isExtensionValid(path);
                    if (valid)
                    {
                        pictureListViewEx.Items.Add(Convert.ToString(order)).SubItems.Add(path);
                        order++;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }                    
                }
                reorder();
                if (filePaths.Length > 0)
                {
                    Picture pic = new Picture(filePaths[0]);
                    try
                    {
                        pic.Ping();
                    }
                    catch
                    {
                        return;  //pri neuspesnem nacteni obrazku se v teto fazi pouze nenastavi komponenty formulare
                    }
                    setValuesFromPicture(pic);
                }
            }
        }

        private bool isExtensionValid(String path)
        {
            if (Path.GetExtension(path) == ".jpg" ||
                Path.GetExtension(path) == ".JPG" ||
                Path.GetExtension(path) == ".jpeg" ||
                Path.GetExtension(path) == ".JPEG" ||
                Path.GetExtension(path) == ".png" ||
                Path.GetExtension(path) == ".PNG" ||
                Path.GetExtension(path) == ".bmp" ||
                Path.GetExtension(path) == ".BMP" ||
                Path.GetExtension(path) == ".tiff" ||
                Path.GetExtension(path) == ".TIFF" ||
                Path.GetExtension(path) == ".tif" ||
                Path.GetExtension(path) == ".TIF")
            {
                return true;
            }               
            return false;
        }

        private void pictureListViewEx_DragEnter(object sender, DragEventArgs e)
        {
            
        }

        private void pictureListViewEx_DragDrop(object sender, DragEventArgs e)
        {
            
            //e.Effect = DragDropEffects.All;
            //reOrder();
            reorderTimer.Start();
        }

        private void copyCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            var indeces = pictureListViewEx.SelectedIndices;

            pictureListViewEx.Focus();
            for (int i = 0; i < indeces.Count; i++)
            {
                pictureListViewEx.Items[indeces[i]].Selected = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                settings.Save(settingsFilename);
            }
            catch { } //pri vyjimce se soubor proste neulozi
        }

        private void uložToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename;
            saveConfigDialog.Filter = "pix|*.pix";
            saveConfigDialog.AddExtension = true;
            if (saveConfigDialog.ShowDialog() == DialogResult.OK)
                filename = saveConfigDialog.FileName;
            else return;

            String[] split = filename.Split('.');
            int lastIndex = split.Length - 1;

            if (split[lastIndex].Equals("pix"))
                saveConfigDialog.AddExtension = false;
            else
                filename += ".pix";

            try
            {
                projectData.Save(filename);
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void načtiToolStripMenuItem_Click(object sender, EventArgs e)
        {

            String filename;
            openConfigDialog.Filter = "pix|*.pix";
            openConfigDialog.AddExtension = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
                filename = openConfigDialog.FileName;
            else return;

            try
            {
                projectData.Load(filename);
                projectData.GetInterlacingData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
                projectData.GetLineData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
                projectData.GetInterlacingData().SetResolutionUnits(((StringValuePair<Units>)settings.GetSelectedResolutionUnits()).value);
                updateAllComponents();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
