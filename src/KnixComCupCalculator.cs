using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class KnixComCupCalculator : ICupCalculator {

        // https://knix.com/blogs/resources/how-to-measure-bra-band-size#:~:text=Finally%2C%20Find%20Your%20Cup%20Size%20%20%20Bust,%20%20C%20%207%20more%20rows%20
        public string Name => "https://knix.com/";

        public CupSize Calculate(float bust, float underbust) {
            var bustIn = Mathf.RoundToInt(UnitUtils.UnityToInches(bust));
            var underbustIn = Mathf.RoundToInt(UnitUtils.UnityToInches(underbust));

            // bust size + 2 inches - if it is odd, add one more
            var band = underbustIn + 2 + (underbustIn % 2);
            var diff = Mathf.Max(0, bustIn - band);
            var cupMapping = CupSize.DifferenceToCupUS(diff);
            return new CupSize { Units = "in", Cup = cupMapping, Band = band, Bust = bust, Underbust = underbust };
        }
    }
}