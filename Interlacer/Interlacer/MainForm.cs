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
        private const String stringOfInputExtensions = "Image Files (*.jpeg, *.jpg, *.png, *.bmp, *.tif)|*.jpeg;*.jpg;*.png;*.bmp;*.tif";
        private const String stringOfOutputExenstions = "JPEG|*.jpg;*.jpeg|PNG|*.png|BMP|*.bmp|TIF|*.tif";
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
                MessageBox.Show(Localization.resourcesStrings.GetString("libraryLoadError"));
                System.Environment.Exit(0);
            }
            InitializeComponent();
            projectData.GetInterlacingData().KeepAspectRatio(keepRatioCheckbox.Checked);
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
            changeLanguage();
            changeUnits();
            pictureListViewEx.MultiSelect = true;
            interpol1ComboBox.SelectedIndex = 0;
            interpol2ComboBox.SelectedIndex = 0;
            resetPictureInfo();
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

        private bool isExtensionValid(String path)
        {
            String pathExt = path.ToLower();
            if (Path.GetExtension(pathExt) == ".jpg" ||
                Path.GetExtension(pathExt) == ".jpeg" ||
                Path.GetExtension(pathExt) == ".png" ||
                Path.GetExtension(pathExt) == ".bmp" ||
                Path.GetExtension(pathExt) == ".tiff" ||
                Path.GetExtension(pathExt) == ".tif")
            {
                return true;
            }
            return false;
        }

        private void tryLoadPictures()
        {
            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                Picture pic = new Picture(pictureListViewEx.Items[i].SubItems[1].Text);
                try
                {
                    pic.Ping();
                }
                catch
                {
                    pictureListViewEx.Items[i].SubItems[3].Text = "X";
                }
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

        private string getPicName(string path)
        {
            string[] splitName = path.Split('\\');
            return splitName[splitName.Length - 1];
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
            t.SetToolTip(addPicButton, Localization.resourcesStrings.GetString("addPicTooltip"));
            t.SetToolTip(removePicButton, Localization.resourcesStrings.GetString("removePicTooltip"));
            t.SetToolTip(copyPicButton, Localization.resourcesStrings.GetString("copyPicTooltip"));
            t.SetToolTip(moveUpButton, Localization.resourcesStrings.GetString("moveUpTooltip"));
            t.SetToolTip(moveDownButton, Localization.resourcesStrings.GetString("moveDownTooltip"));
            t.SetToolTip(clearAllButton, Localization.resourcesStrings.GetString("clearAllTooltip"));
            t.SetToolTip(reverseButton, Localization.resourcesStrings.GetString("revertTooltip"));
            t.SetToolTip(sortButton, Localization.resourcesStrings.GetString("sortTooltip"));
            t.SetToolTip(replaceButton, Localization.resourcesStrings.GetString("replaceTooltip"));
            saveToolStripButton.Text = Localization.resourcesStrings.GetString("saveTooltip");
            loadToolStripButton.Text = Localization.resourcesStrings.GetString("loadTooltip");
            // Nastaveni sloupcu listview
            pictureListViewEx.Columns[0].Text = Localization.resourcesStrings.GetString("orderListView");
            pictureListViewEx.Columns[1].Text = Localization.resourcesStrings.GetString("pathListView");
            pictureListViewEx.Columns[2].Text = Localization.resourcesStrings.GetString("nameListView");
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

                    infoFilenameLabel.Text = getPicName(path);
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

        /// <summary>
        /// pokusi se nastavit sirku a vysku ze seznamu obrazku
        /// pokud se to nepodari, hodnoty zustanou nezmenene
        /// </summary>
        /// <param name="pictures">pole retezcu cest k obrazkum</param>
        private void trySetValuesFromPictures(String[] pictures)
        {
            if (pictures.Length > 0)
            {
                // pouzito vickrat, dat do vlastni metody!
                Picture pic = new Picture(pictures[0]);
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
        
        /// <summary>
        /// Podle počtu obrázků v listu nastaví maximální šířku vodících čar.
        /// </summary>
        private void changeMaxLineThickness()
        {
            lineThicknessTrackbar.Maximum = Math.Max(1, pictureListViewEx.Items.Count - 1);
            maxPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Maximum);
        }      

        /// <summary>
        /// Ze seznamu obrázků vybere jejich cesty a naplní jimi List.
        /// </summary>
        /// <returns>picList list naplnění cestami k obrázkům.</returns>
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

        /// <summary>
        /// Metoda projde všechny položky seznamu od prvního po poslední a přiřadí jim číslo, které značí jejich pořadí v seznamu.
        /// </summary>
        private void reorder()
        {
            order = 1;

            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                pictureListViewEx.Items[i].SubItems[0].Text = Convert.ToString(order);
                order += 1;
            }
        }
        
        /// <summary>
        /// Pomocí hodnot ze tříd LineData a InterlacingData se nastaví komponentám příslušné hodnoty.
        /// Dále se tu upravuje maximální hodnota trackbaru pro šířky čar a vypočítává se počet obrázků pod lentikuli.
        /// </summary>
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
            // Pokud se ze seznamu odebrali obrázky a šířka čar byla větší než je teď maximální hodnota, 
            // nová hodnota se nastaví na maximální hodnotu trackbaru
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
        }
                
        /// <summary>
        /// Prenastavi jednotky ze settings
        /// </summary>
        private void changeUnits()
        {
            projectData.GetInterlacingData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
            projectData.GetLineData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
            projectData.GetInterlacingData().SetResolutionUnits(((StringValuePair<Units>)settings.GetSelectedResolutionUnits()).value);
            string measureUnits = settings.GetSelectedUnits().ToString();
            string[] resolutionUnits = settings.GetSelectedResolutionUnits().ToString().Split(new char[] { ',', ' ' });
            unitsLabel.Text = measureUnits;
            unitsLabel2.Text = measureUnits;
            unitsLabel3.Text = measureUnits;
            unitsLabel4.Text = measureUnits;
            dpiLabel.Text = resolutionUnits[0];
            lpiLabel.Text = resolutionUnits[2];
        }

        /// <summary>
        /// 
        /// </summary>
        private void changeLanguage()
        {
            // Updatuje texty v komboboxech
            Localization.iterateOverControls(this, Localization.resourcesMain);
            updateTexts();
            Invalidate();
        }

        public void ApplySettings()
        {
            changeLanguage();
            changeUnits();
            updateAllComponents();
            resetPictureInfo();
        }

        private List<String> getListFromPictureView()
        {
            List<String> list = new List<String>();
            for (int i = 0; i < pictureListViewEx.Items.Count; i++)
            {
                list.Add(pictureListViewEx.Items[i].SubItems[1].Text);
            }
            return list;
        }

        
        private void setPictureViewFromList(List<String> pathPics)
        {
            for (int i = pictureListViewEx.Items.Count - 1; i >= 0; i--)
            {
                    pictureListViewEx.Items[i].Remove();
            }
            if(pathPics.Count > 0)
            {
                for (int i = 0; i < pathPics.Count; i++)
                {
                    ListViewItem item = new ListViewItem(new[] { Convert.ToString(order), pathPics[i], getPicName(pathPics[i]), "" });
                    pictureListViewEx.Items.Add(item);
                    reorder();
                }
                tryLoadPictures();
            }
        }

        /// <summary>
        /// Metoda pro seřazení položek seznamu podle posledního čísla v jejich názvu.
        /// </summary>
        private void sortListView()
        {
            for (int i = 0; i < pictureListViewEx.Items.Count - 1; i++)
            {
                for(int j = 0; j < pictureListViewEx.Items.Count - 1; j++) 
                {
                    int firstValue = getIntegerFromString(pictureListViewEx.Items[j + 1].SubItems[2].Text);
                    int secondValue = getIntegerFromString(pictureListViewEx.Items[j].SubItems[2].Text);                    
                    if(firstValue < secondValue)
                    {
                        string tmp;
                        for (int k = 0; k < pictureListViewEx.Items[0].SubItems.Count; k++)
                        {
                            tmp = pictureListViewEx.Items[j + 1].SubItems[k].Text;
                            pictureListViewEx.Items[j + 1].SubItems[k].Text = pictureListViewEx.Items[j].SubItems[k].Text;
                            pictureListViewEx.Items[j].SubItems[k].Text = tmp;
                        }
                    }
                }
            }
            reorder();
        }

        /// <summary>
        /// Zjisti posledni cislo z nazvu souboru
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private int getIntegerFromString(String word)
        {
            int start = word.Length;
            int end = 0;
            for (int i = word.Length - 1; i >= 0; i--)
            {
                if (word[i] >= '0' && word[i] <= '9')
                {
                    if (end == 0)
                    {
                        end = i + 1;
                    }
                    start = i;
                }
                else
                {
                    if (end != 0)
                    {
                        break;
                    }
                }
            }
            if (end != 0)
            {
                return Convert.ToInt32(word.Substring(start, end - start));
            }
            return -1;
        }

        /// <summary>
        /// Metoda pro nahrazení jednoho vybraného obrázku jiným.
        /// Funguje pouze pokud je vybrán jeden obrázek.
        /// Po otevření file dialogu je vyplí multi select takže se smí vybrat pouze jeden orbázek.
        /// </summary>
        private void replacePicture()
        {
            if (pictureListViewEx.SelectedItems.Count > 1 || pictureListViewEx.SelectedItems.Count == 0)
            {
                var indeces = pictureListViewEx.SelectedIndices;
                pictureListViewEx.Focus();
                for (int i = 0; i < indeces.Count; i++)
                {
                    pictureListViewEx.Items[indeces[i]].Selected = true;
                }

                return;
            }
            addPicFileDialog.Multiselect = false;
            addPicFileDialog.Filter = stringOfInputExtensions;
            addPicFileDialog.FilterIndex = 1;
            DialogResult result = addPicFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string chosenPicture = addPicFileDialog.FileName;
                pictureListViewEx.SelectedItems[0].SubItems[1].Text = chosenPicture;
                pictureListViewEx.SelectedItems[0].SubItems[2].Text = getPicName(chosenPicture);
                pictureListViewEx.SelectedItems[0].SubItems[3].Text = "";
                // Vrácení focus na nově vloženou položku
                pictureListViewEx.Focus();
                pictureListViewEx.SelectedItems[0].Selected = true;
                reorder();
            }
        }

        /// <summary>
        /// nastavi focus na listView a zvyrazni aktualne vybrany radek
        /// </summary>
        /// <param name="indeces">kolekce indexu vybranych radku</param>
        /// <returns>vraci true, pokud v listu neco je, jinak false</returns>
        bool selectCurrentListItem(ListView.SelectedIndexCollection indeces)
        {
            if (indeces.Count == 0)
            {
                return false;
            }
            int selectedIndex = Convert.ToInt32(indeces[0]);
            pictureListViewEx.Focus();
            pictureListViewEx.Items[selectedIndex].Selected = true;
            return true;
        }
    }
}
