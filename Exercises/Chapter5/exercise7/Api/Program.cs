// ============================================================
using System.Diagnostics;

// Bài tập 7: PLINQ — So sánh Sequential vs Parallel
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Đo thời gian xử lý collection 100,000 items
//           bằng sequential LINQ và PLINQ
// ============================================================

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// PLINQ vs Sequential comparison
app.MapGet("/api/linq-parallel", () =>
{
    var data = Enumerable.Range(1, 100_000)
        .Select(i => new { Id = i, Value = Math.Sqrt(i) * Math.Sin(i) })
        .ToList();

    var sw = Stopwatch.StartNew();

    // Sequential LINQ
    var sequential = data
        .Where(x => x.Value > 0)
        .OrderByDescending(x => x.Value)
        .Take(100)
        .ToList();

    sw.Stop();
    var sequentialMs = sw.ElapsedMilliseconds;

    // Parallel LINQ (PLINQ)
    sw.Restart();
    var parallel = data
        .AsParallel()
        .Where(x => x.Value > 0)
        .OrderByDescending(x => x.Value)
        .Take(100)
        .ToList();

    sw.Stop();
    var parallelMs = sw.ElapsedMilliseconds;

    return Results.Ok(new
    {
        itemCount = data.Count,
        sequential = new
        {
            timeMs = sequentialMs,
            resultCount = sequential.Count
        },
        parallel = new
        {
            timeMs = parallelMs,
            resultCount = parallel.Count
        },
        speedup = sequentialMs > 0 ? (double)sequentialMs / parallelMs : 0,
        winner = parallelMs < sequentialMs ? "PLINQ" : "Sequential",
        message = parallelMs < sequentialMs
            ? "PLINQ nhanh hơn — CPU-bound work phù hợp song song hóa"
            : "Sequential nhanh hơn — collection quá nhỏ hoặc overhead parallel lớn"
    });
});

// Demo: CPU-intensive work để thấy rõ sự khác biệt
app.MapGet("/api/linq-parallel/cpu-intensive", () =>
{
    var data = Enumerable.Range(1, 50_000)
        .Select(i => new { Id = i, Value = ComputeHeavy(i) })
        .ToList();

    var sw = Stopwatch.StartNew();

    // Sequential
    var seqResult = data
        .Where(x => x.Value > 0)
        .OrderByDescending(x => x.Value)
        .Take(50)
        .ToList();
    sw.Stop();
    var sequentialMs = sw.ElapsedMilliseconds;

    // Parallel
    sw.Restart();
    var parResult = data
        .AsParallel()
        .Where(x => x.Value > 0)
        .OrderByDescending(x => x.Value)
        .Take(50)
        .ToList();
    sw.Stop();
    var parallelMs = sw.ElapsedMilliseconds;

    return Results.Ok(new
    {
        note = "CPU-intensive workload — PLINQ sẽ rõ ràng hơn",
        sequentialMs,
        parallelMs,
        speedup = sequentialMs > 0 ? Math.Round((double)sequentialMs / parallelMs, 2) : 0,
        caution = "PLINQ KHÔNG đảm bảo thứ tự — dùng AsParallel().AsOrdered() nếu cần order"
    });
});

app.Run();

// CPU-intensive computation
static double ComputeHeavy(int i)
{
    double result = 0;
    for (int j = 0; j < 100; j++)
    {
        result += Math.Sqrt(i + j) * Math.Sin(j) * Math.Cos(i);
    }
    return result;
}
