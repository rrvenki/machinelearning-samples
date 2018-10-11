Imports Microsoft.ML.Legacy
Imports Microsoft.ML.Legacy.Data
Imports Microsoft.ML.Legacy.Models
Imports TensorFlowMLNETInceptionv3ModelScoring.ImageData

Namespace Model
    Public Class ModelEvaluator
        Private ReadOnly _dataLocation As String
        Private ReadOnly _imagesFolder As String
        Private ReadOnly _modelLocation As String

        Public Sub New(dataLocation As String, imagesFolder As String, modelLocation As String)
            _dataLocation = dataLocation
            _imagesFolder = imagesFolder
            _modelLocation = modelLocation
        End Sub

        Public Async Function Evaluate() As Task
            ' Initialize TensorFlow engine (Needed before loading the ML.NET related to TensorFlow model. This won't be needed when using the new API in v0.6 with Estimators, etc.)
            ' TensorFlowUtils.Initialize()

            Dim model = Await PredictionModel.ReadAsync(Of ImageNetData, ImageNetPrediction)(_modelLocation)

            ' Get Predictions
            Dim predictions = GetPredictions(_dataLocation, _imagesFolder, model).ToArray()
            ShowMetrics(_dataLocation, model)
        End Function

        Protected Iterator Function GetPredictions(testLocation As String, imagesFolder As String,
                                                   model As PredictionModel(Of ImageNetData, ImageNetPrediction)
                                                   ) As IEnumerable(Of ImageNetPrediction)
            Dim testData = ImageNetData.ReadFromCsv(testLocation, imagesFolder)

            For Each sample In testData
                Yield model.Predict(sample)
            Next sample
        End Function

        Protected Sub ShowMetrics(testLocation As String, model As PredictionModel(Of ImageNetData, ImageNetPrediction))
            Dim evaluator As New ClassificationEvaluator
            Dim testDataSource = New TextLoader(testLocation).CreateFrom(Of ImageNetData)()
            Dim metrics As ClassificationMetrics = evaluator.Evaluate(model, testDataSource)
            PrintMetrics(metrics)
        End Sub

        Protected Shared Sub PrintMetrics(metrics As ClassificationMetrics)
            Console.WriteLine($"**************************************************************")
            Console.WriteLine($"*       Metrics for Image Classification          ")
            Console.WriteLine($"*-------------------------------------------------------------")
            Console.WriteLine($"*       Log Loss: {metrics.LogLoss:0.##}")
            Console.WriteLine($"**************************************************************")
        End Sub
    End Class
End Namespace
