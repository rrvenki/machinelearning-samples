Imports BikeSharingDemand.BikeSharingDemandData
Imports BikeSharingDemand.Helpers
Imports BikeSharingDemand.Model
Imports Microsoft.ML
Imports Microsoft.ML.Models
Imports Microsoft.ML.Trainers

Module Program
    Sub Main(args As String())
        Dim trainingDataLocation = "Data/hour_train.csv"
        Dim testDataLocation = "Data/hour_test.csv"

        Dim modelEvaluator As New ModelEvaluator

        Dim fastTreeModel = New ModelBuilder(trainingDataLocation, New FastTreeRegressor).BuildAndTrain()
        Dim fastTreeMetrics = modelEvaluator.Evaluate(fastTreeModel, testDataLocation)
        PrintMetrics("Fast Tree", fastTreeMetrics)

        Dim fastForestModel = New ModelBuilder(trainingDataLocation, New FastForestRegressor).BuildAndTrain()
        Dim fastForestMetrics = modelEvaluator.Evaluate(fastForestModel, testDataLocation)
        PrintMetrics("Fast Forest", fastForestMetrics)

        Dim poissonModel = New ModelBuilder(trainingDataLocation, New PoissonRegressor).BuildAndTrain()
        Dim poissonMetrics = modelEvaluator.Evaluate(poissonModel, testDataLocation)
        PrintMetrics("Poisson", poissonMetrics)

        Dim gradientDescentModel = New ModelBuilder(trainingDataLocation, New OnlineGradientDescentRegressor).BuildAndTrain()
        Dim gradientDescentMetrics = modelEvaluator.Evaluate(gradientDescentModel, testDataLocation)
        PrintMetrics("Online Gradient Descent", gradientDescentMetrics)

        Dim fastTreeTweedieModel = New ModelBuilder(trainingDataLocation, New FastTreeTweedieRegressor).BuildAndTrain()
        Dim fastTreeTweedieMetrics = modelEvaluator.Evaluate(fastTreeTweedieModel, testDataLocation)
        PrintMetrics("Fast Tree Tweedie", fastTreeTweedieMetrics)

        Dim additiveModel = New ModelBuilder(trainingDataLocation, New GeneralizedAdditiveModelRegressor).BuildAndTrain()
        Dim additiveMetrics = modelEvaluator.Evaluate(additiveModel, testDataLocation)
        PrintMetrics("Generalized Additive Model", additiveMetrics)

        Dim stochasticDualCorordinateAscentModel = New ModelBuilder(trainingDataLocation, New StochasticDualCoordinateAscentRegressor).BuildAndTrain()
        Dim stochasticDualCorordinateAscentMetrics = modelEvaluator.Evaluate(stochasticDualCorordinateAscentModel, testDataLocation)
        PrintMetrics("Stochastic Dual Coordinate Ascent", stochasticDualCorordinateAscentMetrics)

        VisualizeTenPredictionsForTheModel(fastTreeTweedieModel, testDataLocation)
        fastTreeTweedieModel.WriteAsync(".\Model.zip").Wait()

        Console.ReadLine()
    End Sub

    Private Sub PrintMetrics(name As String, metrics As RegressionMetrics)
        Console.WriteLine($"*************************************************")
        Console.WriteLine($"*       Metrics for {name}          ")
        Console.WriteLine($"*------------------------------------------------")
        Console.WriteLine($"*       R2 Score: {metrics.RSquared:0.##}")
        Console.WriteLine($"*       Absolute loss: {metrics.L1:#.##}")
        Console.WriteLine($"*       Squared loss: {metrics.L2:#.##}")
        Console.WriteLine($"*       RMS loss: {metrics.Rms:#.##}")
        Console.WriteLine($"*************************************************")
    End Sub

    Private Sub VisualizeTenPredictionsForTheModel(model As PredictionModel(Of BikeSharingDemandSample, BikeSharingDemandPrediction), testDataLocation As String)
        Dim testData = (New BikeSharingDemandsCsvReader()).GetDataFromCsv(testDataLocation).ToList()
        For i As Integer = 0 To 9
            Dim prediction = model.Predict(testData(i))
            Console.WriteLine($"-------------------------------------------------")
            Console.WriteLine($"Predicted : {prediction.PredictedCount}")
            Console.WriteLine($"Actual:    {testData(i).Count}")
            Console.WriteLine($"-------------------------------------------------")
        Next
    End Sub
End Module
