// ============================================================
using Microsoft.AspNetCore.Mvc;

// Bài tập 2: Demo 3 Vòng Đời Dependency Injection
// Chương 5: Controller-based API, Middleware, Async/Await và LINQ
// Mục tiêu: Tạo 3 service với 3 lifetime, inject vào controller
//   - Singleton: 1 instance cho toàn app lifetime
//   - Scoped: 1 instance cho mỗi HTTP request
//   - Transient: 1 instance mỗi lần inject
// ============================================================

var builder = WebApplication.CreateBuilder(args);

// Đăng ký 3 service với 3 lifetime khác nhau
builder.Services.AddSingleton<ISingletonDemo, SingletonDemo>();
builder.Services.AddScoped<IScopedDemo, ScopedDemo>();
builder.Services.AddTransient<ITransientDemo, TransientDemo>();

// Đăng ký controller
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

// ----------------------------------------------------------
// Interfaces
// ----------------------------------------------------------
public interface ISingletonDemo
{
    Guid InstanceId { get; }
    string Lifetime { get; }
}

public interface IScopedDemo
{
    Guid InstanceId { get; }
    string Lifetime { get; }
}

public interface ITransientDemo
{
    Guid InstanceId { get; }
    string Lifetime { get; }
}

// ----------------------------------------------------------
// Implementations
// ----------------------------------------------------------
public class SingletonDemo : ISingletonDemo
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public string Lifetime => "Singleton (1 instance/toàn app)";
}

public class ScopedDemo : IScopedDemo
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public string Lifetime => "Scoped (1 instance/request)";
}

public class TransientDemo : ITransientDemo
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public string Lifetime => "Transient (1 instance/mỗi inject)";
}

// ----------------------------------------------------------
// Controller
// ----------------------------------------------------------
[ApiController]
[Route("api/[controller]")]
public class DiDemoController(
    ISingletonDemo singleton1,
    ISingletonDemo singleton2,
    IScopedDemo scoped1,
    IScopedDemo scoped2,
    ITransientDemo transient1,
    ITransientDemo transient2) : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new
        {
            singleton = new
            {
                lifetime = singleton1.Lifetime,
                instance1 = singleton1.InstanceId,
                instance2 = singleton2.InstanceId,
                sameInstance = singleton1.InstanceId == singleton2.InstanceId
            },
            scoped = new
            {
                lifetime = scoped1.Lifetime,
                instance1 = scoped1.InstanceId,
                instance2 = scoped2.InstanceId,
                sameInstance = scoped1.InstanceId == scoped2.InstanceId
            },
            transient = new
            {
                lifetime = transient1.Lifetime,
                instance1 = transient1.InstanceId,
                instance2 = transient2.InstanceId,
                sameInstance = transient1.InstanceId == transient2.InstanceId
            },
            explanation = new[]
            {
                "Singleton: instance1 == instance2 (cùng ID) → 1 instance cho toàn app",
                "Scoped: instance1 == instance2 (cùng ID) → 1 instance cho request này",
                "Transient: instance1 != instance2 (khác ID) → mỗi lần inject là instance mới"
            }
        });
    }
}
