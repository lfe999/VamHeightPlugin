using System.Collections.Generic;
using System;
using System.Linq;

public class Quartiles {
    public Quartiles(float q0, float q25, float q50, float q75, float q100) {
        Q0 = q0;
        Q25 = q25;
        Q50 = q50;
        Q75 = q75;
        Q100 = q100;
    }
    public float Q0 { get; set; }
    public float Q25 { get; set; }
    public float Q50 { get; set; }
    public float Q75 { get; set; }
    public float Q100 { get; set; }

    public string RangeString {
        get {
            var likely = (int)Q25 != (int)Q75 ? $"{Q25} to {Q75}" : $"{Q25}";
            var maybe  = (int)Q0 != (int)Q100 && !((int)Q0 == (int)Q25 && (int)Q100 == (int)Q75) ? $"{Q0} to {Q100}" : String.Empty; 
            if(maybe != String.Empty) {
                return $"{likely} ({maybe} less likely)";
            }
            return likely;
        }
    }

    public static float? GroupMin(IEnumerable<Quartiles> quartiles)
    {
        if(quartiles == null) {
            return null;
        }

        var qList = quartiles.Where(q => q != null).ToList();
        if(qList.Count == 0) {
            return null;
        }

        return quartiles.Select(q => q.Q0).Min();
    }

    public static float? GroupMax(IEnumerable<Quartiles> quartiles)
    {
        if(quartiles == null) {
            return null;
        }

        var qList = quartiles.Where(q => q != null).ToList();
        if(qList.Count == 0) {
            return null;
        }

        return quartiles.Select(q => q.Q100).Max();
    }


    public static Quartiles GroupOverlapQuartile(IEnumerable<Quartiles> quartiles) {
        var qList = quartiles.Where(q => q != null).ToList();
        if(qList.Count == 0) {
            return null;
        }
        var groupMin = Quartiles.GroupMin(qList);
        var groupMax = Quartiles.GroupMax(qList);

        if(groupMin == null || groupMax == null) {
            return null;
        }

        int chartCount = qList.Count;

        List<float> overlappingAges = new List<float>();
        List<KeyValuePair<float, float>> overlappingAgeScores = new List<KeyValuePair<float, float>>();
        for(var i = (int)groupMin.Value; i <= (int)groupMax.Value; i++) {
            // SuperController.LogMessage($"-----------checking age {i}-------");
            float ageScore = 0;
            int foundInCharts = 0;
            foreach(var q in qList) {
                if(i >= q.Q25 && i <= q.Q75) {
                    // SuperController.LogMessage($"i={i} and is >= {q.Q25} and <= {q.Q75}");
                    foundInCharts++;
                    ageScore += 1;
                }
                else if(i >= q.Q0 && i <= q.Q100) {
                    // SuperController.LogMessage($"i={i} and is >= {q.Q0} and <= {q.Q100}");
                    ageScore += 0.5f;
                }
            }
            var foundInAllGroups = foundInCharts == chartCount;
            ageScore = ageScore/chartCount;
            if(foundInAllGroups) {
                // SuperController.LogMessage($"i was found in all {qList.Count} quartiles");
                overlappingAges.Add((float)i);
            }

            if(ageScore >= 0.5f) {
            // if(ageScore > 0.66f) {
                // SuperController.LogMessage($"{i} has ageScore {ageScore} - 0.5");
                overlappingAgeScores.Add(new KeyValuePair<float, float>((float)i, 0.5f));   
            }
            else if(ageScore > 0.34f) {
                // SuperController.LogMessage($"{i} has ageScore {ageScore} - 0.25");
                overlappingAgeScores.Add(new KeyValuePair<float, float>((float)i, 0.25f));
            }
        }
        if(overlappingAgeScores.Count > 0) {
            // foreach(var kv in overlappingAgeScores) {
            //     SuperController.LogMessage($"{kv.Key} => {kv.Value}");
            // }
            try {
                var allAges = overlappingAgeScores.Select(kvp => kvp.Key).ToList();
                var likelyAges = overlappingAgeScores.Where(kvp => kvp.Value >= 0.5f).Select(kvp => kvp.Key).ToList();
                var highMin = likelyAges.Count > 0 ? likelyAges.Min() : 0;
                var highMax = likelyAges.Count > 0 ? likelyAges.Max() : 0;

                if(likelyAges.Count == 0) {
                    var min = allAges.Min();
                    var max = allAges.Max();
                    var mid = (min + max) / 2f;
                    return new Quartiles(
                        groupMin.Value,
                        (float)Math.Floor(mid),
                        (int)mid,
                        (float)Math.Ceiling(mid),
                        groupMax.Value
                    );

                }
                else {
                    var mid = (highMin + highMax) / 2f;
                    var min = allAges.Min();
                    var max = allAges.Max();
                    // SuperController.LogMessage("here");
                    return new Quartiles(
                        groupMin.Value,
                        highMin,
                        mid,
                        highMax,
                        groupMax.Value
                    );

                }
            }
            catch(Exception e) {
                SuperController.LogError($"Failed to calculate combined age score: {e}");
            }
        }
        return null;

    }

    public override string ToString()
    {
        return $"quartile={Q0},{Q25},{Q50},{Q75},{Q100}";
    }
}