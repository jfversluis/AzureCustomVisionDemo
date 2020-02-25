// model.cs
//
// This file was automatically generated and should not be edited.
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using CoreML;
using CoreVideo;
using Foundation;

namespace AzureCustomVisionDemo.iOS {
	/// <summary>
	/// Model Prediction Input Type
	/// </summary>
	public class modelInput : NSObject, IMLFeatureProvider
	{
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("data")
		);

		CVPixelBuffer data;

		/// <summary>
		///  as color (kCVPixelFormatType_32BGRA) image buffer, 224 pizels wide by 224 pixels high
		/// </summary>
		/// <value></value>
		public CVPixelBuffer Data {
			get { return data; }
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				data = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			switch (featureName) {
			case "data":
				return MLFeatureValue.Create (Data);
			default:
				return null;
			}
		}

		public modelInput (CVPixelBuffer data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			Data = data;
		}
	}

	/// <summary>
	/// Model Prediction Output Type
	/// </summary>
	public class modelOutput : NSObject, IMLFeatureProvider
	{
		static readonly NSSet<NSString> featureNames = new NSSet<NSString> (
			new NSString ("loss"), new NSString ("classLabel")
		);

		NSDictionary<NSObject, NSNumber> loss;
		string classLabel;

		/// <summary>
		///  as dictionary of strings to doubles
		/// </summary>
		/// <value></value>
		public NSDictionary<NSObject, NSNumber> Loss {
			get { return loss; }
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				loss = value;
			}
		}

		/// <summary>
		///  as string value
		/// </summary>
		/// <value></value>
		public string ClassLabel {
			get { return classLabel; }
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				classLabel = value;
			}
		}

		public NSSet<NSString> FeatureNames {
			get { return featureNames; }
		}

		public MLFeatureValue GetFeatureValue (string featureName)
		{
			MLFeatureValue value;
			NSError err;

			switch (featureName) {
			case "loss":
				value = MLFeatureValue.Create (Loss, out err);
				if (err != null)
					err.Dispose ();
				return value;
			case "classLabel":
				return MLFeatureValue.Create (ClassLabel);
			default:
				return null;
			}
		}

		public modelOutput (NSDictionary<NSObject, NSNumber> loss, string classLabel)
		{
			if (loss == null)
				throw new ArgumentNullException (nameof (loss));

			if (classLabel == null)
				throw new ArgumentNullException (nameof (classLabel));

			Loss = loss;
			ClassLabel = classLabel;
		}
	}

	/// <summary>
	/// Class for model loading and prediction
	/// </summary>
	public class model : NSObject
	{
		readonly MLModel _model;

		static NSUrl GetModelUrl ()
		{
			return NSBundle.MainBundle.GetUrlForResource ("model", "mlmodelc");
		}

		public model ()
		{
			NSError err;

			_model = MLModel.Create (GetModelUrl (), out err);
		}

		model (MLModel model)
		{
			this._model = model;
		}

		public static model Create (NSUrl url, out NSError error)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			var _model = MLModel.Create (url, out error);

			if (_model == null)
				return null;

			return new model (_model);
		}

		public static model Create (MLModelConfiguration configuration, out NSError error)
		{
			if (configuration == null)
				throw new ArgumentNullException (nameof (configuration));

			var _model = MLModel.Create (GetModelUrl (), configuration, out error);

			if (_model == null)
				return null;

			return new model (_model);
		}

		public static model Create (NSUrl url, MLModelConfiguration configuration, out NSError error)
		{
			if (url == null)
				throw new ArgumentNullException (nameof (url));

			if (configuration == null)
				throw new ArgumentNullException (nameof (configuration));

			var model = MLModel.Create (url, configuration, out error);

			if (model == null)
				return null;

			return new model (model);
		}

		/// <summary>
		/// Make a prediction using the standard interface
		/// </summary>
		/// <param name="input">an instance of modelInput to predict from</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public modelOutput GetPrediction (modelInput input, out NSError error)
		{
			if (input == null)
				throw new ArgumentNullException (nameof (input));

			var prediction = _model.GetPrediction (input, out error);

			if (prediction == null)
				return null;

			var lossValue = prediction.GetFeatureValue ("loss").DictionaryValue;
			var classLabelValue = prediction.GetFeatureValue ("classLabel").StringValue;

			return new modelOutput (lossValue, classLabelValue);
		}

		/// <summary>
		/// Make a prediction using the standard interface
		/// </summary>
		/// <param name="input">an instance of modelInput to predict from</param>
		/// <param name="options">prediction options</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public modelOutput GetPrediction (modelInput input, MLPredictionOptions options, out NSError error)
		{
			if (input == null)
				throw new ArgumentNullException (nameof (input));

			if (options == null)
				throw new ArgumentNullException (nameof (options));

			var prediction = _model.GetPrediction (input, options, out error);

			if (prediction == null)
				return null;

			var lossValue = prediction.GetFeatureValue ("loss").DictionaryValue;
			var classLabelValue = prediction.GetFeatureValue ("classLabel").StringValue;

			return new modelOutput (lossValue, classLabelValue);
		}

		/// <summary>
		/// Make a prediction using the convenience interface
		/// </summary>
		/// <param name="data"> as color (kCVPixelFormatType_32BGRA) image buffer, 224 pizels wide by 224 pixels high</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public modelOutput GetPrediction (CVPixelBuffer data, out NSError error)
		{
			var input = new modelInput (data);

			return GetPrediction (input, out error);
		}

		/// <summary>
		/// Make a prediction using the convenience interface
		/// </summary>
		/// <param name="data"> as color (kCVPixelFormatType_32BGRA) image buffer, 224 pizels wide by 224 pixels high</param>
		/// <param name="options">prediction options</param>
		/// <param name="error">If an error occurs, upon return contains an NSError object that describes the problem.</param>
		public modelOutput GetPrediction (CVPixelBuffer data, MLPredictionOptions options, out NSError error)
		{
			var input = new modelInput (data);

			return GetPrediction (input, options, out error);
		}
	}
}
