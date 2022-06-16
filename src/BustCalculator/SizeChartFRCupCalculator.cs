using UnityEngine;

namespace LFE {
    public class SizeChartFRCupCalculator : ICupCalculator {

        // http://www.sizechart.com/brasize/french/index.html
        public string Name => "sizechart.com (FR)";

        public CupSize Calculate(float bust, float underbust) {
            var bustCm = UnitUtils.RoundToInt(bust * 100);
            var underbustCm = UnitUtils.RoundToInt(underbust * 100);

			var band = ToBand(underbustCm);
            var cup = ToCup(bustCm, underbustCm);

            return new CupSize { Units = "cm", Cup = cup, Band = band, Bust = bust, Underbust = underbust };
        }

        private int ToBand(int underbust) {
            var bandBase = underbust - 64;
            var minBandSize = 75;
            if(bandBase < 0) {
                return minBandSize;
            }
            // every range of 5 cm of band measurement translates
            // to a bra band size in 5 cm bumps
            return (Mathf.Floor(bandBase/5)+1)*5 + minBandSize;
        }

        private string ToCup(int bustCm, int underbustCm) {
            var diff = bustCm - underbustCm;
            var diffBase = diff - 12;
            var cupIndex = Mathf.Floor(diffBase/2);
            if(cupIndex < 0) {
                return "AA";
            }
            return cupIndex>25 ? "HUGE" : Char.ToString((char)(cupIndex+64));
        }
    }
}