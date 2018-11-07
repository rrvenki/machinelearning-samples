Imports CreditCardFraudDetection.Common
Imports CreditCardFraudDetection.Common.DataModels
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Api
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Runtime.Data.IO
Imports System
Imports System.Linq

Namespace CreditCardFraudDetection.Predictor
	Public Class Predictor
		Private ReadOnly _modelfile As String
		Private ReadOnly _dasetFile As String

		Public Sub New(ByVal modelfile As String, ByVal dasetFile As String)
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _modelfile = modelfile ?? throw new ArgumentNullException(nameof(modelfile));
			_modelfile = If(modelfile, throw New ArgumentNullException(NameOf(modelfile)))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _dasetFile = dasetFile ?? throw new ArgumentNullException(nameof(dasetFile));
			_dasetFile = If(dasetFile, throw New ArgumentNullException(NameOf(dasetFile)))
		End Sub

		Public Sub RunMultiplePredictions(ByVal numberOfTransactions As Integer, Optional ByVal seed? As Integer = 1)

			Dim env = New LocalEnvironment(seed)

			Dim binTestData = New BinaryLoader(env, New BinaryLoader.Arguments(), New MultiFileSource(_dasetFile))
			Dim testRoles = New RoleMappedData(binTestData, roles:= TransactionObservation.Roles())
			Dim dataTest = testRoles.Data

			'Inspect/Peek data from datasource
			ConsoleHelpers.ConsoleWriterSection($"Inspect {numberOfTransactions} transactions observed as fraud and {numberOfTransactions} not observed as fraud, from the test datasource:")
			ConsoleHelpers.InspectData(env, dataTest, numberOfTransactions)

			ConsoleHelpers.ConsoleWriteHeader($"Predictions from saved model:")
				Dim model As ITransformer = env.ReadModel(_modelfile)
				Dim predictionFunc = model.MakePredictionFunction(Of TransactionObservation, TransactionFraudPrediction)(env)
				ConsoleHelpers.ConsoleWriterSection($"Test {numberOfTransactions} transactions, from the test datasource, that should be predicted as fraud (true):")
				dataTest.AsEnumerable(Of TransactionObservation)(env, reuseRowObject:= False).Where(Function(x) x.Label = True).Take(numberOfTransactions).Select(Function(testData) testData).ToList().ForEach(Sub(testData)
										Console.WriteLine($"--- Transaction ---")
										testData.PrintToConsole()
										predictionFunc.Predict(testData).PrintToConsole()
										Console.WriteLine($"-------------------")
				End Sub)


				ConsoleHelpers.ConsoleWriterSection($"Test {numberOfTransactions} transactions, from the test datasource, that should NOT be predicted as fraud (false):")
				dataTest.AsEnumerable(Of TransactionObservation)(env, reuseRowObject:= False).Where(Function(x) x.Label = False).Take(numberOfTransactions).ToList().ForEach(Sub(testData)
										Console.WriteLine($"--- Transaction ---")
										testData.PrintToConsole()
										predictionFunc.Predict(testData).PrintToConsole()
										Console.WriteLine($"-------------------")
				End Sub)
		End Sub

	End Class
End Namespace
