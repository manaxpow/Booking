// ============================================================
using System.Diagnostics;
using System.Text;

// Bài tập 5: Source Generators Concept
// Chương 4: C# Fundamentals
// Mục tiêu: Hiểu khái niệm source generators, so sánh với reflection
// ============================================================

Console.WriteLine("=== BÀI TẬP 5: SOURCE GENERATORS CONCEPT ===\n");

// 1. Source-generated ToString (simulate)
SourceGeneratedDemo.Run();

Console.WriteLine();

// 2. Reflection-based ToString
ReflectionBasedDemo.Run();

Console.WriteLine();

// 3. Performance Benchmark
BenchmarkDemo.Run();

Console.WriteLine();

// 4. So sánh tổng kết
Console.WriteLine("--- 4. So sánh tổng kết ---");
Console.WriteLine("  Source Generators:");
Console.WriteLine("    + Compile-time code generation → runtime KHÔNG overhead");
Console.WriteLine("    + Type-safe, AOT-compatible");
Console.WriteLine("    + Được dùng trong ASP.NET Core Minimal APIs, JSON serialization");
Console.WriteLine("  Reflection:");
Console.WriteLine("    + Linh hoạt, chạy lúc runtime");
Console.WriteLine("    + Chậm hơn, không AOT-compatible");
Console.WriteLine("    + Vẫn cần thiết cho dynamic scenarios");

Console.WriteLine();

// ----------------------------------------------------------
// Demo 1: Source-generated ToString (simulate)
// ----------------------------------------------------------
public static class SourceGeneratedDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 1. Source-generated ToString (simulate) ---");

        var person = new Person("Nguyễn Văn A", 25, "nva@email.com");

        // "Source-generated" — code được generate lúc compile time
        // Trong thực tế, source generator sẽ generate method này tự động
        var result = SourceGenToString(person);
        Console.WriteLine($"  {result}");
        Console.WriteLine("  → Code được generate compile-time, không dùng reflection");
    }

    // Simulate what a source generator would produce
    private static string SourceGenToString(Person p)
        => $"Person {{ Name = \"{p.Name}\", Age = {p.Age}, Email = \"{p.Email}\" }}";
}

// ----------------------------------------------------------
// Demo 2: Reflection-based ToString
// ----------------------------------------------------------
public static class ReflectionBasedDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 2. Reflection-based ToString ---");

        object person = new Person("Nguyễn Văn A", 25, "nva@email.com");

        // Reflection — chạy lúc runtime
        var result = ReflectionToString(person);
        Console.WriteLine($"  {result}");
        Console.WriteLine("  → Dùng reflection, overhead lúc runtime");
    }

    private static string ReflectionToString(object obj)
    {
        var sb = new StringBuilder();
        sb.Append(obj.GetType().Name);
        sb.Append(" { ");
        foreach (var prop in obj.GetType().GetProperties())
        {
            sb.Append($"{prop.Name}={prop.GetValue(obj)} ");
        }
        sb.Append("}");
        return sb.ToString().Trim();
    }
}

// ----------------------------------------------------------
// Demo 3: Performance Benchmark
// ----------------------------------------------------------
public static class BenchmarkDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 3. Performance Benchmark ---");

        var person = new Person("Nguyễn Văn A", 25, "nva@email.com");
        const int iterations = 100_000;
        var sw = new Stopwatch();

        // Source-generated (simulated)
        sw.Start();
        for (int i = 0; i < iterations; i++)
            _ = SourceGenToString(person);
        sw.Stop();
        long sourceGenTime = sw.ElapsedMilliseconds;

        // Reflection-based
        sw.Restart();
        for (int i = 0; i < iterations; i++)
            _ = ReflectionToString(person);
        sw.Stop();
        long reflectionTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"  Source Gen:  {sourceGenTime} ms ({iterations:N0} iterations)");
        Console.WriteLine($"  Reflection: {reflectionTime} ms ({iterations:N0} iterations)");
        Console.WriteLine($"  → Source-generated nhanh hơn đáng kể vì không có reflection overhead");
    }

    private static string SourceGenToString(Person p)
        => $"Person {{ Name = \"{p.Name}\", Age = {p.Age}, Email = \"{p.Email}\" }}";

    private static string ReflectionToString(object obj)
    {
        var sb = new StringBuilder();
        sb.Append(obj.GetType().Name);
        sb.Append(" { ");
        foreach (var prop in obj.GetType().GetProperties())
        {
            sb.Append($"{prop.Name}={prop.GetValue(obj)} ");
        }
        sb.Append("}");
        return sb.ToString().Trim();
    }
}

// ----------------------------------------------------------
// Record type cho demo
// ----------------------------------------------------------
public record Person(string Name, int Age, string Email);
