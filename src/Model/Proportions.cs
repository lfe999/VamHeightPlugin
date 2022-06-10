using System;
using System.Collections.Generic;
using System.Linq;

// units are in head units, not unity units
public class Proportions {

    public static List<Proportions> CommonProportions = new List<Proportions>() {
        // https://hpc.anatomy4sculptors.com/
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Elderly",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderWidth = 1.5f,
            FigureShoulderToNipples = 0.8f,
            FigureShoulderToNavel = 1.5f,
            FigureShoulderToCrotch = 2.1f,
            FigureLengthOfLowerLimb = 3.5f,
            FigureCrotchToBottomOfKnees = 1.8f,
            FigureBottomOfKneesToHeels = 1.7f,
            EstimatedAgeRangeMin = 65,
            EstimatedAgeRangeMax = 90
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Adult",
            FigureHeightInHeads = 8f,
            FigureChinToShoulder = 0.33f,
            FigureShoulderWidth = 2f,
            FigureShoulderToNipples = 0.8f,
            FigureShoulderToNavel = 1.8f,
            FigureShoulderToCrotch = 3f,
            FigureLengthOfLowerLimb = 3.6f,
            FigureCrotchToBottomOfKnees = 1.7f,
            FigureBottomOfKneesToHeels = 2f,
            EstimatedAgeRangeMin = 25,
            EstimatedAgeRangeMax = 64
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Young Adult",
            FigureHeightInHeads = 7.5f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderWidth = 1.5f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.6f,
            FigureShoulderToCrotch = 2.7f,
            FigureLengthOfLowerLimb = 3.5f,
            FigureCrotchToBottomOfKnees = 1.7f,
            FigureBottomOfKneesToHeels = 1.8f,
            EstimatedAgeRangeMin = 19,
            EstimatedAgeRangeMax = 24 
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Teenager",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderWidth = 1.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.3f,
            FigureShoulderToCrotch = 2.3f,
            FigureLengthOfLowerLimb = 3.3f,
            FigureCrotchToBottomOfKnees = 1.5f,
            FigureBottomOfKneesToHeels = 1.8f,
            EstimatedAgeRangeMin = 12,
            EstimatedAgeRangeMax = 18
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Elderly",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderWidth = 1.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.5f,
            FigureShoulderToCrotch = 2.1f,
            FigureLengthOfLowerLimb = 3.5f,
            FigureCrotchToBottomOfKnees = 1.8f,
            FigureBottomOfKneesToHeels = 1.7f,
            EstimatedAgeRangeMin = 65,
            EstimatedAgeRangeMax = 90
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Adult",
            FigureHeightInHeads = 8f,
            FigureChinToShoulder = 0.33f,
            FigureShoulderWidth = 2.0f,
            FigureShoulderToNipples = 0.66f,
            FigureShoulderToNavel = 1.65f,
            FigureShoulderToCrotch = 2.6f,
            FigureLengthOfLowerLimb = 4f,
            FigureCrotchToBottomOfKnees = 2f,
            FigureBottomOfKneesToHeels = 2f,
            EstimatedAgeRangeMin = 25,
            EstimatedAgeRangeMax = 64
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Young Adult",
            FigureHeightInHeads = 7.5f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderWidth = 1.7f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.5f,
            FigureShoulderToCrotch = 2.4f,
            FigureLengthOfLowerLimb = 3.7f,
            FigureCrotchToBottomOfKnees = 1.9f,
            FigureBottomOfKneesToHeels = 1.8f,
            EstimatedAgeRangeMin = 19,
            EstimatedAgeRangeMax = 24
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Teenager",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderWidth = 1.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.3f,
            FigureShoulderToCrotch = 2.3f,
            FigureLengthOfLowerLimb = 3.3f,
            FigureCrotchToBottomOfKnees = 1.5f,
            FigureBottomOfKneesToHeels = 1.8f,
            EstimatedAgeRangeMin = 12,
            EstimatedAgeRangeMax = 18
        }
    };

    public bool IsFemale { get; set; } = true;

    public string ProportionName { get; set; }
    public float FigureHeightInHeads { get; set; }
    public float FigureChinToShoulder { get; set; }
    public float FigureShoulderWidth { get; set; }
    public float FigureShoulderToNipples { get; set; }
    public float FigureShoulderToNavel { get; set; }
    public float FigureShoulderToCrotch { get; set; }
    public float FigureLengthOfUpperLimb { get; set; }
    // public float FigureLengthOfArm { get; set; } // TODO
    // public float FigureLengthOfForearmAndHand { get; set; } // TODO
    public float FigureLengthOfLowerLimb { get; set; } // crotch to feet
    public float FigureCrotchToBottomOfKnees { get; set; }
    public float FigureBottomOfKneesToHeels { get; set; }
    public int EstimatedAgeRangeMin { get; set; }
    public int EstimatedAgeRangeMax { get; set; }

    public Quartiles Quartiles {
        get {
            if(EstimatedAgeRangeMin == 0 && EstimatedAgeRangeMax == 0) {
                return null;
            }

            return new Quartiles(
                EstimatedAgeRangeMin,
                EstimatedAgeRangeMin,
                (EstimatedAgeRangeMin + EstimatedAgeRangeMax) / 2f,
                EstimatedAgeRangeMax,
                EstimatedAgeRangeMax
            );
        }
    }

    public List<Proportions> ClosestMatches(List<Proportions> proportions) {
        return proportions.Where(p => p.IsFemale == this.IsFemale).OrderBy(x => x.ProportionsDeltaScaled(this).SortScore()).ToList();
    }

    public Proportions ClostestMatch(List<Proportions> proportions) {
        var matches = ClosestMatches(proportions);
        if(matches.Count == 0) {
            return null;
        }
        return matches[0];
    }

    public Proportions ProportionsDelta(Proportions x) {
        return new Proportions() {
            ProportionName = $"{ProportionName} vs {x.ProportionName}",
            FigureHeightInHeads = FigureHeightInHeads - x.FigureHeightInHeads,
            FigureChinToShoulder = FigureChinToShoulder - x.FigureChinToShoulder,
            FigureShoulderWidth = FigureShoulderWidth - x.FigureShoulderWidth,
            FigureShoulderToNipples = FigureShoulderToNipples - x.FigureShoulderToNipples,
            FigureShoulderToNavel = FigureShoulderToNavel - x.FigureShoulderToNavel,
            FigureShoulderToCrotch = FigureShoulderToCrotch - x.FigureShoulderToCrotch,
            FigureLengthOfLowerLimb = FigureLengthOfLowerLimb - x.FigureLengthOfLowerLimb,
            FigureLengthOfUpperLimb = FigureLengthOfUpperLimb - x.FigureLengthOfUpperLimb,
            FigureCrotchToBottomOfKnees = FigureCrotchToBottomOfKnees - x.FigureCrotchToBottomOfKnees,
            FigureBottomOfKneesToHeels = FigureBottomOfKneesToHeels - x.FigureBottomOfKneesToHeels
        };
    }

    public Proportions ProportionsDeltaScaled(Proportions x) {
        var normalizeThis = 1f;
        var normalizeX = 1f;
        if(FigureHeightInHeads > x.FigureHeightInHeads && x.FigureHeightInHeads != 0) {
            normalizeX = FigureHeightInHeads / x.FigureHeightInHeads;
        }
        else if(x.FigureHeightInHeads > FigureHeightInHeads && FigureHeightInHeads != 0) {
            normalizeThis = x.FigureHeightInHeads / FigureHeightInHeads;
        }

        return new Proportions() {
            ProportionName = $"{ProportionName} vs {x.ProportionName}",
            FigureHeightInHeads = FigureHeightInHeads - x.FigureHeightInHeads,
            FigureChinToShoulder = (FigureChinToShoulder*normalizeThis) - (x.FigureChinToShoulder*normalizeX),
            FigureShoulderWidth = (FigureShoulderWidth*normalizeThis) - (x.FigureShoulderWidth*normalizeX),
            FigureShoulderToNipples = (FigureShoulderToNipples*normalizeThis) - (x.FigureShoulderToNipples*normalizeX),
            FigureShoulderToNavel = (FigureShoulderToNavel*normalizeThis) - (x.FigureShoulderToNavel*normalizeX),
            FigureShoulderToCrotch = (FigureShoulderToCrotch*normalizeThis) - (x.FigureShoulderToCrotch*normalizeX),
            FigureLengthOfLowerLimb = (FigureLengthOfLowerLimb*normalizeThis) - (x.FigureLengthOfLowerLimb*normalizeX),
            FigureLengthOfUpperLimb = (FigureLengthOfUpperLimb*normalizeThis) - (x.FigureLengthOfUpperLimb*normalizeX),
            FigureCrotchToBottomOfKnees = (FigureCrotchToBottomOfKnees*normalizeThis) - (x.FigureCrotchToBottomOfKnees*normalizeX),
            FigureBottomOfKneesToHeels = (FigureBottomOfKneesToHeels*normalizeThis) - (x.FigureBottomOfKneesToHeels*normalizeX)
        };
    }

    public Proportions ProportionsDeltaPercentage(Proportions x) {
        var deltaFixed = ProportionsDelta(x);
        return new Proportions() {
            ProportionName = $"{deltaFixed.ProportionName} %",
            FigureHeightInHeads = (FigureHeightInHeads - x.FigureHeightInHeads) / FigureHeightInHeads,
            FigureChinToShoulder = (FigureChinToShoulder - x.FigureChinToShoulder) / FigureChinToShoulder,
            FigureShoulderWidth = (FigureShoulderWidth - x.FigureShoulderWidth) / FigureShoulderWidth,
            FigureShoulderToNipples = (FigureShoulderToNipples - x.FigureShoulderToNipples) / FigureShoulderToNipples,
            FigureShoulderToNavel = (FigureShoulderToNavel - x.FigureShoulderToNavel) / FigureShoulderToNavel,
            FigureShoulderToCrotch = (FigureShoulderToCrotch - x.FigureShoulderToCrotch) / FigureShoulderToCrotch,
            FigureLengthOfLowerLimb = (FigureLengthOfLowerLimb - x.FigureLengthOfLowerLimb) / FigureLengthOfLowerLimb,
            FigureLengthOfUpperLimb = (FigureLengthOfUpperLimb - x.FigureLengthOfUpperLimb) / FigureLengthOfUpperLimb,
            FigureCrotchToBottomOfKnees = (FigureCrotchToBottomOfKnees - x.FigureCrotchToBottomOfKnees) / FigureCrotchToBottomOfKnees,
            FigureBottomOfKneesToHeels = (FigureBottomOfKneesToHeels - x.FigureBottomOfKneesToHeels) / FigureBottomOfKneesToHeels
        };
    }

    public float SortScore() {
        return (Math.Abs(FigureHeightInHeads))
            + Math.Abs(FigureChinToShoulder)
            + Math.Abs(FigureShoulderWidth)
            + Math.Abs(FigureShoulderToNipples)
            + Math.Abs(FigureShoulderToNavel)
            + Math.Abs(FigureShoulderToCrotch)
            + Math.Abs(FigureLengthOfLowerLimb)
            + Math.Abs(FigureLengthOfUpperLimb)
            + Math.Abs(FigureCrotchToBottomOfKnees)
            + Math.Abs(FigureBottomOfKneesToHeels);
    }

    public override string ToString() {
        return $"Proportions({ProportionName})\n"
                + $"    FigureHeightInHeads={r(FigureHeightInHeads)}\n"
                + $"    FigureChinToShoulder={r(FigureChinToShoulder)}\n"
                + $"    FigureShoulderWidth={r(FigureShoulderWidth)}\n"
                + $"    FigureShoulderToNipples={r(FigureShoulderToNipples)}\n"
                + $"    FigureShoulderToNavel={r(FigureShoulderToNavel)}\n"
                + $"    FigureShoulderToCrotch={r(FigureShoulderToCrotch)}\n"
                + $"    FigureLengthOfLowerLimb={r(FigureLengthOfLowerLimb)}\n"
                + $"    FigureLengthOfUpperLimb={r(FigureLengthOfLowerLimb)}\n"
                + $"    FigureCrotchToBottomOfKnees={r(FigureCrotchToBottomOfKnees)}\n"
                + $"    FigureBottomOfKneesToHeels={r(FigureBottomOfKneesToHeels)}";
    }

    public Proportions Clone() {
        return new Proportions() {
            ProportionName = this.ProportionName,
            IsFemale = this.IsFemale,
            FigureHeightInHeads = this.FigureHeightInHeads,
            FigureChinToShoulder = this.FigureChinToShoulder,
            FigureShoulderWidth = this.FigureShoulderWidth,
            FigureShoulderToNavel = this.FigureShoulderToNavel,
            FigureShoulderToNipples = this.FigureShoulderToNipples,
            FigureShoulderToCrotch = this.FigureShoulderToCrotch,
            FigureLengthOfLowerLimb = this.FigureLengthOfLowerLimb,
            FigureLengthOfUpperLimb = this.FigureLengthOfUpperLimb,
            FigureCrotchToBottomOfKnees = this.FigureCrotchToBottomOfKnees,
            FigureBottomOfKneesToHeels = this.FigureBottomOfKneesToHeels,
            EstimatedAgeRangeMin = this.EstimatedAgeRangeMin,
            EstimatedAgeRangeMax = this.EstimatedAgeRangeMax
        };
    }

    private float r(float x) {
        return (float)Math.Round(x, 2);
    }
}