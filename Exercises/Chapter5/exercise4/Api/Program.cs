// ============================================================
// Bài tập 4: Deferred vs Immediate Execution
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Hiểu sự khác biệt giữa deferred execution (IEnumerable) và
//           immediate execution (ToList, ToArray, Count, etc.)
// ============================================================

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Demo: Deferred Execution
app.MapGet("/api/deferred-demo", () =>
{
    var numbers = new List<int> { 1, 2, 3, 4, 5 };

    // DEFERRED: Where trả về IEnumerable — CHƯA chạy query
    var deferred = numbers.Where(n => n > 2);

    // Thêm phần tử SAUC khi tạo deferred query
    numbers.Add(10);

    // Giờ enumerate → query MỚI chạy, include phần tử mới
    var deferredResult = deferred.ToList();
    // deferredResult = [3, 4, 5, 10] — bao gồm cả số 10!

    // IMMEDIATE: Where().ToList() chạy query NGAY LẬP TỨC
    var immediate = numbers.Where(n => n > 2).ToList();
    numbers.Add(100);
    // immediate KHÔNG đổi — vẫn là [3, 4, 5, 10]

    return Results.Ok(new
    {
        explanation = "Deferred (IEnumerable) chạy query mỗi khi enumerate. Immediate (ToList) chạy query ngay.",
        numbers_at_call = numbers,
        deferred_result = deferredResult,
        deferred_note = "deferred được tạo TRƯỚC khi add(10), nhưng enumerate SAU → include 10",
        immediate_result = immediate,
        immediate_note = "immediate.ToList() chạy ngay → KHÔNG include 100 được add sau đó"
    });
});

// Demo: Side effects với deferred execution
app.MapGet("/api/side-effects", () =>
{
    var results = new List<string>();
    var numbers = new[] { 1, 2, 3, 4, 5 };

    // Deferred: lambda CHƯA chạy
    var query = numbers.Where(n =>
    {
        results.Add($"Đánh giá {n}");
        return n > 2;
    });

    // CHƯA có gì trong results!
    var beforeEnumerate = results.ToList();

    // Enumerate → lambda MỚI chạy
    var filtered = query.ToList();
    // results bây giờ có: ["Đánh giá 1", "Đánh giá 2", "Đánh giá 3", "Đánh giá 4", "Đánh giá 5"]

    // Enumerate lại → lambda CHẠY LẠI!
    var filtered2 = query.ToList();
    // results giờ có 10 entries — mỗi enumerate là chạy lại query

    return Results.Ok(new
    {
        before_enumerate_count = beforeEnumerate.Count,
        before_enumerate = beforeEnumerate,
        filtered = filtered,
        total_evaluations_after_first_enumerate = results.Count,
        note = "Deferred query chạy lại MỖI LẦN enumerate → cẩn thận side effects!",
        tip = "Dùng .ToList() hoặc .ToArray() để force immediate execution khi cần"
    });
});

// Demo: So sánh các operators
app.MapGet("/api/operators", () =>
{
    var numbers = new[] { 1, 2, 3, 4, 5 };

    return Results.Ok(new
    {
        // DEFERRED (trả về IEnumerable, không execute ngay)
        deferred_operators = new
        {
            Where = "IEnumerable<T> — deferred",
            Select = "IEnumerable<T> — deferred",
            OrderBy = "IOrderedEnumerable<T> — deferred",
            Take = "IEnumerable<T> — deferred",
            Skip = "IEnumerable<T> — deferred",
            GroupBy = "IEnumerable<IGrouping> — deferred"
        },
        // IMMEDIATE (execute ngay, trả về kết quả cụ thể)
        immediate_operators = new
        {
            ToList = "List<T> — immediate",
            ToArray = "T[] — immediate",
            Count = "int — immediate",
            First = "T — immediate",
            FirstOrDefault = "T? — immediate",
            Single = "T — immediate",
            Sum = "number — immediate",
            Average = "double — immediate",
            Max = "T — immediate",
            Min = "T — immediate",
            Any = "bool — immediate",
            All = "bool — immediate",
            Contains = "bool — immediate",
            ToDictionary = "Dictionary — immediate"
        },
        example_deferred = "var query = numbers.Where(n > 2); // Chưa chạy",
        example_immediate = "var result = numbers.Where(n > 2).ToList(); // Chạy ngay",
        best_practice = "Dùng deferred cho chainable queries, gọi ToList() khi cần kết quả stable"
    });
});

app.Run();
