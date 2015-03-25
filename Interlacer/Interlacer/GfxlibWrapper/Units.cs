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
        static public double GetUnitsFromIn(double param, Units units)
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
        static public double GetInFromUnits(double param, Units units)
        {
            switch (units)
            {
                case Units.Cm: return param / INCH;
                case Units.Mm: return param / (INCH * 10);
                default: return param;
            }
        }

        /// <summary>
        /// prevede centimetry na palce
        /// </summary>
        /// <param name="param">hodnota v cm</param>
        /// <returns>hodnota v palcich</returns>
        private static double cmToIn(double param)
        {
            return param / INCH;
        }

        /// <summary>
        /// prevede centimetry na milimetry
        /// </summary>
        /// <param name="param">hodnota v cm</param>
        /// <returns>hodnota v mm</returns>
        private static double cmToMm(double param)
        {
            return param * 10;
        }

        /// <summary>
        /// prevede palce na centimetry
        /// </summary>
        /// <param name="param">hodnota v palcich</param>
        /// <returns>hodnota v cm</returns>
        private static double inToCm(double param)
        {
            return param * INCH;
        }
        
        /// <summary>
        /// prevede palce na milimetry
        /// </summary>
        /// <param name="param">hodnota v palcich</param>
        /// <returns>hodnota v mm</returns>
        private static double inToMm(double param)
        {
            return param * INCH * 10;
        }

        /// <summary>
        /// prevede milimetry na palce
        /// </summary>
        /// <param name="param">hodnota v mm</param>
        /// <returns>hodnota v palcich</returns>
        private static double mmToIn(double param)
        {
            return param / INCH / 10;
        }

        /// <summary>
        /// prevede milimetry na centimetry
        /// </summary>
        /// <param name="param">hodnota v milimetrech</param>
        /// <returns>hodnota v centimetrech</returns>
        private static double mmToCm(double param)
        {
            return param / 10;
        }

        /// <summary>
        /// prevede hodnotu v konkretnich jednotkach na jine jednotky
        /// </summary>
        /// <param name="param">hodnota k prevedeni</param>
        /// <param name="sourceUnits">jednotky pred prevodem</param>
        /// <param name="targetUnits">jednotky, na ktere ma byt hodnota prevedena</param>
        /// <returns>prevedena hodnota</returns>
        public static double Transfer(double param, Units sourceUnits, Units targetUnits)
        {
            if (sourceUnits == Units.In)
            {
                switch (targetUnits)
                {
                    case Units.Cm: return inToCm(param);
                    case Units.Mm: return inToMm(param);
                    default: return param;
                }
            }
            else if (sourceUnits == Units.Cm)
            {
                switch (targetUnits)
                {
                    case Units.Mm: return cmToMm(param);
                    case Units.In: return cmToIn(param);
                    default: return param;
                }
            }
            else
            {
                switch (targetUnits)
                {
                    case Units.Cm: return mmToCm(param);
                    case Units.In: return mmToIn(param);
                    default: return param;
                }
            }
        }
    }
}
