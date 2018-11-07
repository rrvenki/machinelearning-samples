Imports System
Imports System.IO

Imports Microsoft.ML
Imports CustomerSegmentation.DataStructures


Namespace CustomerSegmentation
	Public Class Program
		Shared Sub Main(ByVal args() As String)
			Dim assetsPath = ModelHelpers.GetAssetsPath("..\..\..\assets")

			Dim transactionsCsv = Path.Combine(assetsPath, "inputs", "transactions.csv")
			Dim offersCsv = Path.Combine(assetsPath, "inputs", "offers.csv")
			Dim pivotCsv = Path.Combine(assetsPath, "inputs", "pivot.csv")
			Dim modelZip = Path.Combine(assetsPath, "outputs", "retailClustering.zip")

			Try
				'DataHelpers.PreProcessAndSave(offersCsv, transactionsCsv, pivotCsv);
				'var modelBuilder = new ModelBuilder(pivotCsv, modelZip, kValuesSvg);
				'modelBuilder.BuildAndTrain();

				'STEP 0: Special data pre-process in this sample creating the PivotTable csv file
				DataHelpers.PreProcessAndSave(offersCsv, transactionsCsv, pivotCsv)

				'Create the MLContext to share across components for deterministic results
				Dim mlContext As New MLContext(seed:= 1) 'Seed set to any number so you have a deterministic environment

				'STEP 1: Common data loading
				Dim dataLoader As New DataLoader(mlContext)
				Dim pivotDataView = dataLoader.GetDataView(pivotCsv)

				'STEP 2: Process data transformations in pipeline
				Dim dataProcessor = New DataProcessor(mlContext, 2)
				Dim dataProcessPipeline = dataProcessor.DataProcessPipeline

				' (Optional) Peek data in training DataView after applying the ProcessPipeline's transformations  
				Common.ConsoleHelper.PeekDataViewInConsole(Of PivotObservation)(mlContext, pivotDataView, dataProcessPipeline, 10)
				Common.ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", pivotDataView, dataProcessPipeline, 10)

				' STEP 3: Create and train the model                
				Dim trainer = mlContext.Clustering.Trainers.KMeans("Features", clustersCount:= 3)
				Dim modelBuilder = New Common.ModelBuilder(Of PivotObservation, ClusteringPrediction)(mlContext, dataProcessPipeline)
				modelBuilder.AddTrainer(trainer)
				Dim trainedModel = modelBuilder.Train(pivotDataView)

				' STEP4: Evaluate accuracy of the model
				Dim metrics = modelBuilder.EvaluateClusteringModel(pivotDataView)
				Common.ConsoleHelper.PrintClusteringMetrics(trainer.ToString(), metrics)

				' STEP5: Save/persist the model as a .ZIP file
				modelBuilder.SaveModelAsFile(modelZip)

			Catch ex As Exception
				Common.ConsoleHelper.ConsoleWriteException(ex.Message)
			End Try

			Common.ConsoleHelper.ConsolePressAnyKey()
		End Sub
	End Class
End Namespace
