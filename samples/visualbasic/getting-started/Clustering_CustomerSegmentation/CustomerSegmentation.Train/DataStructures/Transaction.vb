﻿Imports Microsoft.ML.Runtime.Api
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace CustomerSegmentation.DataStructures
	Public Class Transaction
		'Customer Last Name,Offer #
		'Smith,2
		Public Property LastName() As String
		Public Property OfferId() As String

		Public Shared Function ReadFromCsv(ByVal file As String) As IEnumerable(Of Transaction)
			Return System.IO.File.ReadAllLines(file).Skip(1).Select(Function(x) x.Split(","c)).Select(Function(x) New Transaction() With {
				.LastName = x(0),
				.OfferId = x(1)
			})
		End Function
	End Class
End Namespace
