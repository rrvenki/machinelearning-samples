Imports Microsoft.ML.Runtime.Api
Imports System

Namespace ImageClassification.ImageData
	Public Class ImageNetPrediction
		Public Score() As Single

		Public PredictedLabelValue As String
	End Class

	Public Class ImageNetWithLabelPrediction
		Inherits ImageNetPrediction

		Public Sub New(ByVal pred As ImageNetPrediction, ByVal label As String)
			Me.Label = label
			Score = pred.Score
			PredictedLabelValue = pred.PredictedLabelValue
		End Sub

		Public Label As String
	End Class

End Namespace
