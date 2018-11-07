Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data

Namespace BikeSharingDemand
    Friend Class DataLoader
        Private _mlContext As MLContext
        Private _loader As TextLoader

        Public Sub New(mlContext As MLContext)
            _mlContext = mlContext

            _loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
                .Separator = ",",
                .HasHeader = True,
                .Column = {
                    New TextLoader.Column("Season", DataKind.R4, 2),
                    New TextLoader.Column("Year", DataKind.R4, 3),
                    New TextLoader.Column("Month", DataKind.R4, 4),
                    New TextLoader.Column("Hour", DataKind.R4, 5),
                    New TextLoader.Column("Holiday", DataKind.R4, 6),
                    New TextLoader.Column("Weekday", DataKind.R4, 7),
                    New TextLoader.Column("WorkingDay", DataKind.R4, 8),
                    New TextLoader.Column("Weather", DataKind.R4, 9),
                    New TextLoader.Column("Temperature", DataKind.R4, 10),
                    New TextLoader.Column("NormalizedTemperature", DataKind.R4, 11),
                    New TextLoader.Column("Humidity", DataKind.R4, 12),
                    New TextLoader.Column("Windspeed", DataKind.R4, 13),
                    New TextLoader.Column("Count", DataKind.R4, 16)
                }
            })
        End Sub

        Public Function GetDataView(filePath As String) As IDataView
            Return _loader.Read(filePath)
        End Function
    End Class
End Namespace
