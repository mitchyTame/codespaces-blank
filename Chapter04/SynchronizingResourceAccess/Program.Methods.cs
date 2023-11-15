using SynchronizingResourceAccess;

partial class Program
{

    static void MethodA()
    {
        try
        {
            if (Monitor.TryEnter(SharedObjects.Conch, TimeSpan.FromSeconds(15)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(Random.Shared.Next(2000));
                    SharedObjects.Message += "A";
                    Write(".");
                    SharedObjects.Counter++;
                }
            }
            else
            {
                WriteLine("Method A timed out when entering a monitor on the conch.");
            }
        }
        finally
        {
            Monitor.Exit(SharedObjects.Conch);
        }


    }

    static void MethodB()
    {
        try
        {
            if (Monitor.TryEnter(SharedObjects.Conch, TimeSpan.FromSeconds(15)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(Random.Shared.Next(2000));
                    SharedObjects.Message += "B";
                    Write(".");
                    SharedObjects.Counter++;
                }
            }
            else
            {
                WriteLine("Method B timed out when entering a monitor on the conch.");
            }
        }
        finally
        {
            Monitor.Exit(SharedObjects.Conch);
        }


    }
}
