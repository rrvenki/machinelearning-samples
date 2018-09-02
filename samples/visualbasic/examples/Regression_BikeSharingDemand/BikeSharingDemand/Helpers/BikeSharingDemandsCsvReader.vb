Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports BikeSharingDemand.BikeSharingDemandData

Namespace Helpers
    Public Class BikeSharingDemandsCsvReader
        Public Function GetDataFromCsv(dataLocation As String) As IEnumerable(Of BikeSharingDemandSample)
            Return File.ReadAllLines(dataLocation).Skip(1).Select(Function(x) x.Split(","c)).Select(Function(x) New BikeSharingDemandSample() With {
            .Season = Single.Parse(x(2)),
            .Year = Single.Parse(x(3)),
            .Month = Single.Parse(x(4)),
            .Hour = Single.Parse(x(5)),
            .Holiday = Integer.Parse(x(6)) <> 0,
            .Weekday = Single.Parse(x(7)),
            .Weather = Single.Parse(x(8)),
            .Temperature = Single.Parse(x(9)),
            .NormalizedTemperature = Single.Parse(x(10)),
            .Humidity = Single.Parse(x(11)),
            .Windspeed = Single.Parse(x(12)),
            .Count = Single.Parse(x(15))
        })
        End Function
    End Class
End Namespace