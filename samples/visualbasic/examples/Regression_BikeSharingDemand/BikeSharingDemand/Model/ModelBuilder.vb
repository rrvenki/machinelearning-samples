Imports BikeSharingDemand.BikeSharingDemandData
Imports Microsoft.ML.Legacy
Imports Microsoft.ML.Legacy.Data
Imports Microsoft.ML.Legacy.Models
Imports Microsoft.ML.Legacy.Trainers
Imports Microsoft.ML.Legacy.Transforms

Namespace Model

    Public NotInheritable Class ModelBuilder
        Private ReadOnly _trainingDataLocation As String
        Private ReadOnly _algorithm As ILearningPipelineItem

        Public Sub New(trainingDataLocation As String, algorithm As ILearningPipelineItem)
            _trainingDataLocation = trainingDataLocation
            _algorithm = algorithm
        End Sub

        ''' <summary>
        ''' Using training data location that is passed trough constructor this method is building
        ''' and training machine learning model.
        ''' </summary>
        ''' <returns>Trained machine learning model.</returns>
        Public Function BuildAndTrain() As PredictionModel(Of BikeSharingDemandSample, BikeSharingDemandPrediction)
            Dim pipeline = New LearningPipeline()
            pipeline.Add(New TextLoader(_trainingDataLocation).CreateFrom(Of BikeSharingDemandSample)(useHeader:=True, separator:=","c))
            pipeline.Add(New ColumnCopier(("Count", "Label")))
            pipeline.Add(New ColumnConcatenator("Features", "Season", "Year", "Month", "Hour", "Weekday", "Weather", "Temperature", "NormalizedTemperature", "Humidity", "Windspeed"))
            pipeline.Add(_algorithm)

            Return pipeline.Train(Of BikeSharingDemandSample, BikeSharingDemandPrediction)()
        End Function
    End Class

End Namespace