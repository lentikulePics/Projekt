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
            Picture pic = new Picture(2000, 2413);
            for (int i = 0; i < pic.GetWidth(); i++)
            {
                for (int j = 0; j < pic.GetHeight(); j++)
                {
                    pic.SetPixel(i, j, 0, 0, 0);
                }
            }
            List<Picture> pics = new List<Picture>
            {
                new Picture("pics/red.jpg"),
                new Picture("pics/orange.jpg"),
                new Picture("pics/purple.jpg"),
                pic,
                new Picture("pics/blue.jpg"),
                new Picture("pics/green.jpg"),
            };
            InterlacingData intData = new InterlacingData();
            intData.SetUnits(Units.Mm);
            intData.SetWidth(210);
            intData.SetHeight(297);
            intData.SetUnits(Units.In);
            intData.SetPictureResolution(300);
            intData.SetLenticuleDensity(40);
            intData.SetInitialResizeFilter(FilterType.Lanczos);
            intData.SetFinalResampleFilter(FilterType.Triangle);
            PictureContainer picCon = new PictureContainer(pics, intData, null);
            picCon.Interlace();
            pic.Destroy();
            MessageBox.Show("Done!");
            picCon.GetResult().SetDpi(601.183, 601.183);
            picCon.GetResult().Save("result.tif");
            picCon.GetResult().Destroy();
        }
    }
}
