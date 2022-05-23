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
            FigureShoulderToNipples = 0.8f,
            FigureShoulderToNavel = 1.5f,
            FigureShoulderToCrotch = 2.1f,
            FigureLengthOfLowerLimb = 3.5f,
            FigureCrotchToBottomOfKnees = 1.8f,
            FigureBottomOfKneesToHeels = 1.7f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Adult",
            FigureHeightInHeads = 8f,
            FigureChinToShoulder = 0.33f,
            FigureShoulderToNipples = 0.8f,
            FigureShoulderToNavel = 1.8f,
            FigureShoulderToCrotch = 3f,
            FigureLengthOfLowerLimb = 3.6f,
            FigureCrotchToBottomOfKnees = 1.7f,
            FigureBottomOfKneesToHeels = 2f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Young Adult",
            FigureHeightInHeads = 7.5f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.6f,
            FigureShoulderToCrotch = 2.7f,
            FigureLengthOfLowerLimb = 3.5f,
            FigureCrotchToBottomOfKnees = 1.7f,
            FigureBottomOfKneesToHeels = 1.8f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Teenager",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.3f,
            FigureShoulderToCrotch = 2.3f,
            FigureLengthOfLowerLimb = 3.3f,
            FigureCrotchToBottomOfKnees = 1.5f,
            FigureBottomOfKneesToHeels = 1.8f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Child",
            FigureHeightInHeads = 6f,
            FigureChinToShoulder = 0.2f,
            FigureShoulderToNipples = 0.4f,
            FigureShoulderToNavel = 1.3f,
            FigureShoulderToCrotch = 2f,
            FigureLengthOfLowerLimb = 2.8f,
            FigureCrotchToBottomOfKnees = 1.4f,
            FigureBottomOfKneesToHeels = 1.4f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Young Child",
            FigureHeightInHeads = 5.5f,
            FigureChinToShoulder = 0.2f,
            FigureShoulderToNipples = 0.4f,
            FigureShoulderToNavel = 1.2f,
            FigureShoulderToCrotch = 2f,
            FigureLengthOfLowerLimb = 2.3f,
            FigureCrotchToBottomOfKnees = 1f,
            FigureBottomOfKneesToHeels = 1.3f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Infant",
            FigureHeightInHeads = 5f,
            FigureChinToShoulder = 0.2f,
            FigureShoulderToNipples = 0.4f,
            FigureShoulderToNavel = 1f,
            FigureShoulderToCrotch = 1.8f,
            FigureLengthOfLowerLimb = 2f,
            FigureCrotchToBottomOfKnees = 1f,
            FigureBottomOfKneesToHeels = 1f
        },
        new Proportions() {
            IsFemale = true,
            ProportionName = "Female Newborn",
            FigureHeightInHeads = 4f,
            FigureChinToShoulder = 0.0f,
            FigureShoulderToNipples = 0.33f,
            FigureShoulderToNavel = 1.2f,
            FigureShoulderToCrotch = 1.6f,
            FigureLengthOfLowerLimb = 0.6f,
            FigureCrotchToBottomOfKnees = 0.6f,
            FigureBottomOfKneesToHeels = 0.8f
        },

        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Elderly",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.5f,
            FigureShoulderToCrotch = 2.1f,
            FigureLengthOfLowerLimb = 3.5f,
            FigureCrotchToBottomOfKnees = 1.8f,
            FigureBottomOfKneesToHeels = 1.7f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Adult",
            FigureHeightInHeads = 8f,
            FigureChinToShoulder = 0.33f,
            FigureShoulderToNipples = 0.66f,
            FigureShoulderToNavel = 1.65f,
            FigureShoulderToCrotch = 2.6f,
            FigureLengthOfLowerLimb = 4f,
            FigureCrotchToBottomOfKnees = 2f,
            FigureBottomOfKneesToHeels = 2f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Young Adult",
            FigureHeightInHeads = 7.5f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.5f,
            FigureShoulderToCrotch = 2.4f,
            FigureLengthOfLowerLimb = 3.7f,
            FigureCrotchToBottomOfKnees = 1.9f,
            FigureBottomOfKneesToHeels = 1.8f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Teenager",
            FigureHeightInHeads = 7f,
            FigureChinToShoulder = 0.3f,
            FigureShoulderToNipples = 0.6f,
            FigureShoulderToNavel = 1.3f,
            FigureShoulderToCrotch = 2.3f,
            FigureLengthOfLowerLimb = 3.3f,
            FigureCrotchToBottomOfKnees = 1.5f,
            FigureBottomOfKneesToHeels = 1.8f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Child",
            FigureHeightInHeads = 6f,
            FigureChinToShoulder = 0.2f,
            FigureShoulderToNipples = 0.4f,
            FigureShoulderToNavel = 1.3f,
            FigureShoulderToCrotch = 2f,
            FigureLengthOfLowerLimb = 2.8f,
            FigureCrotchToBottomOfKnees = 1.4f,
            FigureBottomOfKneesToHeels = 1.4f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Young Child",
            FigureHeightInHeads = 5.5f,
            FigureChinToShoulder = 0.2f,
            FigureShoulderToNipples = 0.4f,
            FigureShoulderToNavel = 1.2f,
            FigureShoulderToCrotch = 2f,
            FigureLengthOfLowerLimb = 2.3f,
            FigureCrotchToBottomOfKnees = 1f,
            FigureBottomOfKneesToHeels = 1.3f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Infant",
            FigureHeightInHeads = 5f,
            FigureChinToShoulder = 0.2f,
            FigureShoulderToNipples = 0.4f,
            FigureShoulderToNavel = 1f,
            FigureShoulderToCrotch = 1.8f,
            FigureLengthOfLowerLimb = 2f,
            FigureCrotchToBottomOfKnees = 1f,
            FigureBottomOfKneesToHeels = 1f
        },
        new Proportions() {
            IsFemale = false,
            ProportionName = "Male Newborn",
            FigureHeightInHeads = 4f,
            FigureChinToShoulder = 0.0f,
            FigureShoulderToNipples = 0.33f,
            FigureShoulderToNavel = 1.2f,
            FigureShoulderToCrotch = 1.6f,
            FigureLengthOfLowerLimb = 0.6f,
            FigureCrotchToBottomOfKnees = 0.6f,
            FigureBottomOfKneesToHeels = 0.8f
        }
    };

    public bool IsFemale { get; set; } = true;

    public string ProportionName { get; set; }
    public float FigureHeightInHeads { get; set; }
    public float FigureChinToShoulder { get; set; }
    // public float FigureWidthOfShoulders { get; set; } // TODO
    public float FigureShoulderToNipples { get; set; }
    public float FigureShoulderToNavel { get; set; }
    public float FigureShoulderToCrotch { get; set; }
    // public float FigureLengthOfUpperLimb { get; set; } // TODO
    // public float FigureLengthOfArm { get; set; } // TODO
    // public float FigureLengthOfForearmAndHand { get; set; } // TODO
    public float FigureLengthOfLowerLimb { get; set; } // crotch to feet
    public float FigureCrotchToBottomOfKnees { get; set; }
    public float FigureBottomOfKneesToHeels { get; set; }

    public List<Proportions> ClosestMatches(List<Proportions> proportions) {
        return proportions.Where(p => p.IsFemale == this.IsFemale).OrderBy(x => x.ProportionsDeltaScaled(this).SortScore()).ToList();
    }

    public Proportions ClostestMatch(List<Proportions> proportions) {
        return ClosestMatches(proportions).First();
    }

    public Proportions ProportionsDelta(Proportions x) {
        return new Proportions() {
            ProportionName = $"{ProportionName} vs {x.ProportionName}",
            FigureHeightInHeads = FigureHeightInHeads - x.FigureHeightInHeads,
            FigureChinToShoulder = FigureChinToShoulder - x.FigureChinToShoulder,
            FigureShoulderToNipples = FigureShoulderToNipples - x.FigureShoulderToNipples,
            FigureShoulderToNavel = FigureShoulderToNavel - x.FigureShoulderToNavel,
            FigureShoulderToCrotch = FigureShoulderToCrotch - x.FigureShoulderToCrotch,
            FigureLengthOfLowerLimb = FigureLengthOfLowerLimb - x.FigureLengthOfLowerLimb,
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
            FigureShoulderToNipples = (FigureShoulderToNipples*normalizeThis) - (x.FigureShoulderToNipples*normalizeX),
            FigureShoulderToNavel = (FigureShoulderToNavel*normalizeThis) - (x.FigureShoulderToNavel*normalizeX),
            FigureShoulderToCrotch = (FigureShoulderToCrotch*normalizeThis) - (x.FigureShoulderToCrotch*normalizeX),
            FigureLengthOfLowerLimb = (FigureLengthOfLowerLimb*normalizeThis) - (x.FigureLengthOfLowerLimb*normalizeX),
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
            FigureShoulderToNipples = (FigureShoulderToNipples - x.FigureShoulderToNipples) / FigureShoulderToNipples,
            FigureShoulderToNavel = (FigureShoulderToNavel - x.FigureShoulderToNavel) / FigureShoulderToNavel,
            FigureShoulderToCrotch = (FigureShoulderToCrotch - x.FigureShoulderToCrotch) / FigureShoulderToCrotch,
            FigureLengthOfLowerLimb = (FigureLengthOfLowerLimb - x.FigureLengthOfLowerLimb) / FigureLengthOfLowerLimb,
            FigureCrotchToBottomOfKnees = (FigureCrotchToBottomOfKnees - x.FigureCrotchToBottomOfKnees) / FigureCrotchToBottomOfKnees,
            FigureBottomOfKneesToHeels = (FigureBottomOfKneesToHeels - x.FigureBottomOfKneesToHeels) / FigureBottomOfKneesToHeels
        };
    }

    public float SortScore() {
        return (Math.Abs(FigureHeightInHeads))
            + Math.Abs(FigureChinToShoulder)
            + Math.Abs(FigureShoulderToNipples)
            + Math.Abs(FigureShoulderToNavel)
            + Math.Abs(FigureShoulderToCrotch)
            + Math.Abs(FigureLengthOfLowerLimb)
            + Math.Abs(FigureCrotchToBottomOfKnees)
            + Math.Abs(FigureBottomOfKneesToHeels);
    }

    public override string ToString() {
        return $"Proportions({ProportionName})\n"
                + $"    FigureHeightInHeads={r(FigureHeightInHeads)}\n"
                + $"    FigureChinToShoulder={r(FigureChinToShoulder)}\n"
                + $"    FigureShoulderToNipples={r(FigureShoulderToNipples)}\n"
                + $"    FigureShoulderToNavel={r(FigureShoulderToNavel)}\n"
                + $"    FigureShoulderToCrotch={r(FigureShoulderToCrotch)}\n"
                + $"    FigureLengthOfLowerLimb={r(FigureLengthOfLowerLimb)}\n"
                + $"    FigureCrotchToBottomOfKnees={r(FigureCrotchToBottomOfKnees)}\n"
                + $"    FigureBottomOfKneesToHeels={r(FigureBottomOfKneesToHeels)}";
    }

    private float r(float x) {
        return (float)Math.Round(x, 2);
    }
}