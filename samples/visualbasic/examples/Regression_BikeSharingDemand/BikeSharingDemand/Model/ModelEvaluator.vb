Imports BikeSharingDemand.BikeSharingDemandData
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Models

Namespace Model
    Public Class ModelEvaluator
        ''' <summary>
        ''' Ussing passed testing data and model, it calculates model's accuracy.
        ''' </summary>
        ''' <returns>Accuracy of the model.</returns>
        Public Function Evaluate(model As PredictionModel(Of BikeSharingDemandSample, BikeSharingDemandPrediction), testDataLocation As String) As RegressionMetrics
            Dim testData = (New TextLoader(testDataLocation)).CreateFrom(Of BikeSharingDemandSample)(useHeader:=True, separator:=","c)
            Dim metrics = (New RegressionEvaluator()).Evaluate(model, testData)
            Return metrics
        End Function
    End Class
End Namespace