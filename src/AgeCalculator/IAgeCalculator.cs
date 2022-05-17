using System.Collections.Generic;

public interface IAgeCalculator {
    AgeCalculation GuessFemaleAges(float y);
    AgeCalculation GuessMaleAges(float y);
}