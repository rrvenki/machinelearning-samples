Imports System.IO
Imports Microsoft.ML.Legacy
Imports Microsoft.ML.Legacy.Data
Imports Microsoft.ML.Legacy.Trainers
Imports Microsoft.ML.Legacy.Transforms
Imports TensorFlowMLNETInceptionv3ModelScoring.ImageData

Namespace Model
    Public Class ModelTrainer
        Private ReadOnly _dataLocation As String
        Private ReadOnly _imagesFolder As String
        Private ReadOnly _inputModelLocation As String
        Private ReadOnly _outputModelLocation As String

        Public Sub New(dataLocation As String, imagesFolder As String, inputModelLocation As String, outputModelLocation As String)
            _dataLocation = dataLocation
            _imagesFolder = imagesFolder
            _inputModelLocation = inputModelLocation
            _outputModelLocation = outputModelLocation
        End Sub

        Private Structure ImageNetSettings
            Public Const ImageHeight As Integer = 224
            Public Const ImageWidth As Integer = 224
            Public Const Mean As Single = 117
            Public Const Scale As Single = 1
            Public Const ChannelsLast As Boolean = True
        End Structure

        Private Structure InceptionSettings
            ' for checking tensor names, you can use tools like Netron,
            ' which is installed by Visual Studio AI Tools

            ' input tensor name
            Public Const inputTensorName As String = "input"

            ' output tensor name: in this case, is the node before the last one (not softmax, but softmax2_pre_activation).
            Public Const outputTensorName As String = "softmax2_pre_activation"
        End Structure

        Public Async Function BuildAndTrain() As Task
            Dim learningPipeline = BuildModel(_dataLocation, _imagesFolder, _inputModelLocation)
            Dim model = Train(learningPipeline)
            Dim predictions = GetPredictions(_dataLocation, _imagesFolder, model).ToArray()
            ShowPredictions(predictions)

            If _outputModelLocation IsNot Nothing AndAlso _outputModelLocation.Length > 0 Then
                DeleteAssets(_outputModelLocation)
                Await model.WriteAsync(_outputModelLocation)
            End If
        End Function

        Protected Function Train(pipeline As LearningPipeline) As PredictionModel(Of ImageNetData, ImageNetPrediction)
            ' Initialize TensorFlow engine
            ' TensorFlowUtils.Initialize()

            Dim model = pipeline.Train(Of ImageNetData, ImageNetPrediction)()
            Return model
        End Function

        Protected Function BuildModel(dataLocation As String, imagesFolder As String, modelLocation As String) As LearningPipeline
            Const convertPixelsToFloat As Boolean = True
            Const ignoreAlphaChannel As Boolean = False

            Dim pipeline As New LearningPipeline

            ' TextLoader loads tsv file, containing image file location and label 
            pipeline.Add(New TextLoader(dataLocation).CreateFrom(Of ImageNetData)(useHeader:=False))

            ' ImageLoader reads input images
            pipeline.Add(New ImageLoader((NameOf(ImageNetData.ImagePath), "ImageReal")) With {.ImageFolder = imagesFolder})

            ' ImageResizer is used to resize input image files
            ' to the size used by the Neural Network
            pipeline.Add(New ImageResizer(("ImageReal", "ImageCropped")) With {
                .ImageHeight = ImageNetSettings.ImageHeight,
                .ImageWidth = ImageNetSettings.ImageWidth,
                .Resizing = ImageResizerTransformResizingKind.IsoCrop
            })

            ' ImagePixelExtractor is used to process the input image files
            ' according to the requirements of the Deep Neural Network
            ' This step is the perfect place to make specific image transformations,
            ' like normalizing pixel values (Pixel * scale / offset). 
            ' This kind of image pre-processing is common when dealing with images used in DNN
            pipeline.Add(New ImagePixelExtractor(("ImageCropped", InceptionSettings.inputTensorName)) With {
                .UseAlpha = ignoreAlphaChannel,
                .InterleaveArgb = ImageNetSettings.ChannelsLast,
                .Convert = convertPixelsToFloat,
                .Offset = ImageNetSettings.Mean,
                .Scale = ImageNetSettings.Scale
            })

            ' TensorFlowScorer is used to get the activation map before the last output of the Neural Network
            ' This activation map is used as a image vector featurizer 
            pipeline.Add(New TensorFlowScorer With {
                .Model = modelLocation,
                .InputColumns = {InceptionSettings.inputTensorName},
                .OutputColumns = {InceptionSettings.outputTensorName}
            })

            pipeline.Add(New ColumnConcatenator("Features", InceptionSettings.outputTensorName))
            pipeline.Add(New TextToKeyConverter(NameOf(ImageNetData.Label)))

            ' At this point, there are two inputs for the learner: 
            ' * Features: input image vector feaures
            ' * Label: label fom the input file
            ' In this case, we use SDCA for classifying images using Label / Features columns. 
            ' Other multi-class classifier may be used instead of SDCA
            pipeline.Add(New StochasticDualCoordinateAscentClassifier)

            Return pipeline
        End Function

        Protected Iterator Function GetPredictions(testLocation As String, imagesFolder As String,
                                                   model As PredictionModel(Of ImageNetData, ImageNetPrediction)) As IEnumerable(Of ImageNetData)
            Dim labels As String() = Nothing
            model.TryGetScoreLabelNames(labels)
            Dim testData = ImageNetData.ReadFromCsv(testLocation, imagesFolder)

            For Each sample In testData
                Dim probs = model.Predict(sample).PredictedLabels
                Yield New ImageNetData With {
                    .ImagePath = sample.ImagePath,
                    .Label = GetLabel(labels, probs)
                }
            Next

            Dim sampleNotInTraining As New ImageNetData With {
                .ImagePath = Path.Combine(imagesFolder, "teddy5.jpg")
            }

            Dim sampleNotInTrainingProbs = model.Predict(sampleNotInTraining).PredictedLabels
            Yield New ImageNetData With {
                .ImagePath = sampleNotInTraining.ImagePath,
                .Label = GetLabel(labels, sampleNotInTrainingProbs)
            }
        End Function

        Protected Sub ShowPredictions(imageNetData As IEnumerable(Of ImageNetData))
            Dim defaultForeground = Console.ForegroundColor
            Dim labelColor = ConsoleColor.Green
            Console.WriteLine($"**************************************************************")
            Console.WriteLine($"*       Predictions          ")
            Console.WriteLine($"*-------------------------------------------------------------")
            For Each item In imageNetData
                Console.Write($"      ImagePath: {item.ImagePath} predicted as ")
                Console.ForegroundColor = labelColor
                Console.Write(item.Label)
                Console.ForegroundColor = defaultForeground
                Console.WriteLine("")
            Next item
            Console.WriteLine($"**************************************************************")
        End Sub
    End Class
End Namespace
