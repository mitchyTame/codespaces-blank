``` ini

BenchmarkDotNet=v0.13.1, OS=debian 11 (container)
AMD EPYC 7763, 1 CPU, 2 logical cores and 1 physical core
.NET SDK=7.0.401
  [Host]     : .NET 7.0.11 (7.0.1123.42427), X64 RyuJIT
  DefaultJob : .NET 7.0.11 (7.0.1123.42427), X64 RyuJIT


```
|                   Method |     Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------- |---------:|---------:|---------:|------:|--------:|
| StringConcatentationTest | 646.5 ns | 17.14 ns | 49.47 ns |  1.00 |    0.00 |
|        StringBuilderTest | 293.2 ns |  5.90 ns | 15.34 ns |  0.46 |    0.04 |
