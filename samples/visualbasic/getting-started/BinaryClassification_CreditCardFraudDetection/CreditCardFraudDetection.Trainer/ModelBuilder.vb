Imports CreditCardFraudDetection.Common
Imports CreditCardFraudDetection.Common.DataModels

Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Runtime.Data.IO
Imports Microsoft.ML.Runtime.FastTree
Imports Microsoft.ML.Runtime.Api.GenerateCodeCommand

Imports System
Imports System.IO
Imports System.Linq

Namespace CreditCardFraudDetection.Trainer
	Public Class ModelBuilder
		Private ReadOnly _assetsPath As String
		Private ReadOnly _dataSetFile As String
		Private ReadOnly _outputPath As String

		Private _context As BinaryClassificationContext
		Private _reader As TextLoader
		Private _trainData As IDataView
		Private _testData As IDataView
		Private _env As LocalEnvironment

		Public Sub New(ByVal assetsPath As String, ByVal dataSetFile As String)
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _assetsPath = assetsPath ?? throw new ArgumentNullException(nameof(assetsPath));
			_assetsPath = If(assetsPath, throw New ArgumentNullException(NameOf(assetsPath)))
'INSTANT VB TODO TASK: Throw expressions are not converted by Instant VB:
'ORIGINAL LINE: _dataSetFile = dataSetFile ?? throw new ArgumentNullException(nameof(dataSetFile));
			_dataSetFile = If(dataSetFile, throw New ArgumentNullException(NameOf(dataSetFile)))
			_outputPath = Path.Combine(_assetsPath, "output")
		End Sub


		Public Function Build(Optional ByVal seed? As Integer = 1) As ModelBuilder
			' Create a new environment for ML.NET operations.
			' It can be used for exception tracking and logging, 
			' as well as the source of randomness.
			' Seed set to any number so you have a deterministic environment
			_env = New LocalEnvironment(seed)
