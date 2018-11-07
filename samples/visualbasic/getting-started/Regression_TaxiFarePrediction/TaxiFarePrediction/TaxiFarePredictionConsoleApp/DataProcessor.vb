Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports Microsoft.ML.Transforms.Categorical
Imports System
Imports Microsoft.ML.Transforms.Normalizers.NormalizingEstimator

Namespace Regression_TaxiFarePrediction
	Public Class DataProcessor
		Private privateDataProcessPipeline As IEstimator(Of ITransformer)
		Public Property DataProcessPipeline() As IEstimator(Of ITransformer)
			Get
				Return privateDataProcessPipeline
			End Get
			Private Set(ByVal value As IEstimator(Of ITransformer))
				privateDataProcessPipeline = value
			End Set
		End Property

		Public Sub New(ByVal mlContext As MLContext)
			' Configure data transformations in the Process pipeline
			' In our case, we will one-hot encode as categorical values the VendorId, RateCode and PaymentType
			' Then concatenate that with the numeric columns.

			DataProcessPipeline = (New CopyColumnsEstimator(mlContext, "FareAmount", "Label")).Append(mlContext.Transforms.Categorical.OneHotEncoding("VendorId", "VendorIdEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("RateCode", "RateCodeEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("PaymentType", "PaymentTypeEncoded")).Append(mlContext.Transforms.Normalize(inputName:= "PassengerCount", mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Normalize(inputName:= "TripTime", mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Normalize(inputName:= "TripDistance", mode:=NormalizerMode.MeanVariance)).Append(New ColumnConcatenatingEstimator(mlContext, "Features", "VendorIdEncoded", "RateCodeEncoded", "PaymentTypeEncoded", "PassengerCount", "TripTime", "TripDistance"))

		End Sub
	End Class
End Namespace


