Imports System.IO

Imports Microsoft.ML
Imports TitanicSurvivalConsoleApp.DataStructures

Namespace TitanicSurvivalConsoleApp

    Friend Module Program
        Private ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsLocation As String = "../../../../Data"
        Private TrainDataPath As String = $"{BaseDatasetsLocation}/titanic-train.csv"
        Private TestDataPath As String = $"{BaseDatasetsLocation}/titanic-test.csv"

        Private BaseModelsPath As String = "../../../../MLModels"
        Private ModelPath As String = $"{BaseModelsPath}/TitanicModel.zip"

        Public Sub Main(args() As String)
            'Create ML Context with seed for repeteable/deterministic results
            Dim mlContext As New MLContext(seed:=0)

            ' STEP 1: Create/Train, Evaluate and save the model
            CreateTrainAndEvaluateModel(mlContext)

            ' STEP 2: Make a single test prediction
            TestSinglePrediction(mlContext)

            Common.ConsolePressAnyKey()
        End Sub

        Public Sub CreateTrainAndEvaluateModel(mlContext As MLContext)
            ' STEP 1: Common data loading configuration
            Dim dataLoader As New DataLoader(mlContext)
            Dim trainingDataView = dataLoader.GetDataView(TrainDataPath)
            Dim testDataView = dataLoader.GetDataView(TestDataPath)

            ' STEP 2: Common data process configuration with pipeline data transformations
            Dim dataProcessor = New DataProcessor(mlContext)
            Dim dataProcessPipeline = dataProcessor.DataProcessPipeline

            ' (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            Common.PeekDataViewInConsole(Of TitanicData)(mlContext, trainingDataView, dataProcessPipeline, 2)
            Common.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 2)

            ' STEP 3: Set the training algorithm, then create and config the modelBuilder    
            ' FastTreeBinaryClassifier is an algorithm that will be used to train the model.
            ' It has three hyperparameters for tuning decision tree performance. 
            'pipeline.Add(new FastTreeBinaryClassifier());// {NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2});
            Dim modelBuilder = New Common.ModelBuilder(Of TitanicData, TitanicPrediction)(mlContext, dataProcessPipeline)
            Dim trainer = mlContext.BinaryClassification.Trainers.FastTree(label:="Label", features:="Features", numLeaves:=10, numTrees:=5, minDatapointsInLeafs:=10)
            modelBuilder.AddTrainer(trainer)

            ' STEP 4: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============")
            modelBuilder.Train(trainingDataView)

            ' STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====")
            Dim metrics = modelBuilder.EvaluateBinaryClassificationModel(testDataView, "Label", "Score")
            Common.PrintBinaryClassificationMetrics(trainer.ToString(), metrics)

            ' STEP 6: Save/persist the trained model to a .ZIP file
            Console.WriteLine("=============== Saving the model to a file ===============")
            modelBuilder.SaveModelAsFile(ModelPath)
        End Sub


        Private Sub TestSinglePrediction(mlContext As MLContext)
            ' (OPTIONAL) Try/test a single prediction by loding the model from the file, first.
            Dim sampleTitanicPassengerData As TitanicData = TestTitanicData.Passenger

            Dim modelScorer = New Common.ModelScorer(Of TitanicData, TitanicPrediction)(mlContext)
            modelScorer.LoadModelFromZipFile(ModelPath)
            Dim resultprediction = modelScorer.PredictSingle(sampleTitanicPassengerData)

            Console.WriteLine($"=============== Single Prediction  ===============")
            Console.WriteLine($" Did this passenger survive?   Actual: Yes   Predicted: {(If(resultprediction.Survived, "Yes", "No"))} with {resultprediction.Probability * 100}% probability")
            Console.WriteLine($"==================================================")

        End Sub

    End Module
End Namespace