namespace Client;

internal class BolaParams(double p, int[] s, double qMax, double[] utilities, double gamma, double length)
{
    public double P { get; } = p;
    public int[] S { get; } = s;
    public double QMax { get; } = qMax;
    public double[] Utilities { get; } = utilities;
    public double Gamma { get; } = gamma;
    public double Length { get; } = length;
}