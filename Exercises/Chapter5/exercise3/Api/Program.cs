// ============================================================
// Bài tập 3: LINQ Xử Lý Dữ Liệu Collection
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Thực hành Where, GroupBy, OrderBy, Take, Select, Sum
// ============================================================

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/products", () =>
{
    var products = new[]
    {
        new { Name = "Laptop", Price = 1500, Category = "Dien tu" },
        new { Name = "Chuot", Price = 25, Category = "Phu kien" },
        new { Name = "Man hinh", Price = 300, Category = "Dien tu" },
        new { Name = "Ban phim", Price = 80, Category = "Phu kien" },
        new { Name = "CPU", Price = 500, Category = "Dien tu" },
        new { Name = "USB", Price = 15, Category = "Phu kien" },
        new { Name = "RAM", Price = 120, Category = "Dien tu" }
    };

    return Results.Ok(new
    {
        // 1. Filter: sản phẩm giá > 100
        expensiveProducts = products
            .Where(p => p.Price > 100)
            .Select(p => new { p.Name, p.Price })
            .ToArray(),

        // 2. GroupBy: tổng giá theo category
        totalByCategory = products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(p => p.Price), Count = g.Count() })
            .ToArray(),

        // 3. OrderByDescending + Take: top 3 đắt nhất
        top3Expensive = products
            .OrderByDescending(p => p.Price)
            .Take(3)
            .Select(p => new { p.Name, p.Price })
            .ToArray(),

        // 4. Select: transform data
        productSummary = products
            .Select(p => $"{p.Name} ({p.Category}): {p.Price:C}")
            .ToArray(),

        // 5. Any & All
        hasExpensiveItem = products.Any(p => p.Price > 1000),
        allPositivePrice = products.All(p => p.Price > 0),

        // 6. Count & Average
        stats = new
        {
            totalProducts = products.Count(),
            averagePrice = products.Average(p => p.Price),
            maxPrice = products.Max(p => p.Price),
            minPrice = products.Min(p => p.Price)
        }
    });
});

app.MapGet("/api/products/advanced", () =>
{
    var products = new[]
    {
        new { Name = "Laptop", Price = 1500, Category = "Dien tu", Stock = 10 },
        new { Name = "Chuot", Price = 25, Category = "Phu kien", Stock = 50 },
        new { Name = "Man hinh", Price = 300, Category = "Dien tu", Stock = 0 },
        new { Name = "Ban phim", Price = 80, Category = "Phu kien", Stock = 30 },
        new { Name = "CPU", Price = 500, Category = "Dien tu", Stock = 5 },
        new { Name = "USB", Price = 15, Category = "Phu kien", Stock = 0 },
        new { Name = "RAM", Price = 120, Category = "Dien tu", Stock = 20 }
    };

    return Results.Ok(new
    {
        // Chaining LINQ: filter → group → order → select
        categoryStats = products
            .Where(p => p.Stock > 0)
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalValue = g.Sum(p => p.Price * p.Stock),
                AvailableProducts = g.Count(),
                MostExpensive = g.OrderByDescending(p => p.Price).First().Name
            })
            .OrderByDescending(c => c.TotalValue)
            .ToArray(),

        // FirstOrDefault with default
        cheapest = products.OrderBy(p => p.Price).FirstOrDefault(),
        outOfStock = products.FirstOrDefault(p => p.Stock == 0),

        // Skip & Take cho pagination
        page1 = products.OrderBy(p => p.Name).Skip(0).Take(3).Select(p => p.Name).ToArray(),
        page2 = products.OrderBy(p => p.Name).Skip(3).Take(3).Select(p => p.Name).ToArray()
    });
});

app.Run();
