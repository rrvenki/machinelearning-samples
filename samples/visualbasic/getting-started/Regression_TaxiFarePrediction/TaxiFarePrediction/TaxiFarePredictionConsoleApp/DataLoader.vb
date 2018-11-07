Imports Microsoft.ML
Imports Microsoft.ML.Runtime.Data


Namespace Regression_TaxiFarePrediction
	Friend Class DataLoader
		Private _mlContext As MLContext
		Private _loader As TextLoader

		Public Sub New(ByVal mlContext As MLContext)
			_mlContext = mlContext

			_loader = mlContext.Data.TextReader(New TextLoader.Arguments() With {
				.Separator = ",",
				.HasHeader = True,
				.Column = {
					New TextLoader.Column("VendorId", DataKind.Text, 0),
					New TextLoader.Column("RateCode", DataKind.Text, 1),
					New TextLoader.Column("PassengerCount", DataKind.R4, 2),
					New TextLoader.Column("TripTime", DataKind.R4, 3),
					New TextLoader.Column("TripDistance", DataKind.R4, 4),
					New TextLoader.Column("PaymentType", DataKind.Text, 5),
					New TextLoader.Column("FareAmount", DataKind.R4, 6)
				}
			})
		End Sub

		Public Function GetDataView(ByVal filePath As String) As IDataView
			Return _loader.Read(filePath)
		End Function
	End Class
End Namespace

