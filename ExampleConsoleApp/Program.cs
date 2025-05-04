

namespace ExampleConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine(nameof(MethodWithManyParameters));
    }

    public static void MethodWithManyParameters(int a,
        int b,
        string a1,
        int c,
        string a2,
        int d,
        string a3,
        int e)
    {
        Console.WriteLine(a + b + c + d + e);
    }
}