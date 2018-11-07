Imports Microsoft.ML
Imports Microsoft.ML.Core.Data

Namespace Clustering_Iris
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
            ' Configure data transformations in the DataProcess pipeline
            DataProcessPipeline = mlContext.Transforms.Concatenate("Features", "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
        End Sub
    End Class
End Namespace
