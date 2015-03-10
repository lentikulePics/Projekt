using GfxlibWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    class InterlacingData : IProcessData
    {
        private double width;
        private double height;
        private double pictureResolution;
        private double lenticuleDensity;
        private FilterType initialResizeFilter;
        private FilterType finalResampleFilter;
        private Units units = Units.In;

        public void SetUnits(Units units)
        {
            this.units = units;
        }

        public Units GetUnits()
        {
            return this.units;
        }

        public double GetWidth()
        {
            return UnitConverter.getUnitsFromIn(width, units);
        }

        public double GetHeight()
        {
            return UnitConverter.getUnitsFromIn(height, units);
        }

        public void SetWidth(double width)
        {
            this.width = UnitConverter.getInFromUnits(width, units);
        }

        public void SetHeight(double height)
        {
            this.height = UnitConverter.getInFromUnits(height, units);
        }
        
        public double GetPictureResolution()
        {
            return pictureResolution / UnitConverter.getUnitsFromIn(1, units);
        }

        public void SetPictureResolution(double resolution)
        {
            pictureResolution = resolution / UnitConverter.getInFromUnits(1, units);
        }

        public double GetLenticuleDensity()
        {
            return lenticuleDensity / UnitConverter.getUnitsFromIn(1, units);
        }

        public void SetLenticuleDensity(double density)
        {
            lenticuleDensity = density / UnitConverter.getInFromUnits(1, units);
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
    }
}
