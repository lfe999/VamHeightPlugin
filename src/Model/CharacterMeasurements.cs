using UnityEngine;

namespace LFE {

    public class CharacterMeasurements {
        public float? Height { get; set; }
        public float? ChinHeight { get; set; }
        public float? ShoulderHeight { get; set; }
        public float? NippleHeight { get; set; }
        public float? UnderbustHeight { get; set; }
        public float? NavelHeight { get; set; }
        public float? CrotchHeight { get; set; }
        public float? KneeHeight { get; set; }
        public float? HeelHeight { get; set; }

        public float? BustSize { get; set; }
        public float? UnderbustSize { get; set; }
        public string CupSize { get; set; }
        public int? BandSize { get; set; }

        public float? WaistSize { get; set; }
        public float? HipSize { get; set; }

        public float? PenisLength { get; set; }
        public float? PenisWidth { get; set; }
        public float? PenisGirth { get; set; }

        public float? HeadHeight => Height == null || ChinHeight == null ? null : Height - ChinHeight;
        public float? HeadWidth { get; set; }

        public float? NoseHeight { get; set; }
        public float? MouthOffsetLeftRight { get; set; } = 0;
        public float? MouthHeight { get; set; }
        public float? MouthWidth { get; set; }
        public float? EyesHeight { get; set; }
        public float? EyesOffsetLeftRight { get; set; } = 0;
        public float? EyesWidth { get; set; }
        public Vector3 HeelToFloorOffset { get; set; } = Vector3.zero;
        public CharacterPointsOfInterest POI { get; set; } = null;

        public AgeCalculation AgeFromHead { get; set; }
        public AgeCalculation AgeFromHeight { get; set; }

        public Proportions Proportions {
            get {
                var p = new Proportions() {
                    ProportionName = "Current",
                };
                if((HeadHeight ?? 0) == 0) {
                    return p;
                }
                var hh = HeadHeight ?? 0;
                p.FigureHeightInHeads = ((Height ?? 0) - (HeelHeight ?? 0)) / hh;
                p.FigureChinToShoulder = Mathf.Abs((ChinHeight?? 0) - (ShoulderHeight?? 0)) / hh;
                p.FigureShoulderToNipples = Mathf.Abs((ShoulderHeight ?? 0) - (NippleHeight ?? 0)) / hh;
                p.FigureShoulderToNavel = Mathf.Abs((ShoulderHeight ?? 0) - (NavelHeight ?? 0)) / hh;
                p.FigureShoulderToCrotch = Mathf.Abs((ShoulderHeight?? 0) - (CrotchHeight ?? 0)) / hh;
                p.FigureLengthOfLowerLimb = Mathf.Abs((CrotchHeight ?? 0) - (HeelHeight ?? 0)) / hh;
                p.FigureCrotchToBottomOfKnees = Mathf.Abs((CrotchHeight ?? 0) - (KneeHeight ?? 0)) / hh;
                p.FigureBottomOfKneesToHeels = Mathf.Abs((KneeHeight ?? 0) - (HeelHeight ?? 0)) / hh;
                return p;
            }
        }
    }

}