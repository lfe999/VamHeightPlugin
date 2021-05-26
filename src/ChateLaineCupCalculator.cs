using UnityEngine;

namespace LFE {
    public class ChateLaineCupCalculator : ICupCalculator {

        // https://www.chatelaine.com/style/fashion/bra-size-calculator/ 
        public string Name => "https://www.chatelaine.com/";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToInches(bust));
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToInches(underbust));

            // if you measure an even number, add 2, if you measure odd, add 1
            var band = underbustIn % 2 == 0 ? underbustIn + 2 : underbustIn + 1;
            var diff = Mathf.Max(0, bustIn - band);
            var cupMapping = CupSize.DifferenceToCupUS(diff);
            return new CupSize { Units = "in", Cup = cupMapping, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}