Imports System
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks

Imports Microsoft.ML.Legacy
Imports Microsoft.ML.Legacy.Models
Imports Microsoft.ML.Legacy.Data
Imports Microsoft.ML.Legacy.Transforms
Imports Microsoft.ML.Legacy.Trainers

Namespace BinaryClasification_TitanicSurvivalPrediction

	Friend Module Program
		Private ReadOnly Property AppPath() As String
			Get
				Return Path.GetDirectoryName(Environment.GetCommandLineArgs()(0))
			End Get
		End Property
		Private ReadOnly Property TrainDataPath() As String
			Get
				Return Path.Combine(AppPath, "datasets", "titanic-train.csv")
			End Get
		End Property
		Private ReadOnly Property TestDataPath() As String
			Get
				Return Path.Combine(AppPath, "datasets", "titanic-test.csv")
			End Get
		End Property
		Private ReadOnly Property ModelPath() As String
			Get
				Return Path.Combine(AppPath, "TitanicModel.zip")
			End Get
		End Property

		Public Async Function Main(ByVal args() As String) As Task
			' STEP 1: Create a model
			Dim model = Await TrainAsync()

			' STEP2: Test accuracy
			Evaluate(model)

			' STEP 3: Make a prediction
			Dim prediction = model.Predict(TestTitanicData.Passenger)
			Console.WriteLine($"Did this passenger survive?   Actual: Yes   Predicted: {(If(prediction.Survived, "Yes", "No"))} with {prediction.Probability*100}% probability")

			Console.ReadLine()
		End Function

		Public Async Function TrainAsync() As Task(Of PredictionModel(Of TitanicData, TitanicPrediction))
			' LearningPipeline holds all steps of the learning process: data, transforms, learners.  
			Dim pipeline = New LearningPipeline()

			' The TextLoader loads a dataset. The schema of the dataset is specified by passing a class containing
			' all the column names and their types.
			pipeline.Add((New TextLoader(TrainDataPath)).CreateFrom(Of TitanicData)(useHeader:= True, separator:= ","c))

			' Transform low-dimensional categorical columns to one-hot indicators
			pipeline.Add(New CategoricalOneHotVectorizer("Sex", "Cabin", "Pclass", "SibSp", "Parch", "Embarked"))

			' Transform high-dimensional categorical columns to one-hot hash indicators
			pipeline.Add(New CategoricalHashOneHotVectorizer("Ticket", "Cabin") With {.HashBits = 2})

			' Put all features into a vector
			pipeline.Add(New ColumnConcatenator("Features", "Pclass", "Sex", "Age", "SibSp", "Parch", "Ticket", "Fare", "Cabin", "Embarked"))

			' FastTreeBinaryClassifier is an algorithm that will be used to train the model.
			' It has three hyperparameters for tuning decision tree performance. 
			pipeline.Add(New FastTreeBinaryClassifier()) ' {NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2});

			Console.WriteLine("=============== Training model ===============")
			' The pipeline is trained on the dataset that has been loaded and transformed.
			Dim model = pipeline.Train(Of TitanicData, TitanicPrediction)()

			' Saving the model as a .zip file.
			Await model.WriteAsync(ModelPath)

			Console.WriteLine("=============== End training ===============")
			Console.WriteLine("The model is saved to {0}", ModelPath)

			Return model
		End Function

		Private Sub Evaluate(ByVal model As PredictionModel(Of TitanicData, TitanicPrediction))
			' To evaluate how good the model predicts values, the model is ran against new set
			' of data (test data) that was not involved in training.
			Dim testData = (New TextLoader(TestDataPath)).CreateFrom(Of TitanicData)(useHeader:= True, separator:= ","c)

			' BinaryClassificationEvaluator performs evaluation for Binary Classification type of ML problems.
			Dim evaluator = New BinaryClassificationEvaluator()

			Console.WriteLine("=============== Evaluating model ===============")

			Dim metrics = evaluator.Evaluate(model, testData)
			' BinaryClassificationMetrics contains the overall metrics computed by binary classification evaluators
			' The Accuracy metric gets the accuracy of a classifier which is the proportion 
			'of correct predictions in the test set.

			' The Auc metric gets the area under the ROC curve.
			' The area under the ROC curve is equal to the probability that the classifier ranks
			' a randomly chosen positive instance higher than a randomly chosen negative one
			' (assuming 'positive' ranks higher than 'negative').

			' The F1Score metric gets the classifier's F1 score.
			' The F1 score is the harmonic mean of precision and recall:
			'  2 * precision * recall / (precision + recall).

			Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}")
			Console.WriteLine($"Auc: {metrics.Auc:P2}")
			Console.WriteLine($"F1Score: {metrics.F1Score:P2}")
			Console.WriteLine("=============== End evaluating ===============")
			Console.WriteLine()
		End Sub
	End Module
End Namespace