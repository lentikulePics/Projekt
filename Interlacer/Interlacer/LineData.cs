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
        /// <summary>
        ///  šírka čáry na lentikuli kolik je obrázků
        /// </summary>
        private int lineThickness;
        
        private Color lineColor;

        private Color backgroundColor;

        /// <summary>
        ///  šířka rámečku čár
        /// </summary>
        private double frameWidth;

        private double indent;

        private bool left, top, right, bottom;

        private bool centerPosition;

        private Units units = Units.In;

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
            return UnitConverter.getUnitsFromIn(this.frameWidth, units);
        }

        public void SetFrameWidth(double frameWidth)
        {
            this.frameWidth = UnitConverter.getInFromUnits(frameWidth, units);
        }

        public double GetIndent()
        {
            return UnitConverter.getUnitsFromIn(this.indent, units);
        }

        public void SetIndent(double indent)
        {
            this.indent = UnitConverter.getInFromUnits(indent, units);
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
