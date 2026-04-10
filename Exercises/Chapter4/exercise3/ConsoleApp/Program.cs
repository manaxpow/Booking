// ============================================================
// Bài tập 3: State Machine — Manual vs async/await
// Chương 4: C# Fundamentals
// Mục tiêu: Hiểu cách compiler biến async/await thành state machine
// ============================================================

Console.WriteLine("=== BÀI TẬP 3: STATE MACHINE — MANUAL VS ASYNC/AWAIT ===\n");

// 1. Manual State Machine
ManualStateMachineDemo.Run();

Console.WriteLine();

// 2. async/await State Machine (compiler-generated)
AsyncAwaitDemo.Run();

Console.WriteLine();

// 3. So sánh output
Console.WriteLine("--- 3. So sánh ---");
Console.WriteLine("  Manual State Machine: developer tự quản lý state, goto case");
Console.WriteLine("  async/await: compiler tự generate state machine tương đương");
Console.WriteLine("  → async/await giúp viết code bất đồng bộ sạch hơn nhiều!");

Console.WriteLine();

// ----------------------------------------------------------
// Demo 1: Manual State Machine
// ----------------------------------------------------------
public static class ManualStateMachineDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 1. Manual State Machine ---");

        var machine = new ManualStateMachine();
        Console.WriteLine("  Chạy MoveNext() đến khi hoàn thành:");

        int maxSteps = 10;
        int step = 0;
        while (!machine.IsCompleted && step < maxSteps)
        {
            machine.MoveNext();
            step++;
        }

        if (machine.IsCompleted)
            Console.WriteLine($"  Result: {machine.Result}");
    }
}

/// <summary>
/// Mô phỏng cách compiler biến async method thành state machine.
/// Trong thực tế, compiler generates một struct implement IAsyncStateMachine.
/// </summary>
public class ManualStateMachine
{
    private int _state = -1;
    private string _step1Result = "";
    private string _step2Result = "";

    public string? Result { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsStepDone => true; // Giả lập await hoàn thành ngay

    public void MoveNext()
    {
        switch (_state)
        {
            case -1: // Initial state
                Console.WriteLine("    State -1 → Bắt đầu, chuyển sang state 0");
                _state = 0;
                goto case 0;

            case 0: // Await step 1
                Console.WriteLine("    State 0 → Bắt đầu Step 1 (giả lập async)");
                if (!IsStepDone) return; // Nếu chưa hoàn thành, return (sẽ resume sau)
                _step1Result = SimulateAsyncWork("Kết quả Step 1");
                Console.WriteLine($"    State 0 → Step 1 xong: '{_step1Result}', chuyển sang state 1");
                _state = 1;
                goto case 1;

            case 1: // Await step 2
                Console.WriteLine("    State 1 → Bắt đầu Step 2 (giả lập async)");
                if (!IsStepDone) return;
                _step2Result = SimulateAsyncWork("Kết quả Step 2");
                Console.WriteLine($"    State 1 → Step 2 xong: '{_step2Result}', chuyển sang state 2");
                _state = 2;
                goto case 2;

            case 2: // Final
                Result = _step1Result + " | " + _step2Result;
                IsCompleted = true;
                Console.WriteLine($"    State 2 → Hoàn thành! Result = '{Result}'");
                break;
        }
    }

    private static string SimulateAsyncWork(string result)
    {
        Thread.Sleep(10); // Giả lập async delay
        return result;
    }
}

// ----------------------------------------------------------
// Demo 2: async/await (compiler-generated state machine)
// ----------------------------------------------------------
public static class AsyncAwaitDemo
{
    public static void Run()
    {
        Console.WriteLine("--- 2. async/await (Compiler-generated State Machine) ---");

        // Chạy async method
        var result = RunAsync().GetAwaiter().GetResult();
        Console.WriteLine($"  Result: {result}");
    }

    private static async Task<string> RunAsync()
    {
        Console.WriteLine("    → Bắt đầu async method (compiler tạo state machine)");

        // Mỗi 'await' là một state transition trong state machine
        string step1 = await SimulateAsyncWorkAsync("Kết quả Step 1");
        Console.WriteLine($"    → Step 1 xong: '{step1}'");

        string step2 = await SimulateAsyncWorkAsync("Kết quả Step 2");
        Console.WriteLine($"    → Step 2 xong: '{step2}'");

        return step1 + " | " + step2;
    }

    private static async Task<string> SimulateAsyncWorkAsync(string result)
    {
        await Task.Delay(10);
        return result;
    }
}
