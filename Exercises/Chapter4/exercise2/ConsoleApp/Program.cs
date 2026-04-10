// ============================================================
using System.Runtime;

// Bài tập 2: Garbage Collector Demo
// Chương 4: C# Fundamentals
// Mục tiêu: Hiểu GC generations, LOH, IDisposable pattern
// ============================================================

Console.WriteLine("=== BÀI TẬP 2: GARBAGE COLLECTOR DEMO ===\n");

// 1. GC Generations Demo
GcGenerationsDemo.Run();

Console.WriteLine();

// 2. Large Object Heap (LOH) Demo
LohDemo.Run();

Console.WriteLine();

// 3. IDisposable Pattern Demo
DisposableDemo.Run();

Console.WriteLine();

// 4. Finalizer vs IDisposable Demo
FinalizerDemo.Run();

Console.WriteLine();

// 5. GC Tuning Info
GcTuningDemo.Run();

Console.WriteLine();

// ----------------------------------------------------------
// Demo 1: GC Generations
// ----------------------------------------------------------
public static class GcGenerationsDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 1. GC Generations ---");

        var obj1 = new SampleObject("Obj1");
        Console.WriteLine($"  Tạo obj1 → Gen: {GC.GetGeneration(obj1)}");

        GC.Collect(0, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"  Sau GC(0): obj1 → Gen: {GC.GetGeneration(obj1)}");

        GC.Collect(1, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"  Sau GC(1): obj1 → Gen: {GC.GetGeneration(obj1)}");

        GC.Collect(2, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"  Sau GC(2): obj1 → Gen: {GC.GetGeneration(obj1)}");

        Console.WriteLine($"  → Objects được promote qua các thế hệ khi sống sót qua GC");
    }
}

// ----------------------------------------------------------
// Demo 2: Large Object Heap
// ----------------------------------------------------------
public static class LohDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 2. Large Object Heap (LOH) ---");

        var small = new byte[1000];
        var large = new byte[100_000];

        Console.WriteLine($"  Mảng 1,000 bytes  → Gen: {GC.GetGeneration(small)}");
        Console.WriteLine($"  Mảng 100,000 bytes → Gen: {GC.GetGeneration(large)}");
        Console.WriteLine($"  → Objects ≥ 85,000 bytes được allocate trực tiếp vào Gen 2 (LOH)");
    }
}

// ----------------------------------------------------------
// Demo 3: IDisposable Pattern
// ----------------------------------------------------------
public static class DisposableDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 3. IDisposable Pattern ---");

        Console.WriteLine("  Sử dụng using block (C# 1+):");
        using (var holder = new ResourceHolder("BlockResource"))
        {
            Console.WriteLine($"    Đang sử dụng {holder.Name}...");
        } // Dispose() tự động gọi

        Console.WriteLine("  Sử dụng using declaration (C# 8+):");
        using var holder2 = new ResourceHolder("DeclarationResource");
        Console.WriteLine($"    Đang sử dụng {holder2.Name}...");
        // Dispose() tự động gọi khi hết scope method
    }
}

// ----------------------------------------------------------
// Demo 4: Finalizer vs IDisposable
// ----------------------------------------------------------
public static class FinalizerDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 4. Finalizer vs IDisposable ---");

        Console.WriteLine("  Tạo DisposableWithFinalizer...");
        using var resource = new DisposableWithFinalizer();
        Console.WriteLine("  Gọi Dispose()...");
        // resource.Dispose() được gọi bởi using
        // GC.SuppressFinalize() trong Dispose() → finalizer KHÔNG chạy

        Console.WriteLine("  Tạo object không Dispose...");
        _ = new DisposableWithFinalizer();
        // Finalizer sẽ chạy khi GC thu hồi object
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("  → GC đã gọi finalizer cho object không Dispose");
    }
}

// ----------------------------------------------------------
// Demo 5: GC Tuning Info
// ----------------------------------------------------------
public static class GcTuningDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 5. GC Tuning Info ---");
        Console.WriteLine($"  Is Server GC: {GCSettings.IsServerGC}");
        Console.WriteLine($"  GCLatencyMode: {GCSettings.LatencyMode}");
        Console.WriteLine($"  Total memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
        Console.WriteLine("  → ASP.NET Core mặc định dùng Server GC cho throughput cao");
    }
}

// ----------------------------------------------------------
// Helper classes
// ----------------------------------------------------------
public class SampleObject(string name)
{
    public string Name { get; } = name;
}

public class ResourceHolder(string name) : IDisposable
{
    public string Name { get; } = name;
    private bool _disposed;

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.WriteLine($"    [ResourceHolder] Đóng & giải phóng '{Name}'");
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

public class DisposableWithFinalizer : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        Console.WriteLine("  [DisposableWithFinalizer] Dispose() gọi - finalizer bị Suppress");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                Console.WriteLine("    → Giải phóng managed resources");
            Console.WriteLine("    → Giải phóng unmanaged resources");
            _disposed = true;
        }
    }

    ~DisposableWithFinalizer()
    {
        Console.WriteLine("  [DisposableWithFinalizer] Finalizer chạy (Dispose chưa được gọi!)");
        Dispose(false);
    }
}
