Imports CustomerSegmentation.Model
Imports System.IO
Imports Common
Imports Microsoft.ML

Namespace CustomerSegmentation
    Public Class Program
        Shared Sub Main(args() As String)
            Dim assetsPath = "..\..\..\assets"
            Dim pivotCsv = Path.Combine(assetsPath, "inputs", "pivot.csv")
            Dim modelZipFilePath = Path.Combine(assetsPath, "inputs", "retailClustering.zip")
            Dim plotSvg = Path.Combine(assetsPath, "outputs", "customerSegmentation.svg")
            Dim plotCsv = Path.Combine(assetsPath, "outputs", "customerSegmentation.csv")

            Try
                Dim mlContext As New MLContext(seed:=1) 'Seed set to any number so you have a deterministic results

                'Create the clusters: Create data files and plot a chart
                Dim clusteringModelScorer = New ClusteringModelScorer(mlContext, pivotCsv, plotSvg, plotCsv)
                clusteringModelScorer.LoadModelFromZipFile(modelZipFilePath)

                clusteringModelScorer.CreateCustomerClusters()
            Catch ex As Exception
                ConsoleWriteException(ex.Message)
            End Try

            ConsolePressAnyKey()
        End Sub
    End Class
End Namespace
