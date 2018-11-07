Imports Microsoft.ML.Runtime.Api

Namespace TitanicSurvivalConsoleApp.DataStructures
	Public Class TitanicData
		<Column("0")>
		Public PassengerId As Single

        <Column("1")>
        Public Label As Single 'If "Survived" or not

        <Column("2")>
		Public Pclass As Single

		<Column("3")>
		Public Name As String

		<Column("4")>
		Public Sex As String

		<Column("5")>
		Public Age As Single

		<Column("6")>
		Public SibSp As Single

		<Column("7")>
		Public Parch As Single

		<Column("8")>
		Public Ticket As String

		<Column("9")>
		Public Fare As Single

		<Column("10")>
		Public Cabin As String

		<Column("11")>
		Public Embarked As String
	End Class
End Namespace