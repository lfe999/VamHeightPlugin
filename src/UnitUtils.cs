using UnityEngine;

namespace LFE
{
    public class UnitUtils {

        public static int RoundToInt(float value) {
            // NOTE: unity rounding rounds down at 0.5, we want to round up
            // use microsofts rounding which rounds up at 0.5
            return (int)System.Math.Round(value, 0, System.MidpointRounding.AwayFromZero);
        }

        public static float UnityToFeet(float unit) {
            return unit/0.3048f;
        }

        public static float UnityToInches(float unit) {
            return UnityToFeet(unit) * 12;
        }

        public static float InchesToUnity(float inches) {
            return inches * 0.0254f;
        }

        public static int UnityToInchesRounded(float unit) {
            // NOTE: unity rounding rounds down at 0.5, we want to round up
            var inches = UnityToInches(unit);
            return RoundToInt(inches);
        }

        public static string FeetInchString(float feet) {
            int f = (int)feet;
            int inches = (int)((feet - f) * 12);
            return $"{f}'{inches}\"";
        }

        public static string MetersToFeetString(float meters) {
            return FeetInchString(UnityToFeet(meters));
        }
    }
}