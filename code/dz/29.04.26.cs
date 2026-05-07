using System;
using System.Collections.Generic;
using System.Linq;

public class Student
{
    public string FullName { get; set; }
    public double AverageScore { get; set; }
    public int Age { get; set; }

    public Student(string fullName, double averageScore, int age)
    {
        FullName = fullName;
        AverageScore = averageScore;
        Age = age;
    }
}
class Program
{
    static void Main()
    {
        var students = new List<Student>
        {
            new Student("Касьмин Михаил", 85.5, 20),
            new Student("Кириллова Виктория", 70.0, 22),
            new Student("Сидорова Анна", 88.0, 19),
            new Student("Скидан Наталья", 92.0, 24),
            new Student("Морозова Елена", 76.0, 21),
            new Student("Бобров Глеб", 65.0, 25),
            new Student("Урсу Мария", 89.0, 23),
            new Student("Гугняк Светлана", 78.0, 18),
        };

        Console.WriteLine("1. Хорошисты (75 <= балл <= 90):");
        var goodStudents = students
            .Where(s => s.AverageScore >= 75 && s.AverageScore <= 90);

        foreach (var s in goodStudents)
        {
            Console.WriteLine($"{s.FullName} - {s.AverageScore:F1}");
        }

        Console.WriteLine();

        Console.WriteLine("2. Только имена:");
        var names = students.Select(s => s.FullName);
        foreach (var name in names)
        {
            Console.WriteLine(name);
        }

        Console.WriteLine();

        Console.WriteLine("3. Студенты по возрасту (от младшего к старшему):");
        var sortedByAge = students.OrderBy(s => s.Age);
        foreach (var s in sortedByAge)
        {
            Console.WriteLine($"{s.FullName}, возраст: {s.Age}, балл: {s.AverageScore:F1}");
        }

        Console.WriteLine();

        Console.WriteLine("4. Рейтинг лучших студентов младше 25 лет:");
        var rating = students
            .Where(s => s.Age < 25)
            .OrderByDescending(s => s.AverageScore)
            .Select(s => $"{s.FullName} - {s.AverageScore:F1}");

        foreach (var line in rating)
        {
            Console.WriteLine(line);
        }
    }
}