Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data


Namespace SentimentAnalysisConsoleApp
	Friend Class DataLoader
		Private _mlContext As MLContext
		Private _loader As TextLoader

		Public Sub New(mlContext As MLContext)
			_mlContext = mlContext

			_loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = "tab",
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("Label", DataKind.Bool, 0),
					New TextLoader.Column("Text", DataKind.Text, 1)
				}
			})
		End Sub

		Public Function GetDataView(filePath As String) As IDataView
			Return _loader.Read(filePath)
		End Function
	End Class
End Namespace
