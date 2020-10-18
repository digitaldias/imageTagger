namespace VisionClientApi
{
    public class ConfidentResult
    {
        public ConfidentResult()
        {
        }

        public ConfidentResult(string text, double confidence)
        {
            Text       = text;
            Confidence = confidence;
        }

        public double Confidence { get; set; }

        public double ConfidenceBorder
        {
            get
            {
                if (Confidence > 0.02)
                {
                    return Confidence - 0.02;
                }
                return Confidence;
            }
        }

        public string Text { get; set; }
    }
}