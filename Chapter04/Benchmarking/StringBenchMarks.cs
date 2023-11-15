using BenchmarkDotNet.Attributes;
namespace Benchmarking;

public class StringBenchMarks
{
    int[] numbers;

    public StringBenchMarks()
    {
        numbers = Enumerable.Range(start: 1, count: 20).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string StringConcatentationTest()
    {
        string s = string.Empty;

        for(int i = 0; i < numbers.Length; i++)
        {
            s += numbers[i] + ",";
        }

        return s;
    }

    [Benchmark]
    public string StringBuilderTest()
    {
        System.Text.StringBuilder builder = new();

        for(int i = 0; i < numbers.Length; i++)
        {
            builder.Append(numbers[i]);
            builder.Append(",");
        }

        return builder.ToString();
    }
}
