Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms.Text


Namespace GitHubLabeler
    Public Class GitHubLabelerDataProcessPipelineFactory
        Public Shared Function CreateDataProcessPipeline(mlContext As MLContext) As IEstimator(Of ITransformer)
            ' Configure data transformations in the Process pipeline

            Return mlContext.Transforms.Categorical.MapValueToKey("Area", "Label").Append(mlContext.Transforms.Text.FeaturizeText("Title", "TitleFeaturized")).Append(mlContext.Transforms.Text.FeaturizeText("Description", "DescriptionFeaturized")).Append(mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized"))
        End Function
    End Class
End Namespace

