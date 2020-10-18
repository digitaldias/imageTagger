using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisionClientApi
{
    public class VisionClient
    {
        private readonly ComputerVisionClient _client;

        public VisionClient(string subscriptionKey, string imageAnalysisEndpoint)
        {
            _client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = imageAnalysisEndpoint
            };
        }

        public async Task<AnalysisResult> AnalyzeImage(string imagePath)
        {
            var featuresToAnalyze = SelectFeaturesToAnalyze();

            using (var fileStream = File.OpenRead(imagePath))
            {
                var stopwatch = Stopwatch.StartNew();
                var analysis = await _client.AnalyzeImageInStreamAsync(fileStream, visualFeatures: featuresToAnalyze);

                return ConvertImageAnalysisToResult(analysis, stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task<AnalysisResult> AnalyzeImageBytes(byte[] imageBytes)
        {
            var featuresToAnalyze = SelectFeaturesToAnalyze();

            try
            {
                var stopwatch = Stopwatch.StartNew();
                using(var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    var analysis = await _client.AnalyzeImageInStreamAsync(memoryStream, featuresToAnalyze);                    
                    return ConvertImageAnalysisToResult(analysis, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                return new AnalysisResult
                {
                    Success = false,
                    StatusText = "Analysis failed: " + ex.Message
                };
            }  
        }

        static AnalysisResult ConvertImageAnalysisToResult(ImageAnalysis results, long elapsedMilliseconds)
        {
            return new AnalysisResult
            {
                Success    = true,
                StatusText = $"Image Analyzed in {elapsedMilliseconds:### ###.0} ms.",
                ElapsedMs  = elapsedMilliseconds,

                Captions = results.Description?.Captions
                                    .Select(r => new ConfidentResult(r.Text, r.Confidence))
                                    .ToList()
                                    ?? new List<ConfidentResult>(),

                Categories = results.Categories?
                                    .Select(c => new ConfidentResult(c.Name, c.Score))
                                    .ToList()
                                    ?? new List<ConfidentResult>(),

                Tags = results.Tags?
                                    .Where(t => t.Confidence >= 0.9)
                                    .Select(t => new ConfidentResult(t.Name, t.Confidence))
                                    .ToList()
                                    ?? new List<ConfidentResult>(),

                Brands = results.Brands?
                                    .Where(b => b.Confidence > 0.5)
                                    .Select(b => new ConfidentResult(b.Name, b.Confidence))
                                    .ToList()
                                    ?? new List<ConfidentResult>(),

                Objects = results.Objects?
                                    .Select(b => new ObjectResult(b.ObjectProperty, b.Confidence, 
                                        b.Rectangle.X, 
                                        b.Rectangle.Y, 
                                        b.Rectangle.W, 
                                        b.Rectangle.H))
                                    .ToList()
                                    ?? new List<ObjectResult>()
            };
        }

        static List<VisualFeatureTypes?> SelectFeaturesToAnalyze()
        {
            return new List<VisualFeatureTypes?>
            {
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects,
            };
        }
    }
}