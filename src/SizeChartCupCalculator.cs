using UnityEngine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class SizeChartCupCalculator : ICupCalculator {

        public string Name => "sizechart.com/brasize/us/index.html";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(bust) * 12);
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToFeet(underbust) * 12);

            var bustToBand = new Dictionary<Vector2, int>() {
                { new Vector2(23, 25), 28 }, { new Vector2(25, 27), 30 }, { new Vector2(27, 29), 32 }, { new Vector2(29, 31), 34 }, { new Vector2(31, 33), 36 }, { new Vector2(33, 35), 38 },
                { new Vector2(35, 37), 40 }, { new Vector2(37, 39), 42 }, { new Vector2(39, 40), 44 }, { new Vector2(41, 43), 46 },
            };

            var bustMapping = bustToBand.FirstOrDefault(kv => underbustIn >= kv.Key.x && underbustIn < kv.Key.y);
            var band = bustMapping.Value;

            var bustBandDiffToCup = new Dictionary<Vector2, string>() { { new Vector2(0, 1), "AA"}, { new Vector2(1, 2), "A"}, { new Vector2(2, 3), "B"}, { new Vector2(3, 4), "C"},
                { new Vector2(4, 5), "D"}, { new Vector2(5, 6), "DD/E"}, { new Vector2(6, 7), "DDD/F"}, { new Vector2(7, 8), "G"}, { new Vector2(8, 9), "H"}, { new Vector2(9, 10), "I"},
                { new Vector2(10, 11), "J"}, { new Vector2(11, 100000), "HUGE"},
            };
            var cupMapping = bustBandDiffToCup.FirstOrDefault(kv => Mathf.Max(0, bustIn-band) >= kv.Key.x && Mathf.Max(0, bustIn-band) < kv.Key.y);
            return new CupSize { Units = "in", Cup = cupMapping.Value, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}