Imports Microsoft.ML.Runtime.Api

Namespace BinaryClasification_TitanicSurvivalPrediction
	Public Class TitanicPrediction
		<Column(ordinal:= "0", name:= "PredictedLabel")>
		Public Survived As Boolean

		<Column(ordinal:= "1", name:= "Probability")>
		Public Probability As Single
	End Class
End Namespace