namespace VisionClientApi
{
    public class ObjectResult : ConfidentResult
    {
        public ObjectResult(string text, double confidence, int x, int y, int w, int h)
            : base(text, confidence)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public int X { get; }
        public int Y { get; }
        public int W { get; }
        public int H { get; }
    }
}
