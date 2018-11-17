Imports Microsoft.ML.Runtime.Api
Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Clustering_Iris.DataStructures
	Public Class IrisData
		<Column("0")>
		Public Label As Single

		<Column("1")>
		Public SepalLength As Single

		<Column("2")>
		Public SepalWidth As Single

		<Column("3")>
		Public PetalLength As Single

		<Column("4")>
		Public PetalWidth As Single

	End Class
End Namespace
