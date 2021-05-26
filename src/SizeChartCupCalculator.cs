using UnityEngine;

namespace LFE {
    public class SizeChartCupCalculator : ICupCalculator {

        public string Name => "sizechart.com/brasize/us/index.html";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToInches(bust));
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToInches(underbust));

            // their underbust to band algorithm seems to be:
            // if odd underbust, add one - then add 4
            var band = (underbustIn % 2 == 1) ? underbustIn + 5 : underbustIn + 4;
            var diff = Mathf.Max(0, bustIn - band);
            var cupMapping = CupSize.DifferenceToCupUS(diff);
            return new CupSize { Units = "in", Cup = cupMapping, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}