// ============================================================
using System.Diagnostics;
using System.Reflection;

// Bài tập 4: Reflection Demo
// Chương 4: C# Fundamentals
// Mục tiêu: Khám phá type information và custom attributes qua reflection
// ============================================================

Console.WriteLine("=== BÀI TẬP 4: REFLECTION DEMO ===\n");

// 1. Đọc Custom Attributes
CustomAttributesDemo.Run();

Console.WriteLine();

// 2. Khám phá Type Information
TypeInfoDemo.Run();

Console.WriteLine();

// 3. Dynamic Invocation
DynamicInvocationDemo.Run();

Console.WriteLine();

// 4. Reflection Performance
ReflectionPerformanceDemo.Run();

Console.WriteLine();

// ----------------------------------------------------------
// Demo 1: Custom Attributes
// ----------------------------------------------------------
public static class CustomAttributesDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 1. Đọc Custom Attributes ---");

        // Đọc attributes từ class Product
        var classAttrs = typeof(Product).GetCustomAttributes<DemoAttribute>();
        foreach (var attr in classAttrs)
        {
            Console.WriteLine($"  [Product] {attr.Description} (Author: {attr.Author})");
        }

        // Đọc attributes từ property
        var nameProp = typeof(Product).GetProperty("Name")!;
        var propAttrs = nameProp.GetCustomAttributes<DemoAttribute>();
        foreach (var attr in propAttrs)
        {
            Console.WriteLine($"  [Product.Name] {attr.Description} (Author: {attr.Author})");
        }

        // Đọc attributes từ method
        var method = typeof(Product).GetMethod("PrintInfo")!;
        var methodAttrs = method.GetCustomAttributes<DemoAttribute>();
        foreach (var attr in methodAttrs)
        {
            Console.WriteLine($"  [Product.PrintInfo] {attr.Description} (Author: {attr.Author})");
        }
    }
}

// ----------------------------------------------------------
// Demo 2: Type Information
// ----------------------------------------------------------
public static class TypeInfoDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 2. Khám phá Type Information ---");

        var type = typeof(Product);
        Console.WriteLine($"  Type: {type.FullName}");
        Console.WriteLine($"  Is Class: {type.IsClass}");
        Console.WriteLine($"  Is ValueType: {type.IsValueType}");
        Console.WriteLine($"  Assembly: {type.Assembly.GetName().Name}");

        Console.WriteLine("\n  Properties:");
        foreach (var prop in type.GetProperties())
        {
            Console.WriteLine($"    {prop.PropertyType.Name} {prop.Name}");
        }

        Console.WriteLine("\n  Methods:");
        foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly))
        {
            var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            Console.WriteLine($"    {method.ReturnType.Name} {method.Name}({parameters})");
        }
    }
}

// ----------------------------------------------------------
// Demo 3: Dynamic Invocation
// ----------------------------------------------------------
public static class DynamicInvocationDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 3. Dynamic Invocation ---");

        // Tạo instance bằng Activator
        object instance = Activator.CreateInstance(typeof(Product))!;
        Console.WriteLine("  Tạo Product instance bằng Activator.CreateInstance()");

        // Set property bằng reflection
        typeof(Product).GetProperty("Name")!.SetValue(instance, "Laptop Gaming");
        typeof(Product).GetProperty("Price")!.SetValue(instance, 2500.00m);
        typeof(Product).GetProperty("Category")!.SetValue(instance, "Điện tử");
        Console.WriteLine("  Set properties qua reflection: Name='Laptop Gaming', Price=2500, Category='Điện tử'");

        // Get property value
        var name = typeof(Product).GetProperty("Name")!.GetValue(instance);
        Console.WriteLine($"  Get Name qua reflection: {name}");

        // Gọi method bằng reflection
        Console.WriteLine("  Gọi PrintInfo() qua reflection:");
        typeof(Product).GetMethod("PrintInfo")!.Invoke(instance, null);
    }
}

// ----------------------------------------------------------
// Demo 4: Reflection vs Direct Access Performance
// ----------------------------------------------------------
public static class ReflectionPerformanceDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 4. Reflection vs Direct Access Performance ---");

        const int iterations = 100_000;
        var product = new Product { Name = "Test", Price = 100m, Category = "Test" };
        var sw = new Stopwatch();

        // Direct access
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            var _ = product.Name;
            product.Name = "Updated";
        }
        long directTime = sw.ElapsedMilliseconds;

        // Reflection access
        var nameProp = typeof(Product).GetProperty("Name")!;
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            var _ = nameProp.GetValue(product);
            nameProp.SetValue(product, "Updated");
        }
        long reflectionTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"  Direct access:   {directTime} ms");
        Console.WriteLine($"  Reflection:      {reflectionTime} ms");
        Console.WriteLine($"  → Reflection chậm hơn đáng kể → dùng Source Generators khi cần performance!");
    }
}

// ----------------------------------------------------------
// Custom Attribute
// ----------------------------------------------------------
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
public class DemoAttribute(string description) : Attribute
{
    public string Description { get; } = description;
    public string? Author { get; set; }
}

// ----------------------------------------------------------
// Demo class
// ----------------------------------------------------------
[Demo("Lớp đại diện cho sản phẩm", Author = "C-3PO")]
public class Product
{
    [Demo("Tên sản phẩm", Author = "R2-D2")]
    public int Id { get; set; }

    [Demo("Tên sản phẩm", Author = "R2-D2")]
    public string Name { get; set; } = "";

    public decimal Price { get; set; }
    public string Category { get; set; } = "";

    [Demo("In thông tin sản phẩm ra console", Author = "BB-8")]
    public void PrintInfo()
    {
        Console.WriteLine($"    Product: {Name} | Price: {Price:C} | Category: {Category}");
    }
}
