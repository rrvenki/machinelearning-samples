' Requires following NuGet packages
' NuGet: Microsoft.Extensions.Configuration
' NuGet: Microsoft.Extensions.Configuration.Json
Imports System.IO
Imports Microsoft.Extensions.Configuration

Namespace GitHubLabeler
	Friend Module Program
        Public Property Configuration As IConfiguration

        Sub Main(args() As String)
            MainAsync(args).Wait()
        End Sub

        Public Async Function MainAsync(args() As String) As Task
            Dim builder = New ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")

            Configuration = builder.Build()

            Await Predictor.TrainAsync()

            Await Label()
        End Function

        Private Async Function Label() As Task
			Dim token = Configuration("GitHubToken")
			Dim repoOwner = Configuration("GitHubRepoOwner")
			Dim repoName = Configuration("GitHubRepoName")

			If String.IsNullOrEmpty(token) OrElse String.IsNullOrEmpty(repoOwner) OrElse String.IsNullOrEmpty(repoName) Then
				Console.Error.WriteLine()
				Console.Error.WriteLine("Error: please configure the credentials in the appsettings.json file")
				Console.ReadLine()
				Return
			End If

            Dim labeler As New Labeler(repoOwner, repoName, token)

            Await labeler.LabelAllNewIssues()

			Console.WriteLine("Labeling completed")
			Console.ReadLine()
		End Function
	End Module
End Namespace