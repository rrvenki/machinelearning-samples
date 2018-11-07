Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Clustering_Iris
	Friend Class DataLoader
		Private _mlContext As MLContext
		Private _loader As TextLoader

		Public Sub New(ByVal mlContext As MLContext)
			_mlContext = mlContext

			' Create the TextLoader by defining the data columns and where to find (column position) them in the text file.
			_loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = vbTab,
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("Label", DataKind.R4, 0),
					New TextLoader.Column("SepalLength", DataKind.R4, 1),
					New TextLoader.Column("SepalWidth", DataKind.R4, 2),
					New TextLoader.Column("PetalLength", DataKind.R4, 3),
					New TextLoader.Column("PetalWidth", DataKind.R4, 4)
				}
			})
		End Sub

		Public Function GetDataView(ByVal filePath As String) As IDataView
			Return _loader.Read(filePath)
		End Function
	End Class
End Namespace

