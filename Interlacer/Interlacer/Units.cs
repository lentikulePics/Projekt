using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    enum Units
    {
        In, Cm, Mm
    }

    static class UnitConverter
    {
        private const double INCH = 2.54;

        static public double getUnitsFromIn(double param, Units units)
        {
            switch (units)
            {
                case Units.Cm: return INCH * param;
                case Units.Mm: return INCH * 10 * param;
                default: return param;
            }
        }

        static public double getInFromUnits(double param, Units units)
        {
            switch (units)
            {
                case Units.Cm: return param / INCH;
                case Units.Mm: return param / (INCH * 10);
                default: return param;
            }
        }
    }
}
