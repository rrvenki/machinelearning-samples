Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace CustomerSegmentation
	Friend Class DataLoader
		Private _mlContext As MLContext
		Private _loader As TextLoader

		Public Sub New(ByVal mlContext As MLContext)
			_mlContext = mlContext

			' Create the TextLoader by defining the data columns and where to find (column position) them in the text file.
			_loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = ",",
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("Features", DataKind.R4, New TextLoader.Range() {New TextLoader.Range(0, 31) }),
					New TextLoader.Column("LastName", DataKind.Text, 32)
				}
			})
		End Sub

		Public Function GetDataView(ByVal filePath As String) As IDataView
			Return _loader.Read(filePath)
		End Function
	End Class
End Namespace
