using System.Data;
using System.Runtime.CompilerServices;

namespace TaskManager;

public class Task
{
    public readonly int ID;
    public string Title { get; }
    public string Description { get; }
    public string Date { get; }
    public string IsCompleted { get; }

    public Task(int id, string title, string desc, string date, string completed)
    {
        ID = id;
        Title = title;
        Description = desc;
        Date = date;
        IsCompleted = completed;
    }
}

public static class TaskManager
{
    private static List<Task> taskList = new List<Task>();

    public static void AddTask(string title, string desc, string date, string completed)
    {
        if (!taskList.Any())
        {
            taskList.Add(new Task(id: taskList.Count(), title: title, desc: desc, date: date, completed: completed));
        }
        else
        {
            taskList.Add(new Task(id: taskList[^1].ID + 1, title: title, desc: desc, date: date, completed: completed));
        }
    }

    public static void UpdateTask(int taskNumber, string title, string desc, string date, string completed)
    {
        var (found, index) = BinarySearch(0, taskList.Count - 1, taskNumber);
        if (found)
        {
            taskList[index] = new Task(id: taskNumber, title: title, desc: desc, date: date, completed: completed);
        }else
        {
            throw new ArgumentOutOfRangeException("Please enter a valid task ID available in the task list.");
        }
    }

    public static IReadOnlyList<Task> ListTasks()
    {
        IEnumerable<Task> incompleteTasks =
            from task in taskList
            where task.IsCompleted.ToLower() == "n"
            select task;
        return incompleteTasks.ToList().AsReadOnly();
    }

    public static void DeleteTask(int taskNumber)
    {
        var (found, index) = BinarySearch(0, taskList.Count() - 1, taskNumber);
        if (found)
        {
            taskList.RemoveAt(index);
        }else
        {
            throw new ArgumentOutOfRangeException("Please enter a valid task ID available in the task list.");
        }
    }

    public static (bool, int) BinarySearch(int left, int right, int target)
    {
        if (left > right)
        {
            return (false, -1);
        }

        int middle = (left + right) / 2;
        if (taskList[middle].ID < target)
        {
            return BinarySearch(middle + 1, right, target);
        }
        else if (taskList[middle].ID > target)
        {
            return BinarySearch(left, middle - 1, target);
        }
        else
        {
            return (true, middle);
        }
    }
}

public static class InputValidation
{
    public static bool AreInputStringsNotValid(params string[] consoleInputs)
    {
        if (consoleInputs.Count() == 0)
        {
            return false;
        }
        return string.IsNullOrWhiteSpace(consoleInputs[0]) || AreInputStringsNotValid(consoleInputs[1..]);
    }

    public static bool IsYesOrNo(string value)
    {
        return value.ToLower() == "y" || value.ToLower() == "n";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Task Manager!");
        bool stillRunning = true;
        string[] optionList = {
            "Add a task",
            "List all tasks",
            "Update a task",
            "Delete a task",
            "Exit" };
        while (stillRunning)
        {
            Console.WriteLine();
            for (int i = 0; i < optionList.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {optionList[i]}");
            }

            Console.WriteLine();
            Console.Write("Enter your choice: ");
            string? choice = Console.ReadLine();
            int.TryParse(choice, out int option);
            Console.WriteLine();
            switch (option)
            {
                case 1:
                    AddTask();
                    break;
                case 2:
                    ListTasks();
                    break;
                case 3:
                    UpdateTask();
                    break;
                case 4:
                    DeleteTask();
                    break;
                case 5:
                    stillRunning = false;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine($"Please enter a valid number from 1 to {optionList.Count()}");
                    break;
            }
        }
    }

    public static void AddTask()
    {
        Console.Write("Enter title: ");
        string title = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter description: ");
        string description = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter date (MM-DD-YYYY): ");
        string date = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter completion status (y/N): ");
        string completed = Console.ReadLine() ?? string.Empty;

        if (InputValidation.AreInputStringsNotValid(title, description, date, completed))
        {
            Console.WriteLine();
            Console.WriteLine("All fields are required.");
            return;
        }
        TaskManager.AddTask(title: title, desc: description, date: date, completed: completed);
        Console.WriteLine("Task added successfully.");
    }

    public static void ListTasks()
    {
        IReadOnlyList<Task> tasks = TaskManager.ListTasks();
        DisplayTasks(tasks, "No Tasks found.");
    }

    public static void UpdateTask()
    {
        if (!TaskManager.ListTasks().Any())
        {
            Console.WriteLine("No tasks to edit");
            return;
        }
        ListTasks();
        Console.WriteLine();
        Console.Write("Enter task ID to edit: ");
        Console.WriteLine();
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Console.Write("Enter new title: ");
            string title = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter new description: ");
            string description = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter new date (MM-DD-YYYY): ");
            string date = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter new completion status (y/N): ");
            string completed = Console.ReadLine() ?? string.Empty;

            if (InputValidation.AreInputStringsNotValid(title, description, date, completed))
            {
                Console.WriteLine("All fields are required.");
                return;
            }
            try
            {
                TaskManager.UpdateTask(taskNumber: id, title: title, desc: description, date: date, completed: completed);
                Console.WriteLine("Task Updated successfully.");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        else
        {
            Console.WriteLine("Invalid task number.");
            return;
        }
    }

    public static void DeleteTask()
    {
        if (!TaskManager.ListTasks().Any())
        {
            Console.WriteLine("No tasks to delete");
            return;
        }
        ListTasks();
        Console.WriteLine();
        Console.Write("Enter a task ID to delete:  ");
        Console.WriteLine();
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Console.Write("Are you sure? (y/N): ");
            string confirmation = Console.ReadLine() ?? string.Empty;
            if (InputValidation.AreInputStringsNotValid(confirmation) || !InputValidation.IsYesOrNo(confirmation))
            {
                Console.WriteLine("Please enter a valid option");
                return;
            }
            else if (confirmation.ToLower() == "y")
            {
                try
                {
                    TaskManager.DeleteTask(id);
                    Console.WriteLine("Task Deleted Successfully.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid task number.");
            return;
        }
    }

    public static void DisplayTasks(IReadOnlyList<Task> tasks, string message)
    {
        if (tasks.Any())
        {
            for (int i = 0; i < tasks.Count(); i++)
            {
                Console.WriteLine($"{i + 1}. Task ID: {tasks[i].ID}, Title: {tasks[i].Title}, Description: {tasks[i].Description}, Date: {tasks[i].Date}, Completed: {tasks[i].IsCompleted}");
            }
        }
        else
        {
            Console.WriteLine(message);
        }
    }
}
