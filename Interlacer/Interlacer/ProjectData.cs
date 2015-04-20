using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GfxlibWrapper;
using System.Globalization;
using System.Threading;

namespace Interlacer
{
    public class ProjectData
    {
        private InterlacingData interlacingData = new InterlacingData();
        private LineData lineData = new LineData();

        public InterlacingData GetInterlacingData()
        {
            return interlacingData;
        }

        public LineData GetLineData()
        {
            return lineData;
        }

        public void Save(String filename)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");  //nastaveni kultury na ceskou, aby se hodnoty ukladaly vzdy s desetinou carkou
            try
            {
                File.WriteAllText(filename, InterlacingDataToString() + LineDataToString());
            }
            catch{
                Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
                throw new Exception("Soubor " + filename + " se nepodařilo uložit. Překontrolujte zda máte právo zápisu, nebo dostatek místa.");
            }
           
            Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
        }

        public void Load(String filename)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");  //nastaveni kultury na ceskou, aby se hodnoty nacetly vzdy spravne
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                String pom = File.ReadAllText(filename);

                string[] words = pom.Split('\n');
                foreach (string word in words)
                {
                    string[] line = word.Split(':');
                    if (line.Length > 1)
                        dictionary.Add(line[0], line[1].Trim());
                    else
                        dictionary.Add(line[0], "");
                }
            }
            catch
            {
                Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
                throw new Exception("Soubor " + filename + " se nepodařilo načíst.");
            }

            if (validateLoadDictionary(dictionary))
            {
                setLineData(dictionary);
                setInterlacingData(dictionary);
            }
            else
            {
                Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
                throw new Exception("Konfigurační soubor "+filename+" je pravděpodobně poškozen");
            }
            
            Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
        }

        private bool validateLoadDictionary(Dictionary<string, string> dictionary)
        {
            if (dictionary.Count != 19)
            {
                return false;
            }
            if (
                    !dictionary.ContainsKey("UNITS_INTERLACING")
                 || !dictionary.ContainsKey("RESOLUTION_UNITS")
                 || !dictionary.ContainsKey("WIDTH")
                 || !dictionary.ContainsKey("HEIGHT")
                 || !dictionary.ContainsKey("PICURE_RESOLUTION")
                 || !dictionary.ContainsKey("LENTICULE_DENSITY")
                 || !dictionary.ContainsKey("DIRECTION")
                 || !dictionary.ContainsKey("INITIAL_RESIZE_FILTER")
                 || !dictionary.ContainsKey("FINAL_RESAMPLE_FILTER")
                 || !dictionary.ContainsKey("UNITS_LINE")
                 || !dictionary.ContainsKey("LINE_COLOR")
                 || !dictionary.ContainsKey("BACKGROUND_COLOR")
                 || !dictionary.ContainsKey("FRAME_WIDTH")
                 || !dictionary.ContainsKey("INDENT")
                 || !dictionary.ContainsKey("LEFT")
                 || !dictionary.ContainsKey("TOP")
                 || !dictionary.ContainsKey("RIGHT")
                 || !dictionary.ContainsKey("BOTTOM")
                 || !dictionary.ContainsKey("CENTER_POSITION")
                )
                return false;
            if (!validateLoadUnits(dictionary))
                return false;
            if (!validateLoadFilters(dictionary))
                return false;
            if (!validateLoadNumbers(dictionary))
                return false;
            return true;
        }


        private static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private bool validateLoadNumbers(Dictionary<string, string> dictionary)
        {
            if (!IsNumeric(dictionary["WIDTH"])
                || !IsNumeric(dictionary["HEIGHT"])
                || !IsNumeric(dictionary["PICURE_RESOLUTION"])
                || !IsNumeric(dictionary["LENTICULE_DENSITY"])
                || !IsNumeric(dictionary["BACKGROUND_COLOR"])
                || !IsNumeric(dictionary["INDENT"])
                || !IsNumeric(dictionary["FRAME_WIDTH"])
                || !IsNumeric(dictionary["LINE_COLOR"]))
                return false;
            return true;
        }
        
        private bool validateLoadFilters(Dictionary<string, string> dictionary)
        {
            if (!dictionary["INITIAL_RESIZE_FILTER"].Equals("1")
                && !dictionary["INITIAL_RESIZE_FILTER"].Equals("3")
                && !dictionary["INITIAL_RESIZE_FILTER"].Equals("10")
                && !dictionary["INITIAL_RESIZE_FILTER"].Equals("22"))
                return false;
            if (!dictionary["FINAL_RESAMPLE_FILTER"].Equals("1")
                  && !dictionary["FINAL_RESAMPLE_FILTER"].Equals("3")
                  && !dictionary["FINAL_RESAMPLE_FILTER"].Equals("10")
                  && !dictionary["FINAL_RESAMPLE_FILTER"].Equals("22"))
                return false;
            return true;
        }

        private bool validateLoadUnits(Dictionary<string, string> dictionary)
        {
            if (!dictionary["UNITS_INTERLACING"].Equals("Mm")
                && !dictionary["UNITS_INTERLACING"].Equals("Cm")
                && !dictionary["UNITS_INTERLACING"].Equals("In"))
                return false;
            if (!dictionary["UNITS_LINE"].Equals("Mm")
                && !dictionary["UNITS_LINE"].Equals("Cm")
                && !dictionary["UNITS_LINE"].Equals("In"))
                return false;
            if (!dictionary["RESOLUTION_UNITS"].Equals("Mm")
                && !dictionary["RESOLUTION_UNITS"].Equals("Cm")
                && !dictionary["RESOLUTION_UNITS"].Equals("In"))
                return false;
            return true;
        }

        private void setLinePosition(Dictionary<string, string> dictionary)
        {
            if (dictionary["LEFT"].Equals("True"))
                this.lineData.SetLeft(true);
            else
                this.lineData.SetLeft(false);
            if (dictionary["TOP"].Equals("True"))
                this.lineData.SetTop(true);
            else
                this.lineData.SetTop(false);
            if (dictionary["RIGHT"].Equals("True"))
                this.lineData.SetRight(true);
            else
                this.lineData.SetRight(false);
            if (dictionary["BOTTOM"].Equals("True"))
                this.lineData.SetBottom(true);
            else
                this.lineData.SetBottom(false);
            if (dictionary["CENTER_POSITION"].Equals("True"))
                this.lineData.SetCenterPosition(true);
            else
                this.lineData.SetCenterPosition(false);
        }

        private void setLineData(Dictionary<string, string> dictionary)
        {
            switch (dictionary["UNITS_LINE"])
            {
                case "Mm":
                    this.lineData.SetUnits(Units.Mm);
                    break;
                case "Cm":
                    this.lineData.SetUnits(Units.Cm);
                    break;
                case "In":
                    this.lineData.SetUnits(Units.In);
                    break;
                default: break;
            }

            if (dictionary["FRAME_WIDTH"] != null)
                this.lineData.SetFrameWidth(Convert.ToDouble(dictionary["FRAME_WIDTH"]));
            if (dictionary["INDENT"] != null)
                this.lineData.SetIndent(Convert.ToDouble(dictionary["INDENT"]));

            if (dictionary["LINE_COLOR"] != null)
                this.lineData.SetLineColor(Color.FromArgb(Convert.ToInt32(dictionary["LINE_COLOR"])));
            if (dictionary["BACKGROUND_COLOR"] != null)
                this.lineData.SetBackgroundColor(Color.FromArgb(Convert.ToInt32(dictionary["BACKGROUND_COLOR"])));

            setLinePosition(dictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        private void setFilter(Dictionary<string, string> dictionary)
        {
            switch (dictionary["INITIAL_RESIZE_FILTER"])
            {
                case "3":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Triangle);
                    break;
                case "1":
                    this.interlacingData.SetInitialResizeFilter(FilterType.None);
                    break;
                case "22":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Lanczos);
                    break;
                case "10":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Cubic);
                    break;
                default: break;
            }

            switch (dictionary["FINAL_RESAMPLE_FILTER"])
            {
                case "3":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Triangle);
                    break;
                case "1":
                    this.interlacingData.SetFinalResampleFilter(FilterType.None);
                    break;
                case "22":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Lanczos);
                    break;
                case "10":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Cubic);
                    break;
                default: break;
            }
        }
        private void setInterlacingUnits(Dictionary<string, string> dictionary)
        {
            switch (dictionary["UNITS_INTERLACING"])
            {
                case "Mm":
                    this.interlacingData.SetUnits(Units.Mm);
                    break;
                case "Cm":
                    this.interlacingData.SetUnits(Units.Cm);
                    break;
                case "In":
                    this.interlacingData.SetUnits(Units.In);
                    break;
                default: break;
            }

            switch (dictionary["RESOLUTION_UNITS"])
            {
                case "Mm":
                    this.interlacingData.SetResolutionUnits(Units.Mm);
                    break;
                case "Cm":
                    this.interlacingData.SetResolutionUnits(Units.Cm);
                    break;
                case "In":
                    this.interlacingData.SetResolutionUnits(Units.In);
                    break;
                default: break;
            }
        }

        private void setInterlacingData(Dictionary<string, string> dictionary)
        {
            setInterlacingUnits(dictionary);
            setFilter(dictionary);
            
            bool keepRatio = false;
            if (this.interlacingData.getKeepAspectRatio())
                keepRatio = true;
            this.interlacingData.KeepAspectRatio(false);
            
            if (dictionary["WIDTH"] != null)
                this.interlacingData.SetWidth(Convert.ToDouble(dictionary["WIDTH"]));
            if (dictionary["HEIGHT"] != null)
                this.interlacingData.SetHeight(Convert.ToDouble(dictionary["HEIGHT"]));
            if (dictionary["PICURE_RESOLUTION"] != null)
                this.interlacingData.SetPictureResolution(Convert.ToDouble(dictionary["PICURE_RESOLUTION"]));
            if (dictionary["LENTICULE_DENSITY"] != null)
                this.interlacingData.SetLenticuleDensity(Convert.ToDouble(dictionary["LENTICULE_DENSITY"]));
            if (dictionary["LENTICULE_DENSITY"] != null)
                this.interlacingData.SetLenticuleDensity(Convert.ToDouble(dictionary["LENTICULE_DENSITY"]));
            if (dictionary["DIRECTION"].Equals("Vertical"))
                this.interlacingData.SetDirection(Direction.Vertical);
            else
                this.interlacingData.SetDirection(Direction.Horizontal);

            this.interlacingData.KeepAspectRatio(keepRatio);
        }

        private String LineDataToString()
        {
            return
                "UNITS_LINE:" + this.lineData.GetUnits() + Environment.NewLine +
                "LINE_COLOR:" + this.lineData.GetLineColor().ToArgb() + Environment.NewLine +
                "BACKGROUND_COLOR:" + this.lineData.GetBackgroundColor().ToArgb() + Environment.NewLine +
                "FRAME_WIDTH:" + this.lineData.GetFrameWidth() + Environment.NewLine +
                "INDENT:" + this.lineData.GetIndent() + Environment.NewLine +
                "LEFT:" + this.lineData.GetLeft() + Environment.NewLine +
                "TOP:" + this.lineData.GetTop() + Environment.NewLine +
                "RIGHT:" + this.lineData.GetRight() + Environment.NewLine +
                "BOTTOM:" + this.lineData.GetBottom() + Environment.NewLine +
                "CENTER_POSITION:" + this.lineData.GetCenterPosition();
        }

        private String InterlacingDataToString()
        {
            return
                "UNITS_INTERLACING:" + this.interlacingData.GetUnits() + Environment.NewLine +
                "RESOLUTION_UNITS:" + this.interlacingData.GetResolutionUnits() + Environment.NewLine +
                "WIDTH:" + this.interlacingData.GetWidth() + Environment.NewLine +
                "HEIGHT:" + this.interlacingData.GetHeight() + Environment.NewLine +
                "PICURE_RESOLUTION:" + this.interlacingData.GetPictureResolution() + Environment.NewLine +
                "LENTICULE_DENSITY:" + this.interlacingData.GetLenticuleDensity() + Environment.NewLine +
                "DIRECTION:" + this.interlacingData.GetDirection() + Environment.NewLine +
                "INITIAL_RESIZE_FILTER:" + this.interlacingData.GetInitialResizeFilter().filterNum + Environment.NewLine +
                "FINAL_RESAMPLE_FILTER:" + this.interlacingData.GetFinalResampleFilter().filterNum + Environment.NewLine;
        }
    }
}
