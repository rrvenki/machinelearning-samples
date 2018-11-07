Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms.Categorical
Imports Microsoft.ML.Transforms.Text


Namespace GitHubLabeler
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

			DataProcessPipeline = (New ValueToKeyMappingEstimator(mlContext, "Area", "Label")).Append(mlContext.Transforms.Text.FeaturizeText("Title", "TitleFeaturized")).Append(mlContext.Transforms.Text.FeaturizeText("Description", "DescriptionFeaturized")).Append(mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized"))
		End Sub
	End Class
End Namespace

