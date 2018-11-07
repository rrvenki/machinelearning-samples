Imports System
Imports System.IO
Imports Microsoft.ML.Runtime.Learners
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML
Imports Microsoft.ML.Trainers

Imports SentimentAnalysisConsoleApp.DataStructures
Imports Microsoft.ML.Transforms.Text

Namespace SentimentAnalysisConsoleApp

	Friend Module Program
		Private ReadOnly Property AppPath() As String
			Get
				Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
			End Get
		End Property

		Private BaseDatasetsLocation As String = "../../../../Data"
		Private TrainDataPath As String = $"{BaseDatasetsLocation}/wikipedia-detox-250-line-data.tsv"
		Private TestDataPath As String = $"{BaseDatasetsLocation}/wikipedia-detox-250-line-test.tsv"

		Private BaseModelsPath As String = "../../../../MLModels"
		Private ModelPath As String = $"{BaseModelsPath}/SentimentModel.zip"

		Sub Main(ByVal args() As String)
			'Create MLContext to be shared across the model creation workflow objects 
			'Set a random seed for repeatable/deterministic results across multiple trainings.
			Dim mlContext = New MLContext()

			' STEP 1: Common data loading configuration
			Dim dataLoader As New DataLoader(mlContext)
			Dim trainingDataView = dataLoader.GetDataView(TrainDataPath)
			Dim testDataView = dataLoader.GetDataView(TestDataPath)

			' STEP 2: Common data process configuration with pipeline data transformations
			Dim dataProcessor = New DataProcessor(mlContext)
			Dim dataProcessPipeline = dataProcessor.DataProcessPipeline

			' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
			Common.ConsoleHelper.PeekDataViewInConsole(Of SentimentIssue)(mlContext, trainingDataView, dataProcessPipeline, 2)
			'Common.ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 2);

			' STEP 3: Set the training algorithm, then create and config the modelBuilder                            
			Dim modelBuilder = New Common.ModelBuilder(Of SentimentIssue, SentimentPrediction)(mlContext, dataProcessPipeline)
			Dim trainer = mlContext.BinaryClassification.Trainers.FastTree(label:= "Label", features:= "Features")
			modelBuilder.AddTrainer(trainer)

			' STEP 4: Train the model fitting to the DataSet
			Console.WriteLine("=============== Training the model ===============")
			modelBuilder.Train(trainingDataView)

			' STEP 5: Evaluate the model and show accuracy stats
			Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
			Dim metrics = modelBuilder.EvaluateBinaryClassificationModel(testDataView, "Label", "Score")
			Common.ConsoleHelper.PrintBinaryClassificationMetrics(trainer.ToString(), metrics)

			' STEP 6: Save/persist the trained model to a .ZIP file
			Console.WriteLine("=============== Saving the model to a file ===============")
			modelBuilder.SaveModelAsFile(ModelPath)

			' (OPTIONAL) Try/test a single prediction by loding the model from the file, first.
			Dim sampleStatement As SentimentIssue = New SentimentIssue With {.Text = "This is a very rude movie"}
			Dim modelScorer = New Common.ModelScorer(Of SentimentIssue, SentimentPrediction)(mlContext)
			modelScorer.LoadModelFromZipFile(ModelPath)
			Dim resultprediction = modelScorer.PredictSingle(sampleStatement)

			Console.WriteLine($"=============== Single Prediction  ===============")
			Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(If(Convert.ToBoolean(resultprediction.Prediction), "Toxic", "Nice"))} sentiment | Probability: {resultprediction.Probability} ")
			Console.WriteLine($"==================================================")
			'

			Common.ConsoleHelper.ConsoleWriteHeader("=============== End of training process, hit any key to finish ===============")
			Console.ReadKey()

		End Sub
	End Module
End Namespace