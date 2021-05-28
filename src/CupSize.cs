using UnityEngine;

namespace LFE
{
    public class CupSize {
        public float Bust;
        public float Underbust;
        public int Band;
        public string Cup;
        public string Units;

        public int BustToCentimeters => Mathf.RoundToInt(Bust * 100);
        public int BustToInches => UnitUtils.UnityToInchesRounded(Bust);
        public int UnderbustToCentimeters => Mathf.RoundToInt(Underbust * 100);
        public int UnderbustToInches => UnitUtils.UnityToInchesRounded(Underbust);

        public override string ToString() {
            return $"{Band}{Cup}";
        }

        readonly static string[] _bustBandDiffToCupUs = new string[] {
            "AA", "A", "B", "C", "D", "DD", "DDD", "G", "H",
            "I",  "J", "K", "L", "M", "N",  "O",   "P", "Q",
            "R",  "S", "T", "U", "V", "W",  "X",   "Z",
            "ZZ", "ZZZ", "ZZZZ", "ZZZZZ", "ZZZZZZ"
        };
        public static string DifferenceToCupUS(int diff) {
            diff = Mathf.Max(0, diff);
            return diff >= _bustBandDiffToCupUs.Length ? "HUGE" : _bustBandDiffToCupUs[diff];
        }
    }
}