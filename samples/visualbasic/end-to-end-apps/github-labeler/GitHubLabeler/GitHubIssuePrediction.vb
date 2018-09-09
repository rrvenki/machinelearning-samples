Imports Microsoft.ML.Runtime.Api

#Disable Warning IDE0044 ' We don't care about unsused fields here, because they are mapped with the input file.

Namespace GitHubLabeler
	Friend Class GitHubIssuePrediction
		<ColumnName("PredictedLabel")>
		Public Area As String
	End Class
End Namespace
