using System.Xml;

namespace ToDo;

public class QuitAppException : Exception
{
}

static class Constants
{
    public const string FilePath = "./Todos.xml";
}

public static class ToDoApp
{
    public static void Main()
    {
        Run();
    }

    private static void WriteTodo(ToDoTask task)
    {
        XmlDocument doc = new XmlDocument();
        doc.AppendChild(doc.CreateElement("todo"));
        doc.Save(Constants.FilePath);
    }

    private static List<ToDoTask> ReadPastTodos()
    {
        List<ToDoTask> toDoItems = new();
        XmlDocument xmlDoc = new XmlDocument();
        return toDoItems;
    }

    private static void Run()
    {
        var currentChoice = 0;
        List<ToDoTask> toDoItems = ReadPastTodos();

        while (true)
        {
            Display(toDoItems, currentChoice);
            try
            {
                (toDoItems, currentChoice) = HandleKeyPress(toDoItems, currentChoice);
            }
            catch (QuitAppException)
            {
                break;
            }
        }
    }

    private static void Display(IReadOnlyList<ToDoTask> tasks, int currentChoice)
    {
        Console.Clear();
        Console.WriteLine("Today's todos");

        for (var i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            var taskAsStr = $"{(task.Status ? "[x]" : "[]")} {task.Description}";
            if (i == currentChoice)
                taskAsStr = $"> {taskAsStr}";
            Console.WriteLine(taskAsStr);
        }

        Console.WriteLine("\n\nNavigate using j and k");
        Console.WriteLine("Mark task as completed by pressing Enter");
        Console.WriteLine("Create a new task by pressing c");
        Console.WriteLine("Edit task description by pressing e");
        Console.WriteLine("Delete a task by pressing x");
        Console.WriteLine("Exit the program by pressing q");
    }

    private static (List<ToDoTask>, int) HandleKeyPress(List<ToDoTask> tasks, int previousChoice)
    {
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.J or ConsoleKey.DownArrow:
                return (tasks, previousChoice == tasks.Count - 1 ? 0 : previousChoice + 1);
            case ConsoleKey.K or ConsoleKey.UpArrow:
                return (tasks, previousChoice == 0 ? tasks.Count - 1 : previousChoice - 1);
            case ConsoleKey.C:
            {
                Console.Write("> ");
                var newToDo = Console.ReadLine();
                if (newToDo != null)
                {
                    var newTask = new ToDoTask(newToDo);
                    tasks.Add(newTask);
                    WriteTodo(newTask);
                }

                return (tasks, previousChoice);
            }
            case ConsoleKey.Enter:
            {
                if (tasks.Count == 0) break;
                var currentTask = tasks[previousChoice];
                currentTask.Status = !currentTask.Status;
                tasks[previousChoice] = currentTask;

                return (tasks, previousChoice);
            }
            case ConsoleKey.E:
            {
                if (tasks.Count == 0) break;
                Console.Write("> ");
                var newDescription = Console.ReadLine();

                if (newDescription == null) break;
                var taskModified = tasks[previousChoice];
                taskModified.Description = newDescription;
                tasks[previousChoice] = taskModified;

                return (tasks, previousChoice);
            }
            case ConsoleKey.X:
            {
                if (tasks.Count != 0)
                    tasks.RemoveAt(previousChoice);
                return (tasks, previousChoice);
            }

            case ConsoleKey.Q:
                throw new QuitAppException();
        }

        return (tasks, previousChoice);
    }
}