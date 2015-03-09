using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Interlacer
{
    class LineData : IProcessData
    {
        // šírka čáry na lentikuli kolik je obrázků
        private int lineThickness;
        
        private Color lineColor;

        private Color backgroundColor;

        // šířka rámečku čár
        private double frameWidth;

        private double indent;

        private bool left, top, right, bottom;

        private bool centerPosition;

        private Units units;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="units"></param>
        public void SetUnits(Units units)
        {
            this.units = units;
        }

        public Units GetUnits()
        {
            return this.units;
        }

        public int GetLineThickness()
        {
            return lineThickness;
        }

        public void SetLineThickness(int lineThickness)
        {
            this.lineThickness = lineThickness;
        }

        public Color GetLineColor()
        {
            return this.lineColor;
        }

        public void SetLineColor(Color lineColor)
        {
            this.lineColor = lineColor;
        }

        public Color GetBackgroundColor()
        {
            return this.backgroundColor;
        }

        public void SetBackgroundColor(Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        public double GetFrameWidth()
        {
            return this.frameWidth;
        }

        public void SetFrameWidth(double frameWidth)
        {
            this.frameWidth = frameWidth;
        }

        public double GetIndent()
        {
            return this.indent;
        }

        public void SetIndent(double indent)
        {
            this.indent = indent;
        }

        //left, top, right, bottom;
        public bool GetLeft()
        {
            return this.left;
        }

        public void SetLeft(bool left)
        {
            this.left = left;
        }

        public bool GetTop()
        {
            return this.top;
        }

        public void SetTop(bool top)
        {
            this.top = top;
        }

        public bool GetRight()
        {
            return this.right;
        }

        public void SetRight(bool right)
        {
            this.right = right;
        }
        public bool GetBottom()
        {
            return this.bottom;
        }

        public void SetBottom(bool bottom)
        {
            this.bottom = bottom;
        }

        public bool GetCenterPosition()
        {
            return this.centerPosition;
        }

        public void SetCenterPosition(bool centerPosition)
        {
            this.centerPosition = centerPosition;
        }

    }
}
