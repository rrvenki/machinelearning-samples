Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data

Namespace BikeSharingDemand
    Public Class BikeSharingDataProcessPipelineFactory
        Public Shared Function CreateDataProcessPipeline(mlContext As MLContext) As IEstimator(Of ITransformer)
            ' Copy the Count column to the Label column.
            Return mlContext.Transforms.CopyColumns("Count", "Label").Append(mlContext.Transforms.Concatenate("Features", "Season", "Year", "Month", "Hour", "Holiday", "Weekday", "Weather", "Temperature", "NormalizedTemperature", "Humidity", "Windspeed"))
        End Function
    End Class
End Namespace
