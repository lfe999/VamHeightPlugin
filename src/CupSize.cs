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
        public int BustToInches => Mathf.RoundToInt(UnitUtils.UnityToFeet(Bust) * 12);
        public int UnderbustToCentimeters => Mathf.RoundToInt(Underbust * 100);
        public int UnderbustToInches => Mathf.RoundToInt(UnitUtils.UnityToFeet(Underbust) * 12);

        public override string ToString() {
            return $"{Band}{Cup}";
        }
    }
}