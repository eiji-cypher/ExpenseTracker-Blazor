using System.Reflection;
using Xunit;

namespace ExpenseTracker.Tests;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=================================================");
        Console.WriteLine("  EijiDev Budget Pocket - Automated Test Suite   ");
        Console.WriteLine("=================================================");

        var assembly = Assembly.GetExecutingAssembly();
        var testTypes = assembly.GetTypes()
            .Where(t => t.GetMethods().Any(m => m.GetCustomAttribute<FactAttribute>() != null))
            .ToList();

        int passed = 0;
        int failed = 0;

        foreach (var type in testTypes)
        {
            Console.WriteLine($"\n[Running Test Class: {type.Name}]");
            var instance = Activator.CreateInstance(type);

            foreach (var method in type.GetMethods())
            {
                if (method.GetCustomAttribute<FactAttribute>() == null)
                    continue;

                try
                {
                    var result = method.Invoke(instance, null);
                    if (result is Task task)
                    {
                        await task;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("  [PASS] ");
                    Console.ResetColor();
                    Console.WriteLine(method.Name);
                    passed++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("  [FAIL] ");
                    Console.ResetColor();
                    Console.WriteLine($"{method.Name}: {ex.InnerException?.Message ?? ex.Message}");
                    failed++;
                }
            }
        }

        Console.WriteLine("\n-------------------------------------------------");
        Console.WriteLine($"Test Summary: {passed} Passed, {failed} Failed (Total: {passed + failed})");
        Console.WriteLine("-------------------------------------------------");

        return failed == 0 ? 0 : 1;
    }
}

