Namespace GitHubLabeler.DataStructures
    'The only purpose of this class is for peek data after transforming it with the pipeline
    Friend Class GitHubIssue
        Public ID As String

        Public Area As String ' This is an issue label, for example "area-System.Threading"

        Public Title As String

        Public Description As String
    End Class
End Namespace
