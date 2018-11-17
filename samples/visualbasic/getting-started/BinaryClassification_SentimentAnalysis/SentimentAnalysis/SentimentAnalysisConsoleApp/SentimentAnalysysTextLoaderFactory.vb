Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data

Namespace SentimentAnalysisConsoleApp
	Public Module SentimentAnalysysTextLoaderFactory
		Public Function CreateTextLoader(mlContext As MLContext) As TextLoader
			Dim textLoader As TextLoader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = "tab",
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("Label", DataKind.Bool, 0),
					New TextLoader.Column("Text", DataKind.Text, 1)
				}
			})
			Return textLoader
		End Function
	End Module
End Namespace
