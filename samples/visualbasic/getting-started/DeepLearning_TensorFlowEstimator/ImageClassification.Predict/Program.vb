Imports System
Imports System.IO
Imports System.Threading.Tasks
Imports ImageClassification.Model
Imports ImageClassification.Model.ConsoleHelpers

Namespace ImageClassification.Predict
	Friend Class Program
		Shared Async Function Main(ByVal args() As String) As Task
			Dim assetsPath = ModelHelpers.GetAssetsPath("..\..\..\assets")

			Dim tagsTsv = Path.Combine(assetsPath, "inputs", "data", "tags.tsv")
			Dim imagesFolder = Path.Combine(assetsPath, "inputs", "data")
			Dim imageClassifierZip = Path.Combine(assetsPath, "inputs", "imageClassifier.zip")

			Try
				Dim modelEvaluator = New ModelEvaluator(tagsTsv, imagesFolder, imageClassifierZip)
				modelEvaluator.EvaluateStaticApi()
			Catch ex As Exception
				ConsoleWriteException(ex.Message)
			End Try

			ConsolePressAnyKey()
		End Function
	End Class
End Namespace
