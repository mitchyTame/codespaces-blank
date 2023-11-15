using System.Diagnostics;
using static System.Diagnostics.Process;

namespace MonitoringApp;

public class Recorder
{
    private static Stopwatch timer = new();

    private static long bytesPhysicalBefore = 0;
    private static long bytesVirtualBefore = 0;

    public static void Start()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        bytesPhysicalBefore = GetCurrentProcess().WorkingSet64;
        bytesVirtualBefore = GetCurrentProcess().VirtualMemorySize64;

        timer.Restart();

    }

    public static void Stop()
    {
        timer.Stop();

        long bytesPhysicalAfter = GetCurrentProcess().WorkingSet64;
        long bytesVirtualAfter = GetCurrentProcess().VirtualMemorySize64;

        WriteLine("{0} physical bytes used.", bytesPhysicalAfter = bytesPhysicalBefore);
        WriteLine("{0} virtual bytes used.", bytesVirtualBefore - bytesVirtualAfter);
        WriteLine("{0} time elapsed.", timer.Elapsed);
        WriteLine("{0} total milliseconds elapsed.", timer.ElapsedMilliseconds);
    }
}
