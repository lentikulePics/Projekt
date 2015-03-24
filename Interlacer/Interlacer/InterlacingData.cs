using GfxlibWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    public class InterlacingData : IProcessData
    {
        private double width;
        private double height;
        private double pictureResolution;
        private double lenticuleDensity;
        private FilterType initialResizeFilter;
        private FilterType finalResampleFilter;
        private bool keepAspectRatio = false;
        private Units units = Units.In;
        private Units resolutionUnits = Units.In;
        private Direction direction = Direction.Vertical;

        public void SetUnits(Units units)
        {
            this.units = units;
        }

        public Units GetUnits()
        {
            return units;
        }

        public void SetResolutionUnits(Units units)
        {
            this.resolutionUnits = units;
        }

        public Units GetResolutionUnits()
        {
            return resolutionUnits;
        }

        public double GetInchWidth()
        {
            return UnitConverter.getInFromUnits(width, units);
        }

        public double GetInchHeight()
        {
            return UnitConverter.getInFromUnits(height, units);
        }

        public void SetWidth(double width)
        {
            if (keepAspectRatio && this.width > 0.00001)
            {
                double newWidth = width;
                double ratio = newWidth / this.width;
                this.width = newWidth;
                height *= ratio;
            }
            else
                this.width = width;
        }

        public double GetWidth()
        {
            return width;
        }

        public void SetHeight(double height)
        {
            if (keepAspectRatio && this.height > 0.00001)
            {
                double newHeight = height;
                double ratio = newHeight / this.height;
                this.height = newHeight;
                width *= ratio;
            }
            else
                this.height = height;
        }

        public double GetHeight()
        {
            return height;
        }
        
        public double GetDPI()
        {
            return pictureResolution / UnitConverter.getInFromUnits(1, resolutionUnits);
        }

        public void SetPictureResolution(double resolution)
        {
            pictureResolution = resolution;
        }

        public double GetPictureResolution()
        {
            return pictureResolution;
        }

        public double GetLPI()
        {
            return lenticuleDensity / UnitConverter.getInFromUnits(1, resolutionUnits);
        }

        public void SetLenticuleDensity(double density)
        {
            lenticuleDensity = density;
        }

        public double GetLenticuleDensity()
        {
            return lenticuleDensity;
        }
        
        public FilterType GetInitialResizeFilter()
        {
            return initialResizeFilter;
        }

        public void SetInitialResizeFilter(FilterType filter)
        {
            initialResizeFilter = filter;
        }

        public FilterType GetFinalResampleFilter()
        {
            return finalResampleFilter;
        }

        public void SetFinalResampleFilter(FilterType filter)
        {
            finalResampleFilter = filter;
        }

        public void KeepAspectRatio(bool keepAspectRatio)
        {
            this.keepAspectRatio = keepAspectRatio;
        }

        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }

        public Direction GetDirection()
        {
            return direction;
        }
    }
}
