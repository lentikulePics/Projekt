using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    class OutputPictureData
    {
        public readonly double inchWidth;
        public readonly double inchHeight;
        public readonly double dpi;
        public readonly double lpi;

        public OutputPictureData(double inchWidth, double inchHeight, double dpi, double lpi)
        {
            this.inchWidth = inchWidth;
            this.inchHeight = inchHeight;
            this.dpi = dpi;
            this.lpi = lpi;
        }
    }
}
