Imports Octokit

Namespace GitHubLabeler
	Friend Class Labeler
		Private ReadOnly _client As GitHubClient
		Private ReadOnly _repoOwner As String
		Private ReadOnly _repoName As String

        Public Sub New(repoOwner As String, repoName As String, accessToken As String)
            _repoOwner = repoOwner
            _repoName = repoName
            Dim productInformation = New ProductHeaderValue("MLGitHubLabeler")
            _client = New GitHubClient(productInformation) With {.Credentials = New Credentials(accessToken)}
        End Sub

        ' Label all issues that are not labeled yet
        Public Async Function LabelAllNewIssues() As Task
            Dim newIssues = Await GetNewIssues()
            For Each issue In newIssues.Where(Function(issue1) Not issue1.Labels.Any())
                Dim label = Await PredictLabel(issue)
                ApplyLabel(issue, label)
            Next issue
        End Function

        Private Async Function GetNewIssues() As Task(Of IReadOnlyList(Of Issue))
            Dim issueRequest = New RepositoryIssueRequest With {
                .State = ItemStateFilter.Open,
                .Filter = IssueFilter.All,
                .Since = Date.Now.AddMinutes(-10)
            }

            Dim allIssues = Await _client.Issue.GetAllForRepository(_repoOwner, _repoName, issueRequest)

            ' Filter out pull requests and issues that are older than minId
            Return allIssues.Where(Function(i) Not i.HtmlUrl.Contains("/pull/")).ToList()
        End Function

        Private Async Function PredictLabel(issue As Issue) As Task(Of String)
            Dim corefxIssue = New GitHubIssue With {
                .ID = issue.Number.ToString,
                .Title = issue.Title,
                .Description = issue.Body
            }

            Dim predictedLabel = Await Predictor.PredictAsync(corefxIssue)

            Return predictedLabel
        End Function

        Private Sub ApplyLabel(issue As Issue, label As String)
            Dim issueUpdate = New IssueUpdate()
            issueUpdate.AddLabel(label)

            _client.Issue.Update(_repoOwner, _repoName, issue.Number, issueUpdate)

            Console.WriteLine($"Issue {issue.Number} : ""{issue.Title}"" " & vbTab & " was labeled as: {label}")
        End Sub
    End Class
End Namespace
