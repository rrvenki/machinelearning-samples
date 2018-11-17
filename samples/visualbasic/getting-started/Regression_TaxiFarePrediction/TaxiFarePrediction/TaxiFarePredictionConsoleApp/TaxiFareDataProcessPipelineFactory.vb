Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports Microsoft.ML.Transforms.Categorical
Imports Microsoft.ML.Transforms.Normalizers.NormalizingEstimator

Namespace Regression_TaxiFarePrediction
    Public Class TaxiFareDataProcessPipelineFactory
        Public Shared Function CreateDataProcessPipeline(mlContext As MLContext) As IEstimator(Of ITransformer)
            ' Configure data transformations in the Process pipeline
            ' In our case, we will one-hot encode as categorical values the VendorId, RateCode and PaymentType
            ' Then concatenate that with the numeric columns.

            Return mlContext.Transforms.CopyColumns("FareAmount", "Label").Append(mlContext.Transforms.Categorical.OneHotEncoding("VendorId", "VendorIdEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("RateCode", "RateCodeEncoded")).Append(mlContext.Transforms.Categorical.OneHotEncoding("PaymentType", "PaymentTypeEncoded")).Append(mlContext.Transforms.Normalize(inputName:="PassengerCount", mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Normalize(inputName:="TripTime", mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Normalize(inputName:="TripDistance", mode:=NormalizerMode.MeanVariance)).Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PaymentTypeEncoded", "PassengerCount", "TripTime", "TripDistance"))
        End Function
    End Class
End Namespace


