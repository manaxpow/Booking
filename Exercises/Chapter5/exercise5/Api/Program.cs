// ============================================================
// Bài tập 5: CRUD API với Validation
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: CRUD operations với Data Annotations validation
//   - POST: tạo product (validate Name, Price, Category)
//   - GET: danh sách products, chi tiết product
//   - PUT: cập nhật product
//   - DELETE: xóa product
// ============================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapControllers();
app.Run();

// ----------------------------------------------------------
// DTOs với Data Annotations
// ----------------------------------------------------------
public class CreateProductDto
{
    [Required(ErrorMessage = "Tên không được để trống")]
    [StringLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
    public string Name { get; set; } = "";

    [Range(0.01, 999999999, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category không được để trống")]
    public string Category { get; set; } = "";
}

public class UpdateProductDto
{
    [Required(ErrorMessage = "Tên không được để trống")]
    [StringLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
    public string Name { get; set; } = "";

    [Range(0.01, 999999999, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category không được để trống")]
    public string Category { get; set; } = "";
}

// ----------------------------------------------------------
// Product model
// ----------------------------------------------------------
public record Product(int Id, string Name, decimal Price, string Category);

// ----------------------------------------------------------
// In-memory store
// ----------------------------------------------------------
public static class ProductStore
{
    public static List<Product> Products { get; } =
    [
        new(1, "Laptop Gaming", 2500.00m, "Điện tử"),
        new(2, "Chuột không dây", 35.00m, "Phụ kiện"),
        new(3, "Màn hình 27 inch", 450.00m, "Điện tử")
    ];
    public static int NextId { get; set; } = 4;
}

// ----------------------------------------------------------
// Controller
// ----------------------------------------------------------
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // GET /api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(ProductStore.Products);
    }

    // GET /api/products/5
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = ProductStore.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
            return NotFound(new { message = $"Product với id={id} không tồn tại" });
        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    public ActionResult<Product> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product(ProductStore.NextId++, dto.Name, dto.Price, dto.Category);
        ProductStore.Products.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/5
    [HttpPut("{id}")]
    public ActionResult<Product> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = ProductStore.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
            return NotFound(new { message = $"Product với id={id} không tồn tại" });

        var updated = product with { Name = dto.Name, Price = dto.Price, Category = dto.Category };
        var index = ProductStore.Products.FindIndex(p => p.Id == id);
        ProductStore.Products[index] = updated;
        return Ok(updated);
    }

    // DELETE /api/products/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = ProductStore.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
            return NotFound(new { message = $"Product với id={id} không tồn tại" });

        ProductStore.Products.Remove(product);
        return NoContent();
    }
}
