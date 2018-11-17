Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace CustomerSegmentation
	Public Module CustomerSegmentationTextLoaderFactory
		Public Function CreateTextLoader(ByVal mlContext As MLContext) As TextLoader
			Dim textLoader As TextLoader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = ",",
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("Features", DataKind.R4, New TextLoader.Range() {New TextLoader.Range(0, 31) }),
					New TextLoader.Column("LastName", DataKind.Text, 32)
				}
			})

			Return textLoader
		End Function
	End Module
End Namespace
