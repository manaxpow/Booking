using System.Diagnostics;

public static class BoxingDemo
{
    public static void Main()
    {
        const int iterations = 10_000_000;
        var sw = new Stopwatch();

        sw.Restart();
        int total = 0;
        for (int i = 0; i < iterations; i++)
        {
            object box = i;
            total += (int)box;
        }
        long boxingTime = sw.ElapsedMilliseconds;

        sw.Restart();
        total = 0;
        for (int i = 0; i < iterations; i++)
            total += i;
        long noBoxingTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"CÓ boxing:    {boxingTime} ms");
        Console.WriteLine($"KHÔNG boxing: {noBoxingTime} ms");
    }
}