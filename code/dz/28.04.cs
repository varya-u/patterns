using System;

class Program
{
    static double Calculate(double a, double b, Func<double, double, double> operation)
    {
        return operation(a, b);
    }

    static void Main()
    {
        Console.Write("Введите первое число: ");
        if (!double.TryParse(Console.ReadLine(), out double num1))
        {
            Console.WriteLine("Некорректный ввод первого числа.");
            return;
        }

        Console.Write("Введите второе число: ");
        if (!double.TryParse(Console.ReadLine(), out double num2))
        {
            Console.WriteLine("Некорректный ввод второго числа.");
            return;
        }

        Func<double, double, double> add      = (x, y) => x + y;
        Func<double, double, double> subtract = (x, y) => x - y;
        Func<double, double, double> multiply = (x, y) => x * y;
        Func<double, double, double> divide   = (x, y) => y != 0 ? x / y : double.NaN;

        Console.WriteLine("\nРезультаты:");
        Console.WriteLine($"Сложение:     {num1} + {num2} = {Calculate(num1, num2, add)}");
        Console.WriteLine($"Вычитание:    {num1} - {num2} = {Calculate(num1, num2, subtract)}");
        Console.WriteLine($"Умножение:    {num1} * {num2} = {Calculate(num1, num2, multiply)}");

        double divResult = Calculate(num1, num2, divide);
        if (double.IsNaN(divResult))
            Console.WriteLine($"Деление:      {num1} / {num2} = ошибка (деление на ноль)");
        else
            Console.WriteLine($"Деление:      {num1} / {num2} = {divResult}");
    }
}