'INSTANT VB TODO TASK: VB has no equivalent to the C# deconstruction assignments:
			(_context, _reader, _trainData, _testData) = PrepareData(_env)

			Return Me
		End Function

		Public Sub TrainFastTreeAndSaveModels(Optional ByVal cvNumFolds As Integer = 2, Optional ByVal numLeaves As Integer = 20, Optional ByVal numTrees As Integer = 100, Optional ByVal minDocumentsInLeafs As Integer = 10, Optional ByVal learningRate As Double = 0.2, Optional ByVal advancedSettings As Action(Of Arguments) = Nothing)

			Dim logMeanVarNormalizer = New Normalizer(_env,Normalizer.NormalizerMode.MeanVariance,("Features", "FeaturesNormalizedByMeanVar"))

			'Create a flexible pipeline (composed by a chain of estimators) for building/traing the model.
			Dim pipeline = (New ConcatEstimator(_env, "Features", { "Amount", "V1", "V2", "V3", "V4", "V5", "V6", "V7", "V8", "V9", "V10", "V11", "V12", "V13", "V14", "V15", "V16", "V17", "V18", "V19", "V20", "V21", "V22", "V23", "V24", "V25", "V26", "V27", "V28" })).Append(New Normalizer(_env, Normalizer.NormalizerMode.MeanVariance, ("Features", "FeaturesNormalizedByMeanVar"))).Append(New FastTreeBinaryClassificationTrainer(_env, "Label", "Features", numLeaves:= 20, numTrees:= 100, minDocumentsInLeafs := 10, learningRate:= 0.2))

			Dim model = pipeline.Fit(_trainData)

			Dim metrics = _context.Evaluate(model.Transform(_testData), "Label")

			ConsoleHelpers.ConsoleWriteHeader($"Test Metrics:")
			Console.WriteLine("Acuracy: " & metrics.Accuracy)
			metrics.ToConsole()

			model.SaveModel(_env, Path.Combine(_outputPath, "fastTree.zip"))
			Console.WriteLine("Saved model to " & Path.Combine(_outputPath, "fastTree.zip"))
		End Sub

		Private Function PrepareData(ByVal env As LocalEnvironment) As (context As BinaryClassificationContext, TextLoader, trainData As IDataView, testData As IDataView)

			Dim data As IDataView = Nothing
			Dim trainData As IDataView = Nothing
			Dim testData As IDataView = Nothing

			' Step one: read the data as an IDataView.
			' Create the reader: define the data columns 
			' and where to find them in the text file.
			Dim reader = New TextLoader(env, New TextLoader.Arguments With {
				.Column = {
					New TextLoader.Column("Label", DataKind.BL, 30),
					New TextLoader.Column("V1", DataKind.R4, 1),
					New TextLoader.Column("V2", DataKind.R4, 2),
					New TextLoader.Column("V3", DataKind.R4, 3),
					New TextLoader.Column("V4", DataKind.R4, 4),
					New TextLoader.Column("V5", DataKind.R4, 5),
					New TextLoader.Column("V6", DataKind.R4, 6),
					New TextLoader.Column("V7", DataKind.R4, 7),
					New TextLoader.Column("V8", DataKind.R4, 8),
					New TextLoader.Column("V9", DataKind.R4, 9),
					New TextLoader.Column("V10", DataKind.R4, 10),
					New TextLoader.Column("V11", DataKind.R4, 11),
					New TextLoader.Column("V12", DataKind.R4, 12),
					New TextLoader.Column("V13", DataKind.R4, 13),
					New TextLoader.Column("V14", DataKind.R4, 14),
					New TextLoader.Column("V15", DataKind.R4, 15),
					New TextLoader.Column("V16", DataKind.R4, 16),
					New TextLoader.Column("V17", DataKind.R4, 17),
					New TextLoader.Column("V18", DataKind.R4, 18),
					New TextLoader.Column("V19", DataKind.R4, 19),
					New TextLoader.Column("V20", DataKind.R4, 20),
					New TextLoader.Column("V21", DataKind.R4, 21),
					New TextLoader.Column("V22", DataKind.R4, 22),
					New TextLoader.Column("V23", DataKind.R4, 23),
					New TextLoader.Column("V24", DataKind.R4, 24),
					New TextLoader.Column("V25", DataKind.R4, 25),
					New TextLoader.Column("V26", DataKind.R4, 26),
					New TextLoader.Column("V27", DataKind.R4, 27),
					New TextLoader.Column("V28", DataKind.R4, 28),
					New TextLoader.Column("Amount", DataKind.R4, 29)
				},
				.HasHeader = True,
				.Separator = ","
			})


			' We know that this is a Binary Classification task,
			' so we create a Binary Classification context:
			' it will give us the algorithms we need,
			' as well as the evaluation procedure.
			Dim classification = New BinaryClassificationContext(env)

			If Not File.Exists(Path.Combine(_outputPath, "testData.idv")) AndAlso Not File.Exists(Path.Combine(_outputPath, "trainData.idv")) Then
				' Split the data 80:20 into train and test sets, train and evaluate.

				data = reader.Read(New MultiFileSource(_dataSetFile))
				ConsoleHelpers.ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (source)")
				ConsoleHelpers.InspectData(env, data, 4)



				' Can't do stratification when column type is a boolean
				'(trainData, testData) = classification.TrainTestSplit(data, testFraction: 0.2, stratificationColumn: "Label");
'INSTANT VB TODO TASK: VB has no equivalent to the C# deconstruction assignments:
				(trainData, testData) = classification.TrainTestSplit(data, testFraction: 0.2)

				' save test split
				Using ch = env.Start("SaveData")
				Using file = env.CreateOutputFile(Path.Combine(_outputPath, "testData.idv"))
					Dim saver = New BinarySaver(env, New BinarySaver.Arguments())
					DataSaverUtils.SaveDataView(ch, saver, testData, file)
				End Using
				End Using

				' save train split
				Using ch = env.Start("SaveData")
				Using file = env.CreateOutputFile(Path.Combine(_outputPath, "trainData.idv"))
					Dim saver = New BinarySaver(env, New BinarySaver.Arguments())
					DataSaverUtils.SaveDataView(ch, saver, trainData, file)
				End Using
				End Using

			Else
				' Load splited data
				Dim binTrainData = New BinaryLoader(env, New BinaryLoader.Arguments(), New MultiFileSource(Path.Combine(_outputPath, "trainData.idv")))
				Dim trainRoles = New RoleMappedData(binTrainData, roles:= TransactionObservation.Roles())
				trainData = trainRoles.Data


				Dim binTestData = New BinaryLoader(env, New BinaryLoader.Arguments(), New MultiFileSource(Path.Combine(_outputPath, "testData.idv")))
				Dim testRoles = New RoleMappedData(binTestData, roles:= TransactionObservation.Roles())
				testData = testRoles.Data

			End If

			ConsoleHelpers.ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (traindata)")
			ConsoleHelpers.InspectData(env, trainData, 4)

			ConsoleHelpers.ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (testData)")
			ConsoleHelpers.InspectData(env, testData, 4)

			Return (classification, reader, trainData, testData)
		End Function
	End Class

End Namespace
