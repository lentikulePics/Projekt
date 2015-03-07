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
            Environment.SetEnvironmentVariable("PATH", "magick");
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*Picture pic = new Picture(10, 10);
            for (int i = 0; i < pic.GetWidth(); i++)
            {
                for (int j = 0; j < pic.GetHeight(); j++)
                {
                    pic.SetPixel(i, j, 127, 0, 255);
                }
            }
            pic.Save("pic.tif");
            pic.Destroy();*/

            Picture pic = new Picture("picture.jpg");
            pic.Load();
            pic.Resize(300, 300, FilterType.None);
            pic.SetDpi(601.183, 601.183);
            pic.Save("pic.tif");
            pic.Destroy();
        }
    }
}
