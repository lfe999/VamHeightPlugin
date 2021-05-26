using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class ChateLaineCupCalculator : ICupCalculator {

        // https://www.chatelaine.com/style/fashion/bra-size-calculator/ 
        public string Name => "https://www.chatelaine.com/";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(underbust) * 12);

            // if you measure an even number, add 2, if you measure odd, add 1
            var band = underbustIn;
            if(band % 2 == 0) { band += 2; } else { band += 1; }
            var diff = Mathf.Max(0, bustIn - band);
            var bustBandDiffToCup = new Dictionary<Vector2, string>() {
                { new Vector2(0, 1), "AA"}, { new Vector2(1, 2), "A"}, { new Vector2(2, 3), "B"}, { new Vector2(3, 4), "C"}, { new Vector2(4, 5), "D"}, { new Vector2(5, 6), "DD/E"},
                { new Vector2(6, 7), "DDD/F"}, { new Vector2(7, 8), "G"}, { new Vector2(8, 9), "H"}, { new Vector2(9, 10), "I"}, { new Vector2(10, 11), "J"}, { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => diff >= kv.Key.x && diff < kv.Key.y);
            return new CupSize { Units = "in", Cup = cupMapping.Value, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}