using System;
using System.Collections.Generic;
using System.Linq;

public class AgeCalculation {
    private List<AgeGuess> _guesses = new List<AgeGuess>();
    private Dictionary<int, float> _confidenceByAge = new Dictionary<int, float>();

    public AgeCalculation(List<AgeGuess> guesses) {
        guesses = guesses ?? new List<AgeGuess>();
        _guesses = guesses.OrderByDescending(x => x.Confidence).GroupBy(x => x.Age).Select(x => x.FirstOrDefault()).ToList();
        _confidenceByAge = _guesses.ToDictionary(x => x.Age, x => x.Confidence);
    }

    public Quartiles Quartiles {
        get {
            // build up list of ages expanded by probability
            var elements = new List<float>();
            var min = MinAge;
            var max = MaxAge;

            for(var age = min; age <= max; age++) {
                var value = (int)((ConfidenceForAge(age) ?? 0)*100);
                // SuperController.LogMessage($"age={age} conf={value}");
                for(var i = 0; i < value; i++) {
                    elements.Add(age);
                }
            }
            // SuperController.LogMessage($"elements.Length={elements.Count}");

            var elementsArray = elements.ToArray();
            Array.Sort(elementsArray);

            return new Quartiles(
                Percentile(elementsArray, 0),
                Percentile(elementsArray, 0.25f),
                Percentile(elementsArray, 0.50f),
                Percentile(elementsArray, 0.75f),
                Percentile(elementsArray, 1f)
            );
        }
    }

    private float Percentile(float[] sequenceSorted, float excelPercentile)
    {
        // SuperController.LogMessage($"Percentile({sequenceSorted.Length}, {excelPercentile})");
        int N = sequenceSorted.Length;
        float n = (N - 1) * excelPercentile + 1;
        // SuperController.LogMessage($"N={N}, n={n}");
        // Another method: double n = (N + 1) * excelPercentile;
        if (n == 1f) {
            return sequenceSorted[0];
        }
        else if (n == N){
            return sequenceSorted[N - 1];
        }
        else
        {
            int k = (int)n;
            float d = n - k;
            // SuperController.LogMessage($"k={k}, d={d}");
            // SuperController.LogMessage($"ss[k-1]={sequenceSorted[k-1]}");
            return sequenceSorted[k - 1] + d * (sequenceSorted[k] - sequenceSorted[k - 1]);
        }
    }

    public int MinAge {
        get {
            return _guesses.Select(x => x.Age)?.Min() ?? 0;
        }
    }
    public int MaxAge {
        get {
            return _guesses.Select(x => x.Age)?.Max() ?? 0;
        }
    }

    public int LikelyAge {
        get {
            return _guesses.OrderByDescending(x => x.Confidence).ThenBy(x => x.Age).First()?.Age ?? 0;
        }
    }

    public Dictionary<int, float> ConfidenceByAge {
        get {
            return _confidenceByAge;
        }
    }

    public float? ConfidenceForAge(int ageYears) {
        float confidence;
        if(_confidenceByAge.TryGetValue(ageYears, out confidence)) {
            return confidence;
        }
        else {
            return null;
        }
    }
}