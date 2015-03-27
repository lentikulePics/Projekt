using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interlacer
{
    public class PreviewData
    {
        private Dictionary<String, Image> images = new Dictionary<String, Image>();
        private PictureBox pictureBox;
        private Image defaultImage;

        public PreviewData(PictureBox pictureBox, Image defaultImage)
        {
            this.pictureBox = pictureBox;
            this.defaultImage = defaultImage;
        }

        public void Show(String path)
        {
            if (images.ContainsKey(path))
            {
                pictureBox.Image = images[path];
            }
            else
            {
                int baseSize = 300;
                Image image = Image.FromFile(path);
                double ratio = image.Width / (double)image.Height;
                int w = baseSize;
                int h = (int)(baseSize / ratio);
                Bitmap bmp = new Bitmap(image, new Size(w, h));
                images.Add(path, bmp);
                pictureBox.Image = bmp;
            }
        }

        public void ShowDefaultImage()
        {
            pictureBox.Image = defaultImage;
        }
    }
}
