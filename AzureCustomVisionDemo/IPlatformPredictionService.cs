using System.IO;
using System.Threading.Tasks;

namespace AzureCustomVisionDemo
{
    public interface IPlatformPredictionService
    {
        Task<ClassificationResult> Classify(Stream imageStream);
    }
}