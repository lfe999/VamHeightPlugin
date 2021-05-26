using UnityEngine;

namespace LFE
{
    public class UnitUtils {
        public static float UnityToFeet(float unit) {
            return unit/0.3048f;
        }

        public static float UnityToInches(float unit) {
            return UnityToFeet(unit) * 12;
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