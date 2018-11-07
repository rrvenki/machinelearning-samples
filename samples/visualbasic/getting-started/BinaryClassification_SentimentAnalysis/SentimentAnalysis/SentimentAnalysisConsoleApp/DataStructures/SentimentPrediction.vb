Imports Microsoft.ML.Runtime.Api

Namespace SentimentAnalysisConsoleApp.DataStructures
	Public Class SentimentPrediction
		<ColumnName("PredictedLabel")>
		Public Property Prediction() As Boolean

		<ColumnName("Probability")>
		Public Property Probability() As Single

		<ColumnName("Score")>
		Public Property Score() As Single
	End Class
End Namespace