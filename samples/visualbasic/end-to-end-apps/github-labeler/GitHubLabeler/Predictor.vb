Imports System.IO
Imports Microsoft.ML.Legacy
Imports Microsoft.ML.Legacy.Data
Imports Microsoft.ML.Legacy.Trainers
Imports Microsoft.ML.Legacy.Transforms

Namespace GitHubLabeler
	Friend Class Predictor
        Private Shared ReadOnly Property AppPath As String
            Get
                Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
            End Get
        End Property

        Private Shared ReadOnly Property DataPath As String
            Get
                Return Path.Combine(AppPath, "datasets", "corefx-issues-train.tsv")
            End Get
        End Property

        Private Shared ReadOnly Property ModelPath As String
            Get
                Return Path.Combine(AppPath, "GitHubLabelerModel.zip")
            End Get
        End Property

        Private Shared _model As PredictionModel(Of GitHubIssue, GitHubIssuePrediction)

		Public Shared Async Function TrainAsync() As Task
			Dim pipeline = New LearningPipeline()

			pipeline.Add((New TextLoader(DataPath)).CreateFrom(Of GitHubIssue)(useHeader:= True))

			pipeline.Add(New Dictionarizer(("Area", "Label")))

			pipeline.Add(New TextFeaturizer("Title", "Title"))

			pipeline.Add(New TextFeaturizer("Description", "Description"))

			pipeline.Add(New ColumnConcatenator("Features", "Title", "Description"))

			pipeline.Add(New StochasticDualCoordinateAscentClassifier())
			pipeline.Add(New PredictedLabelColumnOriginalValueConverter() With {.PredictedLabelColumn = "PredictedLabel"})

			Console.WriteLine("=============== Training model ===============")

			Dim model = pipeline.Train(Of GitHubIssue, GitHubIssuePrediction)()

			Await model.WriteAsync(ModelPath)

			Console.WriteLine("=============== End training ===============")
			Console.WriteLine("The model is saved to {0}", ModelPath)
		End Function

		Public Shared Async Function PredictAsync(issue As GitHubIssue) As Task(Of String)
			If _model Is Nothing Then
				_model = Await PredictionModel.ReadAsync(Of GitHubIssue, GitHubIssuePrediction)(ModelPath)
			End If

			Dim prediction = _model.Predict(issue)

			Return prediction.Area
		End Function
	End Class
End Namespace
