Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms.Categorical
Imports Microsoft.ML.Transforms.Projections

Namespace CustomerSegmentation
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

        Public Sub New(mlContext As MLContext, Optional rank As Integer = 2)
            ' Configure data transformations in the DataProcess pipeline
            DataProcessPipeline = (New PrincipalComponentAnalysisEstimator(mlContext, "Features", "PCAFeatures", rank:=rank)).Append(New OneHotEncodingEstimator(mlContext, {New OneHotEncodingEstimator.ColumnInfo("LastName", "LastNameKey", CategoricalTransform.OutputKind.Ind)}))
        End Sub
    End Class
End Namespace
