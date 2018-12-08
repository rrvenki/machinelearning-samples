Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace BikeSharingDemand
	Public Module BikeSharingTextLoaderFactory
		Public Function CreateTextLoader(ByVal mlContext As MLContext) As TextLoader
			Dim textLoader As TextLoader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
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
			Return textLoader
		End Function
	End Module
End Namespace
