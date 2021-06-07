using UnityEngine;

namespace LFE
{
    public class UnitUtils {
        public static float HeadHeightInUnity = 0;
        public const string US = "us";
        public const string Metric = "metric";
        public const string Meters = "m";
        public const string Centimeters = "cm";
        public const string Inches = "in";
        public const string Feet = "ft";
        public const string Heads = "h.u.";

        public static float ToUnit(float source, string targetUnit) {
            return ToUnit(source, Meters, targetUnit);
        }

        public static float ToUnit(float sourceValue, string sourceUnits, string targetUnits) {
            if(sourceUnits == targetUnits) {
                return sourceValue;
            }

            float unityValue = ConvertToUnity(sourceValue, sourceUnits);
            switch(targetUnits) {
                case Heads:
                    return HeadHeightInUnity == 0 ? 0 : unityValue / HeadHeightInUnity;
                case Meters:
                    return unityValue;
                case Metric:
                case Centimeters:
                    return unityValue * 100;
                case Feet:
                    return UnityToFeet(unityValue);
                case US:
                case Inches:
                    return UnityToInches(unityValue);
                default:
                    return unityValue;
            }
        }

        public static string ToUnitString(float source, string targetUnits) {
            return ToUnitString(source, Meters, targetUnits);
        }

        public static string ToUnitString(float sourceValue, string sourceUnits, string targetUnits) {
            switch(targetUnits) {
                case US:
                    return $"{ToUnit(sourceValue, sourceUnits, Inches):0.0} {Inches} / {FeetInchString(ToUnit(sourceValue, sourceUnits, Feet))}";
                case Metric:
                    return $"{ToUnit(sourceValue, sourceUnits, Centimeters):0.0} {Centimeters} / {ToUnit(sourceValue, sourceUnits, Meters):0.0} {Meters}";
                default:
                    return $"{ToUnit(sourceValue, sourceUnits, targetUnits):0.0} {targetUnits}";
            }
        }

        public static float ConvertToUnity(float value, string units) {
            // turn source into unity units
            switch(units) {
                case Meters:
                    return value;
                case Metric:
                case Centimeters:
                    return value / 100f;
                case Feet:
                    return value * 0.3048f;
                case US:
                case Inches:
                    return value / 12 * 0.3048f;
                case Heads:
                    return value * HeadHeightInUnity;
                default:
                    return value;
            }


        }


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
            int inches = RoundToInt((feet - f) * 12);
            return $"{f}'{inches}\"";
        }

        public static string MetersToFeetString(float meters) {
            return FeetInchString(UnityToFeet(meters));
        }
    }
}