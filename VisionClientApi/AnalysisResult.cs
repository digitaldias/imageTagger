using System.Collections.Generic;

namespace VisionClientApi
{
    public class AnalysisResult
    {
        public long ElapsedMs { get; set; }
        public string StatusText { get; set; }

        public bool Success { get; set; }

        public List<ConfidentResult> Brands { get; set; }
        public List<ConfidentResult> Captions { get; set; }
        public List<ConfidentResult> Categories { get;  set; }
        public List<ObjectResult> Objects { get; set; }
        public List<ConfidentResult> Tags { get; set; }
    }
}