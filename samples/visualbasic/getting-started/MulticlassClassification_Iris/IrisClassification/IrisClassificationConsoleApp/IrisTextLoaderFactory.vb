Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data

Namespace MulticlassClassification_Iris
    Public Module IrisTextLoaderFactory
        Public Function CreateTextLoader(mlContext As MLContext) As TextLoader
            Dim textLoader As TextLoader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
                .Separator = vbTab,
                .HasHeader = True,
                .Column = {
                    New TextLoader.Column("Label", DataKind.R4, 0),
                    New TextLoader.Column("SepalLength", DataKind.R4, 1),
                    New TextLoader.Column("SepalWidth", DataKind.R4, 2),
                    New TextLoader.Column("PetalLength", DataKind.R4, 3),
                    New TextLoader.Column("PetalWidth", DataKind.R4, 4)
                }
            })
            Return textLoader
        End Function
    End Module
End Namespace

