Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports Microsoft.ML.Transforms.Categorical
Imports Microsoft.ML.Transforms.PCA
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace CustomerSegmentation
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

		Public Sub New(ByVal mlContext As MLContext, Optional ByVal rank As Integer = 2)
			' Configure data transformations in the DataProcess pipeline
			DataProcessPipeline = (New PrincipalComponentAnalysisEstimator(mlContext, "Features", "PCAFeatures", rank:= rank)).Append(New OneHotEncodingEstimator(mlContext, { New OneHotEncodingEstimator.ColumnInfo("LastName", "LastNameKey", CategoricalTransform.OutputKind.Ind) }))
		End Sub
	End Class
End Namespace
