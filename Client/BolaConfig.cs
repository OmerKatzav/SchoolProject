namespace Client
{
    internal class BolaConfig(double maxBufferSize, double minBufferSize, double vMinCoef, double vMaxCoef)
    {
        public double MaxBufferSize { get; set; } = maxBufferSize;
        public double MinBufferSize { get; set; } = minBufferSize;
        public double VMinCoef { get; set; } = vMinCoef;
        public double VMaxCoef { get; set; } = vMaxCoef;
    }
}
