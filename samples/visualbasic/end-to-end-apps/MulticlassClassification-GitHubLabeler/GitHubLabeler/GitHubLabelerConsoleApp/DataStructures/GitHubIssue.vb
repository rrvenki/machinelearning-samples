Imports Microsoft.ML.Runtime.Api

Namespace GitHubLabeler.DataStructures
	'The only purpose of this class is for peek data after transforming it with the pipeline
	Friend Class GitHubIssue
        <Column("0")>
        Public ID As String

        <Column("1")>
        Public Area As String ' This is an issue label, for example "area-System.Threading"

        <Column("2")>
        Public Title As String

        <Column("3")>
        Public Description As String
    End Class
End Namespace
