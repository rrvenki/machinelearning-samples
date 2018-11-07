Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace BikeSharingDemand
	Public Class DataProcessor
		Private privateDataProcessPipeline As IEstimator(Of ITransformer)
		Public Property DataProcessPipeline() As IEstimator(Of ITransformer)
			Get
				Return privateDataProcessPipeline
			End Get
			Private Set(ByVal value As IEstimator(Of ITransformer))
				privateDataProcessPipeline = value
			End Set
		End Property

		Private Shared _featureColumns() As String = { "Season", "Year", "Month", "Hour", "Holiday", "Weekday", "Weather", "Temperature", "NormalizedTemperature", "Humidity", "Windspeed" }

		Public Sub New(ByVal mlContext As MLContext)
			' Configure data transformations in the Process pipeline
			DataProcessPipeline = (New CopyColumnsEstimator(mlContext, "Count", "Label")).Append(New ColumnConcatenatingEstimator(mlContext, "Features", _featureColumns))
		End Sub
	End Class
End Namespace
