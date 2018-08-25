Imports Microsoft.ML.Runtime.Api

Namespace BikeSharingDemandData
    Public Class BikeSharingDemandSample
        <Column("2")>
        Public Season As Single
        <Column("3")>
        Public Year As Single
        <Column("4")>
        Public Month As Single
        <Column("5")>
        Public Hour As Single
        <Column("6")>
        Public Holiday As Boolean
        <Column("7")>
        Public Weekday As Single
        <Column("8")>
        Public Weather As Single
        <Column("9")>
        Public Temperature As Single
        <Column("10")>
        Public NormalizedTemperature As Single
        <Column("11")>
        Public Humidity As Single
        <Column("12")>
        Public Windspeed As Single
        <Column("16")>
        Public Count As Single
    End Class
End Namespace