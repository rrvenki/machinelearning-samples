Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Clustering_Iris
	Public Module IrisTextLoaderFactory
		Public Function CreateTextLoader(ByVal mlContext As MLContext) As TextLoader
			Dim textLoader As TextLoader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
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
			Return textLoader
		End Function
	End Module
End Namespace

