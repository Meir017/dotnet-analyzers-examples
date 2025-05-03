

namespace ExampleConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine(nameof(MethodWithManyParameters));
    }

    public void MethodWithManyParameters(int a, int b, int c, int d, int e)
    {
        Console.WriteLine(a + b + c + d + e);
    }
}