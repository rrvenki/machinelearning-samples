Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data

Namespace TitanicSurvivalConsoleApp
    Friend Class DataLoader
        Private _mlContext As MLContext
        Private _loader As TextLoader

        Public Sub New(mlContext As MLContext)
            _mlContext = mlContext

            _loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
                .Separator = ",",
                .HasHeader = True,
                .Column = {
                    New TextLoader.Column("PassengerId", DataKind.R4, 0),
                    New TextLoader.Column("Label", DataKind.R4, 1),
                    New TextLoader.Column("Pclass", DataKind.R4, 2),
                    New TextLoader.Column("Name", DataKind.Text, 3),
                    New TextLoader.Column("Sex", DataKind.Text, 4),
                    New TextLoader.Column("Age", DataKind.R4, 5),
                    New TextLoader.Column("SibSp", DataKind.R4, 6),
                    New TextLoader.Column("Parch", DataKind.R4, 7),
                    New TextLoader.Column("Ticket", DataKind.Text, 8),
                    New TextLoader.Column("Fare", DataKind.R4, 9),
                    New TextLoader.Column("Cabin", DataKind.Text, 10),
                    New TextLoader.Column("Embarked", DataKind.Text, 11)
                }
            })
        End Sub

        Public Function GetDataView(filePath As String) As IDataView
            Return _loader.Read(filePath)
        End Function
    End Class
End Namespace

'new TextLoader.Column(nameof(TitanicData.PassengerId), DataKind.R4, 0),
'new TextLoader.Column(nameof(TitanicData.Label), DataKind.R4, 1),
'new TextLoader.Column(nameof(TitanicData.Pclass), DataKind.R4, 2),
'new TextLoader.Column(nameof(TitanicData.Name), DataKind.Text, 3),
'new TextLoader.Column(nameof(TitanicData.Sex), DataKind.Text, 4),
'new TextLoader.Column(nameof(TitanicData.Age), DataKind.R4, 5),
'new TextLoader.Column(nameof(TitanicData.SibSp), DataKind.R4, 6),
'new TextLoader.Column(nameof(TitanicData.Parch), DataKind.R4, 7),
'new TextLoader.Column(nameof(TitanicData.Ticket), DataKind.Text, 8),
'new TextLoader.Column(nameof(TitanicData.Fare), DataKind.R4, 9),
'new TextLoader.Column(nameof(TitanicData.Cabin), DataKind.Text, 10),
'new TextLoader.Column(nameof(TitanicData.Embarked), DataKind.Text, 11)