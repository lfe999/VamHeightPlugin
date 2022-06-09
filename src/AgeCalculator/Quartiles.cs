using System.Collections.Generic;
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
            if((int)Q25 != (int)Q75) {
                return $"{Q25} to {Q75}";
            }
            else {
                return $"{Q25}";
            }
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

        List<float> overlappingAges = new List<float>();
        for(var i = (int)groupMin.Value; i <= (int)groupMax.Value; i++) {
            // SuperController.LogMessage($"-----------checking age {i}-------");
            int foundInCharts = 0;
            foreach(var q in qList) {
                if(i >= q.Q25 && i <= q.Q75) {
                    // SuperController.LogMessage($"i={i} and is >= {q.Q25} and <= {q.Q75}");
                    foundInCharts++;
                }
            }
            var foundInAllGroups = foundInCharts == qList.Count;
            if(foundInAllGroups) {
                // SuperController.LogMessage($"i was found in all {qList.Count} quartiles");
                overlappingAges.Add((float)i);
            }
        }
        if(overlappingAges.Count > 0) {
            var overlappingMin = overlappingAges.Min();
            var overlappingMax = overlappingAges.Max();
            return new Quartiles(
                overlappingMin,
                overlappingMin,
                (overlappingMin + overlappingMax) / 2f,
                overlappingMax,
                overlappingMax
            );
        }
        else {
            return null;
        }
    }

    public override string ToString()
    {
        return $"quartile={Q0},{Q25},{Q50},{Q75},{Q100}";
    }
}