Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports ImageClassification.ImageData
Imports ImageClassification.Model.ConsoleHelpers
Imports ImageClassification.Model.ModelHelpers
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Runtime.ImageAnalytics
Imports System.IO
Imports Microsoft.ML.Transforms

Namespace ImageClassification.Model
	Public Class ModelScorerCustom
		Private ReadOnly dataLocation As String
		Private ReadOnly imagesFolder As String
		Private ReadOnly modelLocation As String
		Private ReadOnly labelsLocation As String
		Private ReadOnly env As ConsoleEnvironment

		Public Sub New(ByVal dataLocation As String, ByVal imagesFolder As String, ByVal modelLocation As String, ByVal labelsLocation As String)
			Me.dataLocation = dataLocation
			Me.imagesFolder = imagesFolder
			Me.modelLocation = modelLocation
			Me.labelsLocation = labelsLocation
			env = New ConsoleEnvironment()
		End Sub

		Public Structure ImageNetSettings
			Public Const imageHeight As Integer = 224
			Public Const imageWidth As Integer = 224
			Public Const mean As Single = 117
			Public Const channelsLast As Boolean = True
		End Structure

		Public Sub Score()


			Dim model = LoadModel(dataLocation, imagesFolder, modelLocation)

			Dim predictions = PredictDataUsingModel(dataLocation, imagesFolder, labelsLocation, model).ToArray()

			'EvaluateModel(dataLocation, model);
		End Sub

		Private Function LoadModel(ByVal dataLocation As String, ByVal imagesFolder As String, ByVal modelLocation As String) As PredictionFunction(Of ImageNetData, ImageNetPrediction)
			ConsoleWriteHeader("Read model")
			Console.WriteLine($"Model location: {modelLocation}")

			Dim loader = TextLoader.CreateReader(env, Function(ctx) (ImagePath:= ctx.LoadText(0), Label:= ctx.LoadText(1)))

			Dim data = loader.Read(New MultiFileSource(dataLocation))

			Dim estimator = loader.MakeNewEstimator().Append(Function(row) (row.Label, input_1:= row.ImagePath.LoadAsImage(imagesFolder).Resize(ImageNetSettings.imageHeight, ImageNetSettings.imageWidth).ExtractPixels(interleaveArgb:= ImageNetSettings.channelsLast, offset:= ImageNetSettings.mean))).Append(Function(row) (row.Label, dense_2_Softmax := row.input_1.ApplyTensorFlowGraph(modelLocation)))

			Dim model = estimator.Fit(data)

			Dim predictionFunction = model.AsDynamic.MakePredictionFunction(Of ImageNetData, ImageNetPrediction)(env)

			Return predictionFunction
		End Function

		Protected Iterator Function PredictDataUsingModel(ByVal testLocation As String, ByVal imagesFolder As String, ByVal labelsLocation As String, ByVal model As PredictionFunction(Of ImageNetData, ImageNetPrediction)) As IEnumerable(Of ImageNetData)
			ConsoleWriteHeader("Classificate images")
			Console.WriteLine($"Images folder: {imagesFolder}")
			Console.WriteLine($"Training file: {testLocation}")
			Console.WriteLine($"Labels file: {labelsLocation}")

			Dim labels = ModelHelpers.ReadLabels(labelsLocation)

			'var testData = ImageNetData.ReadFromCsv(testLocation, imagesFolder);
			Dim testData = {
				New ImageNetData With {
					.ImagePath = Path.Combine(imagesFolder, "bracelet.jpg"),
					.Label = "bracelet"
				},
				New ImageNetData With {
					.ImagePath = Path.Combine(imagesFolder, "frisbee.jpg"),
					.Label = "frisbee"
				}
			}

			For Each sample In testData
				Dim probs = model.Predict(sample).PredictedLabels
				Dim imageData = New ImageNetDataProbability() With {.ImagePath = sample.ImagePath}
'INSTANT VB TODO TASK: VB has no equivalent to the C# deconstruction assignments:
				(imageData.Label, imageData.Probability) = GetLabel(labels, probs)
				imageData.ConsoleWrite()
				Yield imageData
			Next sample
		End Function

		Protected Sub EvaluateModel(ByVal testLocation As String, ByVal model As PredictionFunction(Of ImageNetData, ImageNetPrediction))
			ConsoleWriteHeader("Metrics for Image Classification")
			Dim evaluator = New MultiClassClassifierEvaluator(env, New MultiClassClassifierEvaluator.Arguments())

			Dim data = TextLoader.ReadFile(env, New TextLoader.Arguments With {
				.Separator = "tab",
				.HasHeader = False,
				.Column = {
					New TextLoader.Column("ImagePath", DataKind.Text, 0),
					New TextLoader.Column("Label", DataKind.Text, 1)
				}
			}, New MultiFileSource(testLocation))

			Dim evalRoles = New RoleMappedData(data, label:= "Label", feature:= "ImagePath")

			Dim metrics = evaluator.Evaluate(evalRoles)

			For Each item In metrics
				Console.WriteLine($"{item.Key}: {item.Value}")
			Next item
		End Sub
	End Class
End Namespace
