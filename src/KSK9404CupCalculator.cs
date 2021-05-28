using UnityEngine;
using System;

namespace LFE {
    public class KSK9404CupCalculator : ICupCalculator {

        // https://www.standard.go.kr
        public string Name => "KS K 9404:2019-0161";

        public CupSize Calculate(float bust, float underbust) {
            var bustCm = Mathf.RoundToInt(bust * 100);
            var underbustCm = Mathf.RoundToInt(underbust * 100);

            // bust size + 2 inches - if it is odd, add one more
			var band = underbustCm;
            var diff = bustCm - underbustCm;
            var diffBin = Mathf.Max(0,(int)Mathf.Floor(((float)diff-6.25f)/2.5f));

            var cupMapping = diff>73.75f ? "HUGE" : Char.ToString((char)(diffBin+64)).Replace("@", "AA").Replace("?", "AAA");

            return new CupSize { Units = "cm", Cup = cupMapping, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}