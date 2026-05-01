using System;
using System.Collections.Generic;

public class TaskItem
{
    public string Description { get; set; }
    public bool IsDone { get; set; }

    public TaskItem(string description)
    {
        Description = description;
        IsDone = false; 
    }

    public override string ToString()
    {
        string status = IsDone ? "[X]" : "[ ]";
        return $"{status} {Description}";
    }
}

class Program
{
    static void Main()
    {
        List<TaskItem> tasks = new List<TaskItem>();
        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            Console.WriteLine("=== Менеджер задач ===");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Посмотреть задачи");
            Console.WriteLine("3. Отметить задачу как выполненную");
            Console.WriteLine("4. Выйти");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask(tasks);
                    break;

                case "2":
                    ShowTasks(tasks);
                    break;

                case "3":
                    MarkTaskAsDone(tasks);
                    break;

                case "4":
                    isRunning = false;
                    Console.WriteLine("Программа завершена.");
                    break;

                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                    break;
            }

            if (isRunning)
            {
                Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
                Console.ReadLine();
            }
        }
    }

    static void AddTask(List<TaskItem> tasks)
    {
        Console.Write("Введите описание задачи: ");
        string description = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(description))
        {
            TaskItem newTask = new TaskItem(description);
            tasks.Add(newTask);
            Console.WriteLine("Задача добавлена.");
        }
        else
        {
            Console.WriteLine("Описание не может быть пустым.");
        }
    }

    static void ShowTasks(List<TaskItem> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("Список задач пуст.");
            return;
        }

        Console.WriteLine("\nСписок задач:");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i]}");
        }
    }

    static void MarkTaskAsDone(List<TaskItem> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("Список задач пуст.");
            return;
        }

        ShowTasks(tasks);

        Console.Write("\nВведите номер задачи, которую нужно отметить как выполненную: ");
        if (int.TryParse(Console.ReadLine(), out int number) && number >= 1 && number <= tasks.Count)
        {
            tasks[number - 1].IsDone = true;
            Console.WriteLine("Задача отмечена как выполненная.");
        }
        else
        {
            Console.WriteLine("Некорректный номер задачи.");
        }
    }
}