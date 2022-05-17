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