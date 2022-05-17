using System;
using System.Collections.Generic;
using System.Linq;

public class Anatomy4SculptersCalculator : IAgeCalculator {
    
    public AgeCalculation GuessFemaleAges(float y) {
        // return Algorithm2(y);
        return Algorithm1(y);
    }

    public AgeCalculation GuessMaleAges(float y) {
        return GuessFemaleAges(y);
    }

    private AgeCalculation Algorithm1(float y) {
        // SuperController.singleton.ClearMessages();
        var response = new List<AgeGuess>();
        
        var rangeSize = 4;
        var rangeAmount = 0.075f;
        
        for(var i = rangeSize*-1; i <= rangeSize ; i++) {
            var headSize = y + i*rangeAmount;
            var guess = GuessSingleFemaleAge(headSize);
            if(guess != null) {
                guess.Confidence = 1.0f / (float)Math.Pow(1 + Math.Abs(i), 1.4);
                // SuperController.LogMessage($"y={Math.Round(y, 3)} age={guess.Age} confidence={guess.Confidence}");
                response.Add(guess);
            }
        }

        if(response.Count > 0) {
            return new AgeCalculation(response);
        }
        return null;
    }

    private AgeCalculation Algorithm2(float y) {
        // SuperController.singleton.ClearMessages();
        var results = new List<AgeGuess>();

        // y is the actual proportion -- consider this the 50% percentile
        var pctBounding = 0.02f;

        // then create a range of 5% in either direction
        var min = y * (1-pctBounding);
        var median = y;
        var max = y * (1+pctBounding);

        // now calculate standard deviation of those ranges using the "range rule"
        var stddev = (max - min) / 4f;

        // calculate head/hight radios on the border of some percentile distributions
        var stepCount = 10;
        var step = (max - min) / stepCount;

        int yearAccumCurrent = -1;
        int yearAccumCounter = 0;
        var guess = new AgeGuess();
        for(var i = 0; i <= stepCount; i++) {
            var x1 = min + (step * i);
            var x2 = min + (step * (i+1));
            var p1 = ZscoreToPercentile(ZScore(x1, median, stddev));
            var p2 = ZscoreToPercentile(ZScore(x2, median, stddev));
            var age = (int)(HeadProportionToAge(x1) ?? 0);
            var confidence = (float)Math.Abs(p1 - p2);

            if(age != yearAccumCurrent) {

                if(yearAccumCounter != 0) {
                    // SuperController.LogMessage($"Logging year {guess.Age} with {yearAccumCounter} items averaged");
                    guess.Confidence /= yearAccumCounter;
                    results.Add(guess);
                }

                yearAccumCurrent = age;
                yearAccumCounter = 0;

                guess = new AgeGuess() {
                    Age = age,
                    Input = y,
                    Confidence = 0
                };
            }

            guess.Confidence += confidence;
            yearAccumCounter++;

            if(i == stepCount) {
                // SuperController.LogMessage($"Logging year {guess.Age} with 1 items averaged");
                results.Add(guess);
            }
        }

        // normalize responses
        if(results.Count > 0) {
            var maxConfidence = results.Max(x => x.Confidence);
            var minConfidence = 0;
            if(maxConfidence - minConfidence != 0) {
                for(var i = 0; i < results.Count; i++) {
                    var g = results[i];
                    // SuperController.LogMessage($"BEFORE Input={g.Input}, Age={g.Age}, Confidence={g.Confidence}");
                    results[i].Confidence = (results[i].Confidence - minConfidence)/(maxConfidence - minConfidence);
                    // SuperController.LogMessage($"AFTER Input={g.Input}, Age={g.Age}, Confidence={g.Confidence}");
                }
            }
            return new AgeCalculation(results);
        }
        else {
            return null;
        }
    }

    private AgeGuess GuessSingleFemaleAge(float y) {
        if(y > 0 && y <= 4.2) {
            return new AgeGuess() {
                Input = y,
                Age = 0,
                Confidence = 1
            };
        }
        else if(y > 8) {
            return new AgeGuess() {
                Input = y,
                Age = 25,
                Confidence = 1
            };
        }
        
        var x = HeadProportionToAge(y); if(x == null) {
            return null;
        }
        
        if(x < 0) {
            return null;
        }
        else if(x > 25) {
            return new AgeGuess() {
                Input = y,
                Age = 25,
                Confidence = 1
            };
        }
        else {
            return new AgeGuess() {
                Input = y,
                Age = (int)Math.Round(x.Value),
                Confidence = 1
            };
        }
    }

    private float? HeadProportionToAge(float proportion) {
        // this is a quadratic equation fitted to the scatterplot of the
        // head/height proportion from Anatomy4Sculpters
        // y = 4.26775 + 0.288619 x - 0.00557796 x^2
        // 
        float a = -0.00557796f;
        float b = 0.288619f;
        float c = 4.26775f - proportion;
        
        // (-b +- sqrt(b2 - 4ac)) / 2a
        double d = (b*b) - (4*a*c);
        if(d<=0) {
            return null;
        }
        
        return (float)((-b + Math.Sqrt(d))/(2*a));
    }

	private static double ZscoreToPercentile(double x)
	{
		var originalX = x;
		// constants
		double a1 = 0.254829592;
		double a2 = -0.284496736;
		double a3 = 1.421413741;
		double a4 = -1.453152027;
		double a5 = 1.061405429;
		double p = 0.3275911;
			
		// Save the sign of x
		int sign = 1;
		if (x < 0)
			sign = -1;
		x = Math.Abs(x) / Math.Sqrt(2.0);
			
		// A&S formula 7.1.26
		double t = 1.0 / (1.0 + p*x);
		double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t * Math.Exp(-x*x);
			
		var a = 0.5 * (1.0 + sign*y);
		return a;
	}

    private static double ZScore(double x, double mean, double stddev) {
        return (x - mean)/stddev;
    }

}