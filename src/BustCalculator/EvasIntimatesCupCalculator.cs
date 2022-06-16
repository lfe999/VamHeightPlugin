using UnityEngine;

namespace LFE {
    public class EvasIntimatesCupCalculator : ICupCalculator {
        // https://www.evasintimates.com/blog/us-bra-sizes/#AA-Cups

        public string Name => "Evas Intimates (US)";

        public CupSize Calculate(float bust, float underbust) {
            // instructions say to measure TIGHTLY under bust so subtracting 3/8 inch here
            var tightUnderbust = underbust - UnitUtils.InchesToUnity(0.375f);
            var bustIn = UnitUtils.UnityToInchesRounded(bust);
            var underbustIn = UnitUtils.UnityToInchesRounded(tightUnderbust);

            // underbust size + 4 inches - if it is odd subtract one to get band
            var band = underbustIn + 4 - (underbustIn % 2);
            var diff = Mathf.Max(0, bustIn - band);
            var cupMapping = CupSize.DifferenceToCupUS(diff);
            return new CupSize { Units = "in", Cup = cupMapping, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}