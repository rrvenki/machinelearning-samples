Imports System.IO
Imports Microsoft.ML
Imports Microsoft.ML.Data
Imports Microsoft.ML.Models
Imports Microsoft.ML.Trainers
Imports Microsoft.ML.Transforms

Imports PLplot

Friend Module Program
    Private ReadOnly Property AppPath As String
        Get
            Return Path.GetDirectoryName(Environment.GetCommandLineArgs(0))
        End Get
    End Property
    Private ReadOnly Property TrainDataPath As String
        Get
            Return Path.Combine(AppPath, "datasets", "taxi-fare-train.csv")
        End Get
    End Property
    Private ReadOnly Property TestDataPath As String
        Get
            Return Path.Combine(AppPath, "datasets", "taxi-fare-test.csv")
        End Get
    End Property
    Private ReadOnly Property ModelPath As String
        Get
            Return Path.Combine(AppPath, "TaxiFareModel.zip")
        End Get
    End Property

    Sub Main(args() As String)
        MainAsync(args).Wait()
    End Sub

    Public Async Function MainAsync(args() As String) As Task
        ' If args(0) = "svg" Then a vector-based chart will be created instead a .png chart
        ' STEP 1: Create a model
        Dim model = Await TrainAsync()

        ' STEP2: Test accuracy
        Evaluate(model)

        ' STEP 3: Make a test prediction
        Dim prediction = model.Predict(TestTaxiTrips.Trip1)
        Console.WriteLine($"Predicted fare: {prediction.FareAmount:0.####}, actual fare: 29.5")

        'STEP 4: Paint regression distribution chart for a number of elements read from a Test DataSet file
        PaintChart(model, TestDataPath, 100, args)

        Console.WriteLine("Press any key to exit..")
        Console.ReadLine()
    End Function

    Private Async Function TrainAsync() As Task(Of PredictionModel(Of TaxiTrip, TaxiTripFarePrediction))
        ' LearningPipeline holds all steps of the learning process: data, transforms, learners.
        ' The TextLoader loads a dataset. The schema of the dataset is specified by passing a class containing
        ' all the column names and their types. This will be used to create the model, and train it.

        ' Transforms
        ' When ML model starts training, it looks for two columns: Label and Features.
        ' Label:   values that should be predicted. If you have a field named Label in your data type,
        '              no extra actions required.
        '          If you don’t have it, like in this example, copy the column you want to predict with
        '              ColumnCopier transform:

        ' CategoricalOneHotVectorizer transforms categorical (string) values into 0/1 vectors

        ' Features: all data used for prediction. At the end of all transforms you need to concatenate
        '              all columns except the one you want to predict into Features column with
        '              ColumnConcatenator transform:       
        ' FastTreeRegressor is an algorithm that will be used to train the model.
        Dim pipeline = New LearningPipeline From {
            (New TextLoader(TrainDataPath)).CreateFrom(Of TaxiTrip)(separator:=","c),
            New ColumnCopier(("FareAmount", "Label")),
            New CategoricalOneHotVectorizer("VendorId", "RateCode", "PaymentType"),
            New ColumnConcatenator("Features", "VendorId", "RateCode", "PassengerCount", "TripDistance", "PaymentType"),
            New FastTreeRegressor()
        }

        Console.WriteLine("=============== Training model ===============")
        ' The pipeline is trained on the dataset that has been loaded and transformed.
        Dim model = pipeline.Train(Of TaxiTrip, TaxiTripFarePrediction)()

        ' Saving the model as a .zip file.
        Await model.WriteAsync(ModelPath)

        Console.WriteLine("=============== End training ===============")
        Console.WriteLine("The model is saved to {0}", ModelPath)

        Return model
    End Function

    Private Sub Evaluate(model As PredictionModel(Of TaxiTrip, TaxiTripFarePrediction))
        ' To evaluate how good the model predicts values, it is run against new set
        ' of data (test data) that was not involved in training.
        Dim testData = (New TextLoader(TestDataPath)).CreateFrom(Of TaxiTrip)(separator:=","c)

        ' RegressionEvaluator calculates the differences (in various metrics) between predicted and actual
        ' values in the test dataset.
        Dim evaluator = New RegressionEvaluator()

        Console.WriteLine("=============== Evaluating model ===============")

        Dim metrics = evaluator.Evaluate(model, testData)

        Console.WriteLine($"Rms = {metrics.Rms}, ideally should be around 2.8, can be improved with larger dataset")
        Console.WriteLine($"RSquared = {metrics.RSquared}, a value between 0 and 1, the closer to 1, the better")
        Console.WriteLine("=============== End evaluating ===============")
        Console.WriteLine()
    End Sub

    Private Sub PaintChart(model As PredictionModel(Of TaxiTrip, TaxiTripFarePrediction), testDataSetPath As String, numberOfRecordsToRead As Integer, args() As String)
        Dim chartFileName As String = ""

        Using pl = New PLStream()
            ' use SVG backend and write to SineWaves.svg in current directory
            If args.Length = 1 AndAlso args(0) = "svg" Then
                pl.sdev("svg")
                chartFileName = "TaxiRegressionDistribution.svg"
                pl.sfnam(chartFileName)
            Else
                pl.sdev("pngcairo")
                chartFileName = "TaxiRegressionDistribution.png"
                pl.sfnam(chartFileName)
            End If

            ' use white background with black foreground
            pl.spal0("cmap0_alternate.pal")

            ' Initialize plplot
            pl.init()

            ' set axis limits
            Const xMinLimit As Integer = 0
            Const xMaxLimit As Integer = 40 'Rides larger than $40 are not shown in the chart
            Const yMinLimit As Integer = 0
            Const yMaxLimit As Integer = 40 'Rides larger than $40 are not shown in the chart
            pl.env(xMinLimit, xMaxLimit, yMinLimit, yMaxLimit, AxesScale.Independent, AxisBox.BoxTicksLabelsAxes)

            ' Set scaling for mail title text 125% size of default
            pl.schr(0, 1.25)

            ' The main title
            pl.lab("Measured", "Predicted", "Distribution of Taxi Fare Prediction")

            ' plot using different colors
            ' see http://plplot.sourceforge.net/examples.php?demo=02 for palette indices
            pl.col0(1)

            Dim totalNumber As Integer = numberOfRecordsToRead
            Dim testData = (New TaxiTripCsvReader()).GetDataFromCsv(testDataSetPath, totalNumber).ToList()

            'This code is the symbol to paint
            Dim code As Char = ChrW(9)

            ' plot using other color
            'pl.col0(9); //Light Green
            'pl.col0(4); //Red
            pl.col0(2) 'Blue

            Dim yTotal As Double = 0
            Dim xTotal As Double = 0
            Dim xyMultiTotal As Double = 0
            Dim xSquareTotal As Double = 0

            For i As Integer = 0 To testData.Count - 1
                Dim x = New Double(0) {}
                Dim y = New Double(0) {}
                Dim FarePrediction = model.Predict(testData(i))

                x(0) = testData(i).FareAmount
                y(0) = FarePrediction.FareAmount

                'Paint a dot
                pl.poin(x, y, code)

                xTotal += x(0)
                yTotal += y(0)

                Dim multi As Double = x(0) * y(0)
                xyMultiTotal += multi

                Dim xSquare As Double = x(0) * x(0)
                xSquareTotal += xSquare

                Dim ySquare As Double = y(0) * y(0)

                Console.WriteLine($"-------------------------------------------------")
                Console.WriteLine($"Predicted : {FarePrediction.FareAmount}")
                Console.WriteLine($"Actual:    {testData(i).FareAmount}")
                Console.WriteLine($"-------------------------------------------------")
            Next i

            ' Regression Line calculation explanation:
            ' https://www.khanacademy.org/math/statistics-probability/describing-relationships-quantitative-data/more-on-regression/v/regression-line-example

            Dim minY As Double = yTotal / totalNumber
            Dim minX As Double = xTotal / totalNumber
            Dim minXY As Double = xyMultiTotal / totalNumber
            Dim minXsquare As Double = xSquareTotal / totalNumber

            Dim m As Double = ((minX * minY) - minXY) / ((minX * minX) - minXsquare)

            Dim b As Double = minY - (m * minX)

            'Generic function for Y for the regression line
            ' y = (m * x) + b;

            Dim x1 As Double = 1
            'Function for Y1 in the line
            Dim y1 As Double = (m * x1) + b

            Dim x2 As Double = 39
            'Function for Y2 in the line
            Dim y2 As Double = (m * x2) + b

            Dim xArray = New Double(1) {}
            Dim yArray = New Double(1) {}
            xArray(0) = x1
            yArray(0) = y1
            xArray(1) = x2
            yArray(1) = y2

            pl.col0(4)
            pl.line(xArray, yArray)

            ' end page (writes output to disk)
            pl.eop()

            ' output version of PLplot
            Dim verText As Object = Nothing
            pl.gver(verText)
            Console.WriteLine("PLplot version " & verText)

        End Using ' the pl object is disposed here

        ' Open Chart File In Microsoft Photos App (Or default app, like browser for .svg)

        Console.WriteLine("Showing chart...")
        Dim p = New Process()
        Dim chartFileNamePath As String = ".\" & chartFileName
        p.StartInfo = New ProcessStartInfo(chartFileNamePath) With {.UseShellExecute = True}
        p.Start()
    End Sub

End Module

Public Class TaxiTripCsvReader
    Public Function GetDataFromCsv(dataLocation As String, numMaxRecords As Integer) As IEnumerable(Of TaxiTrip)
        Dim records As IEnumerable(Of TaxiTrip) = File.ReadAllLines(dataLocation).Skip(1).Select(Function(x) x.Split(","c)).Select(Function(x) New TaxiTrip() With {
            .VendorId = x(0),
            .RateCode = x(1),
            .PassengerCount = Single.Parse(x(2)),
            .TripTime = Single.Parse(x(3)),
            .TripDistance = Single.Parse(x(4)),
            .PaymentType = x(5),
            .FareAmount = Single.Parse(x(6))
        }).Take(numMaxRecords)

        Return records
    End Function
End Class