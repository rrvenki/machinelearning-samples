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
				'Unzip datasets as they are significantly large, too large for GitHub if not zipped
				ConsoleHelpers.UnZipDataSet(zipDataSet, dataSetFile)

				' Create a common ML.NET context.
				' Seed set to any number so you have a deterministic environment for repeateable results
				Dim mlContext As New MLContext(seed:=1)

				Dim modelBuilder = New ModelBuilder(mlContext, assetsPath, dataSetFile)
				modelBuilder.PreProcessData(mlContext)

				ConsoleHelpers.ConsoleWriteHeader("Creating and training the model")
				modelBuilder.TrainFastTreeAndSaveModels()
			Catch e As Exception
				ConsoleHelpers.ConsoleWriteException( { e.Message, e.StackTrace })
			End Try

			ConsoleHelpers.ConsolePressAnyKey()
		End Sub
	End Class
End Namespace
