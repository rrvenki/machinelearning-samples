Imports Microsoft.ML
Imports Microsoft.ML.Core.Data
Imports Microsoft.ML.Runtime.Data
Imports System.IO

Namespace CreditCardFraudDetection.Common
    Public Module ConsoleExtensions

        <System.Runtime.CompilerServices.Extension>
        Public Sub ToConsole(result As MultiClassClassifierEvaluator.Result)
            Console.WriteLine($"Acuracy macro: {result.AccuracyMacro}")
            Console.WriteLine($"Acuracy micro: {result.AccuracyMicro}")
            Console.WriteLine($"Log loss: {result.LogLoss}")
            Console.WriteLine($"Log loss reduction: {result.LogLossReduction}")
            Console.WriteLine($"Per class log loss: ")
            Dim count As Integer = 0
            For Each logLossClass In result.PerClassLogLoss
                Console.WriteLine(vbTab & $" [{count + 1}]: {logLossClass}")
                count += 1
            Next logLossClass
            Console.WriteLine($"Top K: {result.TopK}")
            Console.WriteLine($"Top K accuracy: {result.TopKAccuracy}")
        End Sub

        <System.Runtime.CompilerServices.Extension> _
        Public Sub ToConsole(result As BinaryClassifierEvaluator.CalibratedResult)
            Console.WriteLine($"Area under ROC curve: {result.Auc}")
            Console.WriteLine($"Area under the precision/recall curve: {result.Auprc}")
            Console.WriteLine($"Entropy: {result.Entropy}")
            Console.WriteLine($"F1 Score: {result.F1Score}")
            Console.WriteLine($"Log loss: {result.LogLoss}")
            Console.WriteLine($"Log loss reduction: {result.LogLossReduction}")
            Console.WriteLine($"Negative precision: {result.NegativePrecision}")
            Console.WriteLine($"Positive precision: {result.PositivePrecision}")
            Console.WriteLine($"Positive recall: {result.PositiveRecall}")

        End Sub

        <System.Runtime.CompilerServices.Extension> _
        Public Sub ToConsole(result As RegressionEvaluator.Result)
            Console.WriteLine($"L1: {result.L1}")
            Console.WriteLine($"L2: {result.L2}")
            Console.WriteLine($"Loss function: {result.LossFn}")
            Console.WriteLine($"Root mean square of the L2 loss: {result.Rms}")
            Console.WriteLine($"R scuared: {result.RSquared}")
        End Sub

        'public static IEnumerable<string> GetColumnNames(this ISchema schema)
        '{
        '    for (int i = 0; i < schema.ColumnCount; i++)
        '    {
        '        if (!schema.IsHidden(i))
        '            yield return schema.GetColumnName(i);
        '    }
        '}

        <System.Runtime.CompilerServices.Extension> _
        Public Sub SaveModel(model As ITransformer, env As MLContext, modelSavePath As String)
            Using stream = File.Create(modelSavePath)
                ' Saving and loading happens to 'dynamic' models, so the static typing is lost in the process.
                model.SaveTo(env, stream)
            End Using
        End Sub

        <System.Runtime.CompilerServices.Extension> _
        Public Function ReadModel(env As MLContext, modelLocation As String) As ITransformer
            Dim model As ITransformer
            Using file = IO.File.OpenRead(modelLocation)
                model = TransformerChain.LoadFrom(env, file)
            End Using
            Return model
        End Function
    End Module

End Namespace
