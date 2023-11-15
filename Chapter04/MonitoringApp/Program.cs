// See https://aka.ms/new-console-template for more information
using MonitoringApp;

WriteLine("Processing. Please wait....");

Recorder.Start();

int[] largeArrayOfInts = Enumerable.Range(start:1, count: 1_000_000).ToArray();
Thread.Sleep(new Random().Next(5,10)* 1000);
Recorder.Stop();



int[] numbers = Enumerable.Range(start:1, count:50_000).ToArray();

SectionTitle("Using StringBuilder");

Recorder.Start();

System.Text.StringBuilder builder = new();

for(int i=0; i< numbers.Length; i++)
{
    builder.Append(numbers[i]);
    builder.Append(",");
}

Recorder.Stop();

SectionTitle("Using strings with +");
Recorder.Start();


string s = String.Empty;

for(int i=0; i < numbers.Length; i++)
{
    s += numbers[i] + ",";
}

Recorder.Stop();