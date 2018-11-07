Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace GitHubLabeler
	Friend Class DataLoader
		Private _mlContext As MLContext
		Private _loader As TextLoader

		Public Sub New(ByVal mlContext As MLContext)
			_mlContext = mlContext

			_loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = "tab",
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("ID", DataKind.Text, 0),
					New TextLoader.Column("Area", DataKind.Text, 1),
					New TextLoader.Column("Title", DataKind.Text, 2),
					New TextLoader.Column("Description", DataKind.Text, 3)
				}
			})
		End Sub

		Public Function GetDataView(ByVal filePath As String) As IDataView
			Return _loader.Read(filePath)
		End Function
	End Class
End Namespace
