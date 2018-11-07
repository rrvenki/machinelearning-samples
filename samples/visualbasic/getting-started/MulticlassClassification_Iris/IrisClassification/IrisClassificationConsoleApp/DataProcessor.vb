Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports Microsoft.ML.Transforms.Categorical
Imports System
Imports Microsoft.ML.Transforms.Normalizers.NormalizingEstimator

Namespace MulticlassClassification_Iris
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

			DataProcessPipeline = mlContext.Transforms.Concatenate("Features", { "SepalLength", "SepalWidth", "PetalLength", "PetalWidth" })
		End Sub
	End Class
End Namespace


