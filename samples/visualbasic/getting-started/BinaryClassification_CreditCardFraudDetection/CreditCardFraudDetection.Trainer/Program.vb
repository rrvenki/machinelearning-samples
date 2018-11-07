Imports CreditCardFraudDetection.Common
Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Trainers
Imports System.Linq
Imports System.IO
Imports Microsoft.ML.Runtime.Data.IO
Imports System

Namespace CreditCardFraudDetection.Trainer
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			Dim assetsPath = ConsoleHelpers.GetAssetsPath("..\..\..\assets")
			Dim zipDataSet = Path.Combine(assetsPath, "input", "creditcardfraud-dataset.zip")
			Dim dataSetFile = Path.Combine(assetsPath, "input", "creditcard.csv")

			Try
				ConsoleHelpers.UnZipDataSet(zipDataSet, dataSetFile)

				Dim modelBuilder = New ModelBuilder(assetsPath, dataSetFile)
				modelBuilder.Build()
				modelBuilder.TrainFastTreeAndSaveModels()
			Catch e As Exception
				ConsoleHelpers.ConsoleWriteException( { e.Message, e.StackTrace })
			End Try

			ConsoleHelpers.ConsolePressAnyKey()
		End Sub
	End Class
End Namespace
