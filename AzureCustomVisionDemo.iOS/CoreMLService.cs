using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureCustomVisionDemo.iOS;
using CoreGraphics;
using CoreML;
using CoreVideo;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(CoreMLService))]
namespace AzureCustomVisionDemo.iOS
{
    public class CoreMLService : IPlatformPredictionService
    {
        NSUrl _assetPath = NSBundle.MainBundle.GetUrlForResource("model", "mlmodelc");
        MLModel _model;

        public CoreMLService()
        {
            _model = MLModel.Create(_assetPath, out NSError error1);
        }

        public Task<ClassificationResult> Classify(Stream imageStream)
        {
            var imageData = NSData.FromStream(imageStream);
            var uiImage = UIImage.LoadFromData(imageData);
			var pixelBuffer = uiImage.Scale(new CGSize(224, 224)).ToCVPixelBuffer();

            var result = _model.GetPrediction(new modelInput(pixelBuffer), out NSError error);
			var classification = result.GetFeatureValue("classLabel").StringValue;
			var loss = result.GetFeatureValue("loss").DictionaryValue;
			var confidence = (NSNumber)loss.ValueForKey((NSString)classification);
			
			return Task.FromResult(new ClassificationResult(classification, confidence.DoubleValue));
        }
    }

	public static class UIImageExtensions
	{
		public static CVPixelBuffer ToCVPixelBuffer(this UIImage self)
		{
			var attrs = new CVPixelBufferAttributes();
			attrs.CGImageCompatibility = true;
			attrs.CGBitmapContextCompatibility = true;

			var cgImg = self.CGImage;

			var pb = new CVPixelBuffer(cgImg.Width, cgImg.Height, CVPixelFormatType.CV32ARGB, attrs);
			pb.Lock(CVPixelBufferLock.None);
			var pData = pb.BaseAddress;
			var colorSpace = CGColorSpace.CreateDeviceRGB();
			var ctxt = new CGBitmapContext(pData, cgImg.Width, cgImg.Height, 8, pb.BytesPerRow, colorSpace, CGImageAlphaInfo.NoneSkipFirst);
			ctxt.TranslateCTM(0, cgImg.Height);
			ctxt.ScaleCTM(1.0f, -1.0f);
			UIGraphics.PushContext(ctxt);
			self.Draw(new CGRect(0, 0, cgImg.Width, cgImg.Height));
			UIGraphics.PopContext();
			pb.Unlock(CVPixelBufferLock.None);

			return pb;
		}
	}
}