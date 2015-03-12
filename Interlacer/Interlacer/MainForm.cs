using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GfxlibWrapper;
using System.Runtime.InteropServices;

namespace Interlacer
{
    /// <summary>
    /// Hlavni formular programu
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// konstruktor pro inicializaci formulare
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Picture> pics = new List<Picture>
            {
                new Picture("pics/s01.jpg"),
                new Picture("pics/s02.jpg"),
                new Picture("pics/s03.jpg"),
                new Picture("pics/s04.jpg"),
                new Picture("pics/s05.jpg"),
                new Picture("pics/s06.jpg"),
                new Picture("pics/s07.jpg"),
                new Picture("pics/s08.jpg"),
                new Picture("pics/s09.jpg"),
                new Picture("pics/s10.jpg"),
                new Picture("pics/s11.jpg"),
                new Picture("pics/s12.jpg"),
                new Picture("pics/s13.jpg"),
                new Picture("pics/s14.jpg"),
                new Picture("pics/s15.jpg"),
            };
            InterlacingData intData = new InterlacingData();
            intData.SetUnits(Units.Mm);
            intData.SetWidth(210);
            intData.SetHeight(297);
            intData.SetUnits(Units.In);
            intData.SetPictureResolution(400);
            intData.SetLenticuleDensity(40);
            intData.SetInitialResizeFilter(FilterType.None);
            intData.SetFinalResampleFilter(FilterType.Triangle);
             
            LineData linedata = new LineData();
            linedata.SetLeft(false);
            linedata.SetTop(true);
            linedata.SetBottom(true);
            linedata.SetRight(true);
            linedata.SetLineThickness(1);
            intData.SetUnits(Units.Cm);
            linedata.SetIndent(0.2);
            linedata.SetFrameWidth(1);
            linedata.SetCenterPosition(false);
            linedata.SetBackgroundColor(Color.White);
            linedata.SetLineColor(Color.Black);

            PictureContainer picCon = new PictureContainer(pics, intData, linedata);
            if (picCon.CheckPictures())
                MessageBox.Show("Pictures are OK");
            else
                MessageBox.Show("Pictures will be clipped");
            picCon.Interlace();
            MessageBox.Show("Done!");
            picCon.GetResult().SetDpi(600, 600);
            picCon.GetResult().Save("result.tif");
            picCon.GetResult().Destroy();
        }
    }
}
