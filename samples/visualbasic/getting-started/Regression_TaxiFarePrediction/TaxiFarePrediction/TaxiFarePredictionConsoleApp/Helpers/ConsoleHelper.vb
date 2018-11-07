Imports System
Imports System.Linq
Imports Microsoft.ML.Runtime.Api
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Core.Data
Imports System.Collections.Generic
Imports Microsoft.ML.Data
Imports Regression_TaxiFarePrediction.DataStructures
Imports Microsoft.ML

Namespace Regression_TaxiFarePrediction.Helpers
	Public Module ConsoleHelper
		Public Function PeekDataViewInConsole(ByVal context As MLContext, ByVal dataView As IDataView, ByVal pipeline As EstimatorChain(Of ITransformer), Optional ByVal numberOfRows As Integer = 4) As List(Of TaxiTrip)
			Dim msg As String = String.Format("Show {0} rows with all the columns", numberOfRows.ToString())
			ConsoleWriteHeader(msg)

			'https://github.com/dotnet/machinelearning/blob/master/docs/code/MlNetCookBook.md#how-do-i-look-at-the-intermediate-data
			Dim transformedData = pipeline.Fit(dataView).Transform(dataView)

			' 'transformedData' is a 'promise' of data, lazy-loading. Let's actually read it.
			' Convert to an enumerable of user-defined type.
			Dim someRows = transformedData.AsEnumerable(Of TaxiTrip)(context, reuseRowObject:= False).Take(numberOfRows).ToList()

			' print to console the peeked rows
			someRows.ForEach(Sub(row)
				Console.WriteLine($"Label [FareAmount]: {row.FareAmount} || Features: [RateCode] {row.RateCode} [PassengerCount] {row.PassengerCount} [TripTime] {row.TripTime} [TripDistance] {row.TripDistance} [PaymentType] {row.PaymentType} ")
			End Sub)

			Return someRows
		End Function

		Public Function PeekFeaturesColumnDataInConsole(ByVal columnName As String, ByVal mlcontext As MLContext, ByVal dataView As IDataView, ByVal pipeline As EstimatorChain(Of ITransformer), Optional ByVal numberOfRows As Integer = 4) As List(Of Single())
			Dim msg As String = String.Format("Show {0} rows with just the '{1}' column", numberOfRows, columnName)
			ConsoleWriteHeader(msg)

			Dim transformedData = pipeline.Fit(dataView).Transform(dataView)
			' Extract the 'Features' column.

			Dim someColumnData = transformedData.GetColumn(Of Single())(mlcontext, columnName).Take(numberOfRows).ToList()

			' print to console the peeked rows
			someColumnData.ForEach(Sub(row)
				Dim concatColumn As String = String.Empty
				For Each f As Single In row
					concatColumn += f.ToString()
				Next f
				Console.WriteLine(concatColumn)
			End Sub)

			Return someColumnData
		End Function

		Public Sub ConsoleWriteHeader(ParamArray ByVal lines() As String)
			Dim defaultColor = Console.ForegroundColor
			Console.ForegroundColor = ConsoleColor.Yellow
			Console.WriteLine(" ")
			For Each line In lines
				Console.WriteLine(line)
			Next line
			Dim maxLength = lines.Select(Function(x) x.Length).Max()
			Console.WriteLine(New String("#"c, maxLength))
			Console.ForegroundColor = defaultColor
		End Sub

		Public Sub ConsoleWriterSection(ParamArray ByVal lines() As String)
			Dim defaultColor = Console.ForegroundColor
			Console.ForegroundColor = ConsoleColor.Blue
			Console.WriteLine(" ")
			For Each line In lines
				Console.WriteLine(line)
			Next line
			Dim maxLength = lines.Select(Function(x) x.Length).Max()
			Console.WriteLine(New String("-"c, maxLength))
			Console.ForegroundColor = defaultColor
		End Sub

		Public Sub ConsolePressAnyKey()
			Dim defaultColor = Console.ForegroundColor
			Console.ForegroundColor = ConsoleColor.Green
			Console.WriteLine(" ")
			Console.WriteLine("Press any key to finish.")
			Console.ReadKey()
		End Sub

		Public Sub ConsoleWriteException(ParamArray ByVal lines() As String)
			Dim defaultColor = Console.ForegroundColor
			Console.ForegroundColor = ConsoleColor.Red
			Const exceptionTitle As String = "EXCEPTION"
			Console.WriteLine(" ")
			Console.WriteLine(exceptionTitle)
			Console.WriteLine(New String("#"c, exceptionTitle.Length))
			Console.ForegroundColor = defaultColor
			For Each line In lines
				Console.WriteLine(line)
			Next line
		End Sub

		Public Sub ConsoleWriteWarning(ParamArray ByVal lines() As String)
			Dim defaultColor = Console.ForegroundColor
			Console.ForegroundColor = ConsoleColor.DarkMagenta
			Const warningTitle As String = "WARNING"
			Console.WriteLine(" ")
			Console.WriteLine(warningTitle)
			Console.WriteLine(New String("#"c, warningTitle.Length))
			Console.ForegroundColor = defaultColor
			For Each line In lines
				Console.WriteLine(line)
			Next line
		End Sub

	End Module
End Namespace
