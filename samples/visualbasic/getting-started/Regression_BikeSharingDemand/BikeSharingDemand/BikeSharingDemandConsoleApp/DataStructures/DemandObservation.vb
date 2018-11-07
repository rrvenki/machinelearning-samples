Imports Microsoft.ML.Runtime.Api
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace BikeSharingDemand.DataStructures
	Public Class DemandObservation
		Public Season As Single
		Public Year As Single
		Public Month As Single
		Public Hour As Single
		Public Holiday As Single
		Public Weekday As Single
		Public WorkingDay As Single
		Public Weather As Single
		Public Temperature As Single
		Public NormalizedTemperature As Single
		Public Humidity As Single
		Public Windspeed As Single
		Public Count As Single ' This is the observed count, to be used a "label" to predict
	End Class

	Public Module DemandObservationSample
		Public ReadOnly Property SingleDemandSampleData() As DemandObservation
			Get
				Return New DemandObservation() With {
					.Season = 3,
					.Year = 1,
					.Month = 8,
					.Hour = 10,
					.Holiday = 0,
					.Weekday = 4,
					.WorkingDay = 1,
					.Weather = 1,
					.Temperature = 0.8F,
					.NormalizedTemperature = 0.7576F,
					.Humidity = 0.55F,
					.Windspeed = 0.2239F
				}
			End Get
		End Property
	End Module
End Namespace
