using UnityEngine;

namespace LFE {
    public class ChateLaineCupCalculator : ICupCalculator {

        // https://www.chatelaine.com/style/fashion/bra-size-calculator/ 
        public string Name => "Chate Laine (US)";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = UnitUtils.UnityToInchesRounded(bust);
            var underbustIn = UnitUtils.UnityToInchesRounded(underbust);

            // if you measure an even number, add 2, if you measure odd, add 1
            var band = underbustIn % 2 == 0 ? underbustIn + 2 : underbustIn + 1;
            var diff = Mathf.Max(0, bustIn - band);
            var cupMapping = CupSize.DifferenceToCupUS(diff);
            return new CupSize { Units = "in", Cup = cupMapping, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}