Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data

Namespace GitHubLabeler
    Public Module GitHubLabelerTextLoaderFactory
        Public Function CreateTextLoader(mlContext As MLContext) As TextLoader
            Dim textLoader As TextLoader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
                .Separator = "tab",
                .HasHeader = True,
                .Column = {
                    New TextLoader.Column("ID", DataKind.Text, 0),
                    New TextLoader.Column("Area", DataKind.Text, 1),
                    New TextLoader.Column("Title", DataKind.Text, 2),
                    New TextLoader.Column("Description", DataKind.Text, 3)
                }
            })

            Return textLoader
        End Function
    End Module

End Namespace
