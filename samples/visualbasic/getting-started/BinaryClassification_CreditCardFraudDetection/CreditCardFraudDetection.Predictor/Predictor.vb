Imports CreditCardFraudDetection.Common
Imports CreditCardFraudDetection.Common.DataModels
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Api
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Runtime.Data.IO

Namespace CreditCardFraudDetection.Predictor
    Public Class Predictor
        Private ReadOnly _modelfile As String
        Private ReadOnly _dasetFile As String

        Public Sub New(modelfile As String, dasetFile As String)
            If modelfile Is Nothing Then Throw New ArgumentNullException(NameOf(modelfile))
            _modelfile = modelfile
            If dasetFile Is Nothing Then Throw New ArgumentNullException(NameOf(dasetFile))
            _dasetFile = dasetFile
        End Sub

        Public Sub RunMultiplePredictions(numberOfTransactions As Integer, Optional seed? As Integer = 1)

            Dim mlContext = New MLContext(seed)

            Dim binTestData = New BinaryLoader(mlContext, New BinaryLoader.Arguments(), New MultiFileSource(_dasetFile))
            Dim testRoles = New RoleMappedData(binTestData, roles:=TransactionObservation.Roles())
            Dim dataTest = testRoles.Data

            'Inspect/Peek data from datasource
            ConsoleWriterSection($"Inspect {numberOfTransactions} transactions observed as fraud and {numberOfTransactions} not observed as fraud, from the test datasource:")
            InspectData(mlContext, dataTest, numberOfTransactions)

            ConsoleWriteHeader($"Predictions from saved model:")
            Dim model As ITransformer = mlContext.ReadModel(_modelfile)
            Dim predictionFunc = model.MakePredictionFunction(Of TransactionObservation, TransactionFraudPrediction)(mlContext)
            ConsoleWriterSection($"Test {numberOfTransactions} transactions, from the test datasource, that should be predicted as fraud (true):")
            dataTest.AsEnumerable(Of TransactionObservation)(mlContext, reuseRowObject:=False).Where(Function(x) x.Label = True).Take(numberOfTransactions).Select(Function(testData) testData).ToList().ForEach(Sub(testData)
                                                                                                                                                                                                                     Console.WriteLine($"--- Transaction ---")
                                                                                                                                                                                                                     testData.PrintToConsole()
                                                                                                                                                                                                                     predictionFunc.Predict(testData).PrintToConsole()
                                                                                                                                                                                                                     Console.WriteLine($"-------------------")
                                                                                                                                                                                                                 End Sub)


            ConsoleWriterSection($"Test {numberOfTransactions} transactions, from the test datasource, that should NOT be predicted as fraud (false):")
            dataTest.AsEnumerable(Of TransactionObservation)(mlContext, reuseRowObject:=False).Where(Function(x) x.Label = False).Take(numberOfTransactions).ToList().ForEach(Sub(testData)
                                                                                                                                                                                  Console.WriteLine($"--- Transaction ---")
                                                                                                                                                                                  testData.PrintToConsole()
                                                                                                                                                                                  predictionFunc.Predict(testData).PrintToConsole()
                                                                                                                                                                                  Console.WriteLine($"-------------------")
                                                                                                                                                                              End Sub)
        End Sub

    End Class
End Namespace
