using UnityEngine;
using System;

namespace LFE {
    public class EvasIntimatesFRCupCalculator : ICupCalculator {
        // https://www.evasintimates.com/blog/fr-es-be-bra-sizes/

        public string Name => "Evas Intimates (FR/ES/BE)";

        public CupSize Calculate(float bust, float underbust) {
            // SuperController.singleton.ClearMessages();
            var bustCm = UnitUtils.RoundToInt(bust * 100);
            var underbustCm = UnitUtils.RoundToInt(underbust * 100);

            var underbustDownCm = underbustCm - underbustCm%5;

            // band is underbust rounded down to the nearest x5 - and add 15cm
            var band = underbustDownCm + 15;

            // cup is based on difference in bust / underbust and rounded down to nearest 5
            var diff = Mathf.Max(0, bustCm - underbustDownCm);

            var cupIndex = Mathf.Floor((diff - 12)/2f) + 1;
            var cup = "AA";
            if(cupIndex > 0) {
                cup = cupIndex > 26 ? "HUGE" : Char.ToString((char)(cupIndex+64));
            }

            // SuperController.LogMessage($"bustCm={bustCm} underCm={underbustCm} underDownCm={underbustDownCm} diff={diff} band={band} cupIndex={cupIndex} cup={cup}");

            return new CupSize { Units = "in", Cup = cup, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}