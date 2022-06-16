using UnityEngine;
using System;

namespace LFE {
    public class SizeChartAUSCupCalculator : ICupCalculator {

        // http://www.sizechart.com/brasize/australian/index.html
        public string Name => "sizechart.com (AUS)";

        public CupSize Calculate(float bust, float underbust) {
            // SuperController.singleton.ClearMessages();
            var bustCm = UnitUtils.RoundToInt(bust * 100);
            var underbustCm = UnitUtils.RoundToInt(underbust * 100);

			var band = ToBand(underbustCm);
            var cup = ToCup(bustCm, underbustCm);

            // SuperController.LogMessage($"bustCm={bustCm} underbustCm={underbustCm} bust-underbustCm={bustCm - underbustCm} band={band} cup={cup}");

            return new CupSize { Units = "cm", Cup = cup, Band = band, Bust = bust, Underbust = underbust };
        }

        private int ToBand(int underbust) {
            var bandBase = underbust - 64;
            var minBandSize = 6;
            var bandIncrement = 2;
            var underbustIncrement = 5;
            if(bandBase < 0) {
                return minBandSize;
            }
            // every range of 5 cm of band measurement translates
            // to a bra band size in 5 cm bumps
            return ((int)Mathf.Floor(bandBase/underbustIncrement)+1)*bandIncrement + minBandSize;
        }

        private string ToCup(int bustCm, int underbustCm) {
            var diff = bustCm - underbustCm;
            var diffBase = diff - 12;
            var cupIndex = Mathf.Floor(diffBase/2f) + 1;
            // SuperController.LogMessage($"diffBase={diffBase} diffBase/2={diffBase/2f} cupIndex={cupIndex}");
            if(cupIndex <= 0) {
                return "AA";
            }
            return cupIndex>26 ? "HUGE" : Char.ToString((char)(cupIndex+64));
        }
    }
}