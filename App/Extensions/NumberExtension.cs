using System;

namespace KiTPO.Extensions
{
    public static class NumberExtensions
    {
        public const double Min = -30;
        public const double Max = 30;

        public static bool IsInRange(this double v) => v >= Min && v <= Max;
        
        public static double RandomInRange(double minNumber, double maxNumber)
        {
            return new Random().NextDouble() * (maxNumber - minNumber) + minNumber;
        }
    }
}