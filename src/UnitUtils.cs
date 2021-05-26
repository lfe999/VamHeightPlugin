using UnityEngine;

namespace LFE
{
    public class UnitUtils {
        public static float UnityToFeet(float unit) {
            return unit/0.3048f;
        }

        public static string FeetInchString(float feet) {
            int f = (int)feet;
            int inches = (int)((feet - f) * 12);
            return $"{f}'{inches}\"";
        }
    }
}