Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports Microsoft.ML.Transforms.Categorical
Imports Microsoft.ML.Transforms.Text
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace SentimentAnalysisConsoleApp
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

			DataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("Text", "Features")
								  'Another way: new TextFeaturizingEstimator(mlContext, "Text", "Features");
										 'You can add additional transformations here with Appends()
										 '.Append(new YourSelectedEstimator(mlContext, YOUR_REQUIRED_PARAMETERS))                     
		End Sub
	End Class
End Namespace