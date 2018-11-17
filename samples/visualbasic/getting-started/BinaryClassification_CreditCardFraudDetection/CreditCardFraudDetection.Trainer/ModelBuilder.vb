Imports CreditCardFraudDetection.Common
Imports CreditCardFraudDetection.Common.DataModels

Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Runtime.Data.IO
Imports Microsoft.ML.Transforms.Normalizers.NormalizingEstimator
Imports Microsoft.ML.Runtime.Api.GenerateCodeCommand
Imports System.IO
Imports Microsoft.ML.Runtime

Namespace CreditCardFraudDetection.Trainer
    Public Class ModelBuilder
        Private ReadOnly _assetsPath As String
        Private ReadOnly _dataSetFile As String
        Private ReadOnly _outputPath As String

        Private _context As BinaryClassificationContext
        Private _reader As TextLoader
        Private _trainData As IDataView
        Private _testData As IDataView
        Private _mlContext As MLContext

        Public Sub New(mlContext As MLContext, assetsPath As String, dataSetFile As String)
            If assetsPath Is Nothing Then
                Throw New ArgumentNullException(NameOf(assetsPath))
            End If
            If dataSetFile Is Nothing Then
                Throw New ArgumentNullException(NameOf(dataSetFile))
            End If

            _mlContext = mlContext
            _assetsPath = assetsPath
            _dataSetFile = dataSetFile
            _outputPath = Path.Combine(_assetsPath, "output")
        End Sub


        Public Function PreProcessData(mlContext As MLContext) As ModelBuilder
            With PrepareData(_mlContext)
                _context = .context
                _reader = .Item2
                _trainData = .trainData
                _testData = .testData
            End With
            Return Me
        End Function

        Public Sub TrainFastTreeAndSaveModels(Optional cvNumFolds As Integer = 2, Optional numLeaves As Integer = 20, Optional numTrees As Integer = 100, Optional minDocumentsInLeafs As Integer = 10, Optional learningRate As Double = 0.2, Optional advancedSettings As Action(Of Arguments) = Nothing)

            'Create a flexible pipeline (composed by a chain of estimators) for building/traing the model.

            Dim pipeline = _mlContext.Transforms.Concatenate("Features", {"Amount", "V1", "V2", "V3", "V4", "V5", "V6", "V7", "V8", "V9", "V10", "V11", "V12", "V13", "V14", "V15", "V16", "V17", "V18", "V19", "V20", "V21", "V22", "V23", "V24", "V25", "V26", "V27", "V28"}).Append(_mlContext.Transforms.Normalize(inputName:="Features", outputName:="FeaturesNormalizedByMeanVar", mode:=NormalizerMode.MeanVariance)).Append(_mlContext.BinaryClassification.Trainers.FastTree(label:="Label", features:="Features", numLeaves:=20, numTrees:=100, minDatapointsInLeafs:=10, learningRate:=0.2))

            Dim model = pipeline.Fit(_trainData)

            Dim metrics = _context.Evaluate(model.Transform(_testData), "Label")

            ConsoleWriteHeader($"Test Metrics:")
            Console.WriteLine("Acuracy: " & metrics.Accuracy)
            metrics.ToConsole()

            model.SaveModel(_mlContext, Path.Combine(_outputPath, "fastTree.zip"))
            Console.WriteLine("Saved model to " & Path.Combine(_outputPath, "fastTree.zip"))
        End Sub

        Private Function PrepareData(mlContext As MLContext) As (context As BinaryClassificationContext, TextLoader, trainData As IDataView, testData As IDataView)

            Dim data As IDataView = Nothing
            Dim trainData As IDataView = Nothing
            Dim testData As IDataView = Nothing

            ' Step one: read the data as an IDataView.
            ' Create the reader: define the data columns 
            ' and where to find them in the text file.
            Dim reader = New TextLoader(mlContext, New TextLoader.Arguments With {
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
            Dim classification = New BinaryClassificationContext(mlContext)

            If Not File.Exists(Path.Combine(_outputPath, "testData.idv")) AndAlso Not File.Exists(Path.Combine(_outputPath, "trainData.idv")) Then
                ' Split the data 80:20 into train and test sets, train and evaluate.

                data = reader.Read(New MultiFileSource(_dataSetFile))
                ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (source)")
                InspectData(mlContext, data, 4)

                ' Can't do stratification when column type is a boolean, is this an issue?
                With classification.TrainTestSplit(data, testFraction:=0.2)
                    trainData = .trainSet
                    testData = .testSet
                End With

                ' save test split
                Dim env As IHostEnvironment = DirectCast(mlContext, IHostEnvironment)
                Using ch = env.Start("SaveData")
                    Using file = env.CreateOutputFile(Path.Combine(_outputPath, "testData.idv"))
                        Dim saver = New BinarySaver(mlContext, New BinarySaver.Arguments())
                        DataSaverUtils.SaveDataView(ch, saver, testData, file)
                    End Using
                End Using

                ' save train split
                Using ch = DirectCast(env, IHostEnvironment).Start("SaveData")
                    Using file = env.CreateOutputFile(Path.Combine(_outputPath, "trainData.idv"))
                        Dim saver = New BinarySaver(mlContext, New BinarySaver.Arguments())
                        DataSaverUtils.SaveDataView(ch, saver, trainData, file)
                    End Using
                End Using

            Else
                ' Load splited data
                Dim binTrainData = New BinaryLoader(mlContext, New BinaryLoader.Arguments(), New MultiFileSource(Path.Combine(_outputPath, "trainData.idv")))
                Dim trainRoles = New RoleMappedData(binTrainData, roles:=TransactionObservation.Roles())
                trainData = trainRoles.Data


                Dim binTestData = New BinaryLoader(mlContext, New BinaryLoader.Arguments(), New MultiFileSource(Path.Combine(_outputPath, "testData.idv")))
                Dim testRoles = New RoleMappedData(binTestData, roles:=TransactionObservation.Roles())
                testData = testRoles.Data

            End If

            ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (traindata)")
            InspectData(mlContext, trainData, 4)

            ConsoleWriteHeader("Show 4 transactions fraud (true) and 4 transactions not fraud (false) -  (testData)")
            InspectData(mlContext, testData, 4)

            Return (classification, reader, trainData, testData)
        End Function
    End Class

End Namespace
