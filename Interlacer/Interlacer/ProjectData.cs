using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GfxlibWrapper;

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
            File.WriteAllText(filename, InterlacingDataToString() + LineDataToString());
        }

        public void Load(String filename)
        {
            String pom = File.ReadAllText(filename);
            MessageBox.Show(pom);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] words = pom.Split('\n');
            foreach (string word in words)
            {
                string[] line = word.Split(':');
                if (line.Length > 1)
                    dictionary.Add(line[0], line[1].Trim());
                else
                    dictionary.Add(line[0], "");
            }

            setLineData(dictionary);
            setInterlacingData(dictionary);
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

            if (dictionary["LINE_THICKNESS"] != null)
                this.lineData.SetLineThickness(Convert.ToInt32(dictionary["LINE_THICKNESS"]));
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
        private void setFilter(Dictionary<string, string> dictionary)
        {

        }
        private void setInterlacingData(Dictionary<string, string> dictionary)
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

            switch (dictionary["INITIAL_RESIZE_FILTER"])
            {
                case "Triangle":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Triangle);
                    break;
                case "None":
                    this.interlacingData.SetInitialResizeFilter(FilterType.None);
                    break;
                case "Lanczos":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Lanczos);
                    break;
                case "Cubic":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Cubic);
                    break;
                default: break;
            }

            switch (dictionary["FINAL_RESAMPLE_FILTER"])
            {
                case "Triangle":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Triangle);
                    break;
                case "None":
                    this.interlacingData.SetFinalResampleFilter(FilterType.None);
                    break;
                case "Lanczos":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Lanczos);
                    break;
                case "Cubic":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Cubic);
                    break;
                default: break;
            }

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
        }
        private String LineDataToString()
        {
            return
                "UNITS_LINE:" + this.lineData.GetUnits() + Environment.NewLine +
                "LINE_THICKNESS:" + this.lineData.GetLineThickness() + Environment.NewLine +
                "LINE_COLOR:" + this.lineData.GetLineColor().ToArgb() + Environment.NewLine +
                "BACKGROUND_COLOR:" + this.lineData.GetBackgroundColor().ToArgb() + Environment.NewLine +
                "FRAME_WIDTH:" + this.lineData.GetFrameWidth() + Environment.NewLine +
                "INDENT:" + this.lineData.GetIndent() + Environment.NewLine +
                "LEFT:" + this.lineData.GetLeft() + Environment.NewLine +
                "TOP:" + this.lineData.GetTop() + Environment.NewLine +
                "RIGHT:" + this.lineData.GetRight() + Environment.NewLine +
                "BOTTOM:" + this.lineData.GetBottom() + Environment.NewLine +
                "CENTER_POSITION:" + this.lineData.GetCenterPosition() + Environment.NewLine;
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
                "INITIAL_RESIZE_FILTER:" + this.interlacingData.GetInitialResizeFilter() + Environment.NewLine +
                "FINAL_RESAMPLE_FILTER:" + this.interlacingData.GetFinalResampleFilter() + Environment.NewLine;

        }
    }
}
