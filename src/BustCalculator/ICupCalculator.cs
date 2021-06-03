namespace LFE {
    public interface ICupCalculator {
        string Name { get; }
        CupSize Calculate(float bust, float underbust);
    }
}