Imports BikeSharingDemand.BikeSharingDemandData
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Models
Imports Microsoft.ML.Trainers
Imports Microsoft.ML.Transforms

Namespace Model

    Public NotInheritable Class ModelBuilder
        Private ReadOnly _trainingDataLocation As String
        Private ReadOnly _algorythm As ILearningPipelineItem

        Public Sub New(trainingDataLocation As String, algorythm As ILearningPipelineItem)
            _trainingDataLocation = trainingDataLocation
            _algorythm = algorythm
        End Sub

        ''' <summary>
        ''' Using training data location that is passed trough constructor this method is building
        ''' and training machine learning model.
        ''' </summary>
        ''' <returns>Trained machine learning model.</returns>
        Public Function BuildAndTrain() As PredictionModel(Of BikeSharingDemandSample, BikeSharingDemandPrediction)
            Dim pipeline = New LearningPipeline()
            pipeline.Add((New TextLoader(_trainingDataLocation)).CreateFrom(Of BikeSharingDemandSample)(useHeader:=True, separator:=","c))
            pipeline.Add(New ColumnCopier(("Count", "Label")))
            pipeline.Add(New ColumnConcatenator("Features", "Season", "Year", "Month", "Hour", "Weekday", "Weather", "Temperature", "NormalizedTemperature", "Humidity", "Windspeed"))
            pipeline.Add(_algorythm)

            Return pipeline.Train(Of BikeSharingDemandSample, BikeSharingDemandPrediction)()
        End Function
    End Class

End Namespace