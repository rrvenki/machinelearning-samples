Imports Microsoft.ML.Runtime.Api

Namespace GitHubLabeler.DataStructures
	Friend Class GitHubIssueTransformed
		Public ID As String
		Public Area As String
		'public float[] Label;                 // -> Area dictionarized
		Public Title As String
		'public float[] TitleFeaturized;       // -> Title Featurized 
		Public Description As String
		'public float[] DescriptionFeaturized; // -> Description Featurized 
	End Class
End Namespace


'public Scalar<bool> label { get; set; }
'public Scalar<float> score { get; set; }