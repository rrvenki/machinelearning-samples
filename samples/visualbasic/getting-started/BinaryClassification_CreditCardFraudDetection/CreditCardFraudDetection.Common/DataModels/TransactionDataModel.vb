Imports Microsoft.ML.Runtime.Api
Imports System
Imports System.Collections.Generic
Imports Microsoft.ML.Runtime.Data.RoleMappedSchema

Namespace CreditCardFraudDetection.Common.DataModels

	Public Interface IModelEntity
		Sub PrintToConsole()
	End Interface

	Public Class TransactionObservation
		Implements IModelEntity

		Public Label As Boolean
		Public V1 As Single
		Public V2 As Single
		Public V3 As Single
		Public V4 As Single
		Public V5 As Single
		Public V6 As Single
		Public V7 As Single
		Public V8 As Single
		Public V9 As Single
		Public V10 As Single
		Public V11 As Single
		Public V12 As Single
		Public V13 As Single
		Public V14 As Single
		Public V15 As Single
		Public V16 As Single
		Public V17 As Single
		Public V18 As Single
		Public V19 As Single
		Public V20 As Single
		Public V21 As Single
		Public V22 As Single
		Public V23 As Single
		Public V24 As Single
		Public V25 As Single
		Public V26 As Single
		Public V27 As Single
		Public V28 As Single
		Public Amount As Single

		Public Sub PrintToConsole() Implements IModelEntity.PrintToConsole
			Console.WriteLine($"Label: {Label}")
			Console.WriteLine($"Features: [V1] {V1} [V2] {V2} [V3] {V3} ... [V28] {V28} Amount: {Amount}")
		End Sub

		Public Shared Function Roles() As List(Of KeyValuePair(Of ColumnRole, String))
			Return New List(Of KeyValuePair(Of ColumnRole, String))() From {
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Label, "Label"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V1"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V2"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V3"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V4"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V5"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V6"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V7"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V8"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V9"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V10"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V11"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V12"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V13"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V14"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V15"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V16"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V17"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V18"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V19"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V20"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V21"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V22"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V23"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V24"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V25"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V26"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V27"),
				New KeyValuePair(Of ColumnRole, String)(ColumnRole.Feature, "V28"),
				New KeyValuePair(Of ColumnRole, String)(New ColumnRole("Amount"), "")
			}
		End Function
	End Class

	Public Class TransactionFraudPrediction
		Implements IModelEntity

		Public Label As Boolean
		Public PredictedLabel As Boolean
		Public Score As Single
		Public Probability As Single

		Public Sub PrintToConsole() Implements IModelEntity.PrintToConsole
			Console.WriteLine($"Predicted Label: {PredictedLabel}")
			Console.WriteLine($"Probability: {Probability}  ({Score})")
		End Sub
	End Class
End Namespace
