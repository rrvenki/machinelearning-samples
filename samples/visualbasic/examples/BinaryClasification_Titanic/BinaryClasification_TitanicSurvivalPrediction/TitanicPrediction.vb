Imports Microsoft.ML.Runtime.Api

Public Class TitanicPrediction
    <Column("0", "PredictedLabel")>
    Public Survived As Boolean
    <Column("1", "Probability")>
    Public Probability As Single
End Class
