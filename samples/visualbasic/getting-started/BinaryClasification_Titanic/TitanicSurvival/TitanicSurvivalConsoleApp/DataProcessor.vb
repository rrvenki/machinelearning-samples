Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms.Categorical

Namespace TitanicSurvivalConsoleApp
    Public Class DataProcessor
        Private privateDataProcessPipeline As IEstimator(Of ITransformer)
        Public Property DataProcessPipeline() As IEstimator(Of ITransformer)
            Get
                Return privateDataProcessPipeline
            End Get
            Private Set(value As IEstimator(Of ITransformer))
                privateDataProcessPipeline = value
            End Set
        End Property

        Public Sub New(mlContext As MLContext)
            ' Configure data transformations in the Process pipeline
            ' In our case, we will one-hot encode as categorical values 
            ' Then concatenate those encoded values into a new "features" column. 

            DataProcessPipeline = mlContext.Transforms.Categorical.OneHotEncoding("Sex", "SexEncoded").Append(mlContext.Transforms.Categorical.OneHotEncoding("Age", "AgeEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Cabin", "CabinEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Pclass", "PclassEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("SibSp", "SibSpEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Parch", "ParchEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Embarked", "EmbarkedEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Ticket", "TicketEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Fare", "FareEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("Cabin", "CabinEncoded")).Append(mlContext.Transforms.Concatenate("Features", "PclassEncoded", "SexEncoded", "AgeEncoded", "SibSpEncoded", "ParchEncoded", "TicketEncoded", "FareEncoded", "CabinEncoded", "EmbarkedEncoded"))
        End Sub
    End Class
End Namespace


