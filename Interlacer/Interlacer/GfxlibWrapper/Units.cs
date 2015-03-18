using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    /// <summary>
    /// reprezentuje jednotky
    /// </summary>
    public enum Units
    {
        /// <summary>
        /// palce
        /// </summary>
        In,
        /// <summary>
        /// centimetry
        /// </summary>
        Cm,
        /// <summary>
        /// milimetry
        /// </summary>
        Mm
    }

    /// <summary>
    /// staticka trida, ktera provadi prevod z palcu na jine jednoty a obracene
    /// </summary>
    public static class UnitConverter
    {
        private const double INCH = 2.54;

        /// <summary>
        /// prevede hodnotu v palcich na hodnotu o zadanych jednotkach
        /// </summary>
        /// <param name="param">hodnota v palcich</param>
        /// <param name="units">jednotky, do kterych ma byt hodnota prevedena</param>
        /// <returns>prevedena hodnota</returns>
        static public double getUnitsFromIn(double param, Units units)
        {
            switch (units)
            {
                case Units.Cm: return INCH * param;
                case Units.Mm: return INCH * 10 * param;
                default: return param;
            }
        }

        /// <summary>
        /// prevede hodnotu v zadanych jednotkach na hodnotu v palcich
        /// </summary>
        /// <param name="param">hodnota v zadanych jednotkach</param>
        /// <param name="units">jednotky, ve kterych je zadana hodnota</param>
        /// <returns>prevedena hodnota</returns>
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
