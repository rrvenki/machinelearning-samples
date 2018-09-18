Imports System.IO

Namespace Model
    Public Module ModelHelpers
        Private currentAssemblyLocation As New FileInfo(GetType(Program).Assembly.Location)
        Private ReadOnly _dataRoot As String = Path.Combine(currentAssemblyLocation.Directory.FullName, "assets")

        Public Function GetAssetsPath(ParamArray paths() As String) As String
            If paths Is Nothing OrElse paths.Length = 0 Then
                Return Nothing
            End If

            Return Path.GetFullPath(Path.Combine(paths.Prepend(_dataRoot).ToArray()))
        End Function

        Public Function DeleteAssets(ParamArray paths() As String) As String
            Dim location = GetAssetsPath(paths)

            If Not String.IsNullOrWhiteSpace(location) AndAlso File.Exists(location) Then
                File.Delete(location)
            End If
            Return location
        End Function

        Public Function Columns(Of T As Class)() As IEnumerable(Of String)
            Return GetType(T).GetProperties.Select(Function(p) p.Name)
        End Function

        Public Function Columns(Of T As Class, U)() As IEnumerable(Of String)
            Dim typeofU = GetType(U)
            Return From c In GetType(T).GetProperties
                   Where c.PropertyType Is typeofU
                   Select c.Name
        End Function

        Public Function Columns(Of T As Class, U, V)() As IEnumerable(Of String)
            Dim typeofUV = {GetType(U), GetType(V)}
            Return From c In GetType(T).GetProperties
                   Where typeofUV.Contains(c.PropertyType)
                   Select c.Name
        End Function

        Public Function Columns(Of T As Class, U, V, W)() As IEnumerable(Of String)
            Dim typeofUVW = {GetType(U), GetType(V), GetType(W)}
            Return From c In GetType(T).GetProperties
                   Where typeofUVW.Contains(c.PropertyType)
                   Select c.Name
        End Function

        Public Function ColumnsNumerical(Of T As Class)() As String()
            Return Columns(Of T, Single, Integer).ToArray()
        End Function

        Public Function ColumnsString(Of T As Class)() As String()
            Return Columns(Of T, String).ToArray()
        End Function

        Public Function ColumnsDateTime(Of T As Class)() As String()
            Return Columns(Of T, Date).ToArray()
        End Function

        Public Function GetLabel(labels() As String, probs() As Single) As String
            Dim index = probs.AsSpan.IndexOf(probs.Max)
            Return labels(index)
        End Function
    End Module
End Namespace
