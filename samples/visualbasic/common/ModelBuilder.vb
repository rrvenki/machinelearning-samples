Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Api
Imports Microsoft.ML.Runtime.Data

Namespace Common
	Public Class ModelBuilder(Of TObservation As Class, TPrediction As {Class, New})
		Private _mlcontext As MLContext
		Private privateTrainingPipeline As IEstimator(Of ITransformer)
		Public Property TrainingPipeline() As IEstimator(Of ITransformer)
			Get
				Return privateTrainingPipeline
			End Get
			Private Set(ByVal value As IEstimator(Of ITransformer))
				privateTrainingPipeline = value
			End Set
		End Property
		Private privateTrainedModel As ITransformer
		Public Property TrainedModel() As ITransformer
			Get
				Return privateTrainedModel
			End Get
			Private Set(ByVal value As ITransformer)
				privateTrainedModel = value
			End Set
		End Property

		Public Sub New(ByVal mlContext As MLContext, ByVal dataProcessPipeline As IEstimator(Of ITransformer))
			_mlcontext = mlContext
			TrainingPipeline = dataProcessPipeline
		End Sub

		Public Sub AddTrainer(ByVal trainer As IEstimator(Of ITransformer))
			Me.AddEstimator(trainer)
		End Sub

		Public Sub AddEstimator(ByVal estimator As IEstimator(Of ITransformer))
			TrainingPipeline = TrainingPipeline.Append(estimator)
		End Sub

		Public Function Train(ByVal trainingData As IDataView) As ITransformer
			TrainedModel = TrainingPipeline.Fit(trainingData)
			Return TrainedModel
		End Function

		Public Function EvaluateRegressionModel(ByVal testData As IDataView, ByVal label As String, ByVal score As String) As RegressionEvaluator.Result
			CheckTrained()
			Dim predictions = TrainedModel.Transform(testData)
			Dim metrics = _mlcontext.Regression.Evaluate(predictions, label:= label, score:= score)
			Return metrics
		End Function

		Public Function EvaluateBinaryClassificationModel(ByVal testData As IDataView, ByVal label As String, ByVal score As String) As BinaryClassifierEvaluator.Result
			CheckTrained()
			Dim predictions = TrainedModel.Transform(testData)
			Dim metrics = _mlcontext.BinaryClassification.Evaluate(predictions, label:=label, score:=score)
			Return metrics
		End Function

		Public Function EvaluateMultiClassClassificationModel(ByVal testData As IDataView, Optional ByVal label As String ="Label", Optional ByVal score As String ="Score") As MultiClassClassifierEvaluator.Result
			CheckTrained()
			Dim predictions = TrainedModel.Transform(testData)
			Dim metrics = _mlcontext.MulticlassClassification.Evaluate(predictions, label:= label, score:= score)
			Return metrics
		End Function

        Public Function CrossValidateAndEvaluateMulticlassClassificationModel(ByVal data As IDataView, Optional ByVal numFolds As Integer = 5, Optional ByVal labelColumn As String = "Label", Optional ByVal stratificationColumn As String = Nothing) As (metrics As MultiClassClassifierEvaluator.Result, model As ITransformer, scoredTestData As IDataView)()
            'CrossValidation happens actually before training, so no check here.

            'Cross validate
            Dim crossValidationResults = _mlcontext.MulticlassClassification.CrossValidate(data, TrainingPipeline, numFolds, labelColumn, stratificationColumn)

            'Another way to do it:
            'Dim context As New MulticlassClassificationContext(_mlcontext)
            'Dim crossValidationResults = context.CrossValidate(data, TrainingPipeline, numFolds, labelColumn, stratificationColumn)

            Return crossValidationResults
        End Function

        Public Function EvaluateClusteringModel(ByVal dataView As IDataView) As ClusteringEvaluator.Result
			CheckTrained()
			Dim predictions = TrainedModel.Transform(dataView)

			Dim metrics = _mlcontext.Clustering.Evaluate(predictions, score:="Score", features:= "Features")
			Return metrics
		End Function

		Public Sub SaveModelAsFile(ByVal persistedModelPath As String)
			CheckTrained()

			Using fs = New FileStream(persistedModelPath, FileMode.Create, FileAccess.Write, FileShare.Write)
				_mlcontext.Model.Save(TrainedModel, fs)
			End Using
			Console.WriteLine("The model is saved to {0}", persistedModelPath)
		End Sub

		Private Sub CheckTrained()
			If TrainedModel Is Nothing Then
				Throw New InvalidOperationException("Cannot test before training. Call Train() first.")
			End If
		End Sub

	End Class
End Namespace
