using System;

namespace AzureCustomVisionDemo
{
    public class ClassificationResult
    {
        public string Tag { get; private set; }
        public double Confidence { get; private set; }

        public ClassificationResult(string tag, double confidence)
        {
            Tag = tag;
            Confidence = Math.Round(confidence * 100, 2);
        }
    }
}