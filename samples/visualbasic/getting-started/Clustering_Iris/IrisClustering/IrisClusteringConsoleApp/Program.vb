Imports System
Imports System.IO

Imports Microsoft.ML
Imports Common
Imports Clustering_Iris.DataStructures
Imports Microsoft.ML.Runtime.Data

Namespace Clustering_Iris
    Friend Module Program
        Private ReadOnly Property AppPath() As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private BaseDatasetsLocation As String = "../../../../Data"
        Private DataPath As String = $"{BaseDatasetsLocation}/iris-full.txt"

        Private BaseModelsPath As String = "../../../../MLModels"
        Private ModelPath As String = $"{BaseModelsPath}/IrisModel.zip"

        Public Sub Main(ByVal args() As String)
            'Create the MLContext to share across components for deterministic results
            Dim mlContext As New MLContext(seed:=1) 'Seed set to any number so you have a deterministic environment

            'STEP 1: Common data loading
            Dim dataLoader As New DataLoader(mlContext)
            Dim fullData = dataLoader.GetDataView(DataPath)

            'INSTANT VB TODO TASK: VB has no equivalent to C# deconstruction declarations:
            With mlContext.Clustering.TrainTestSplit(fullData, testFraction:=0.2)
                'STEP 2: Process data transformations in pipeline
                Dim dataProcessor = New DataProcessor(mlContext)
                Dim dataProcessPipeline = dataProcessor.DataProcessPipeline

                ' (Optional) Peek data in training DataView after applying the ProcessPipeline's transformations  
                PeekDataViewInConsole(Of IrisData)(mlContext, .trainSet, dataProcessPipeline, 10)
                PeekVectorColumnDataInConsole(mlContext, "Features", .trainSet, dataProcessPipeline, 10)

                ' STEP 3: Create and train the model                
                Dim modelBuilder = New ModelBuilder(Of IrisData, IrisPrediction)(mlContext, dataProcessPipeline)
                Dim trainer = mlContext.Clustering.Trainers.KMeans(features:="Features", clustersCount:=3)
                modelBuilder.AddTrainer(trainer)
                Dim trainedModel = modelBuilder.Train(.trainSet)

                ' STEP4: Evaluate accuracy of the model
                Dim metrics = modelBuilder.EvaluateClusteringModel(.testSet)
                PrintClusteringMetrics(trainer.ToString(), metrics)

                ' STEP5: Save/persist the model as a .ZIP file
                modelBuilder.SaveModelAsFile(ModelPath)
            End With

            Console.WriteLine("=============== End of training process ===============")

            Console.WriteLine("=============== Predict a cluster for a single case (Single Iris data sample) ===============")

            ' Test with one sample text 
            Dim sampleIrisData = New IrisData() With {
                .SepalLength = 3.3F,
                .SepalWidth = 1.6F,
                .PetalLength = 0.2F,
                .PetalWidth = 5.1F
            }

            'Create the clusters: Create data files and plot a chart
            Dim modelScorer = New ModelScorer(Of IrisData, IrisPrediction)(mlContext)
            modelScorer.LoadModelFromZipFile(ModelPath)

            Dim prediction = modelScorer.PredictSingle(sampleIrisData)

            Console.WriteLine($"Cluster assigned for setosa flowers:" & prediction.SelectedClusterId)

            Console.WriteLine("=============== End of process, hit any key to finish ===============")
            Console.ReadKey()
        End Sub
    End Module
End Namespace
