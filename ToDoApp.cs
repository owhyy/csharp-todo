using System.Xml;

namespace ToDo;

internal static class Constants
{
    public const string FilePath = "todos.xml";
}

public static class ToDoApp
{
    public static void Main()
    {
        Run();
    }

    private static void WriteToFile(ToDoTask task)
    {
        XmlWriterSettings settings = new()
        {
            NewLineChars = "\n",
            NewLineOnAttributes = true,
            Indent = true,
            IndentChars = "\t"
        };
        using var writer = XmlWriter.Create(Constants.FilePath, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("tasks");

        writer.WriteStartElement("task");
        writer.WriteAttributeString("status", task.IsCompleted.ToString());
        writer.WriteString(task.Description);
        writer.WriteEndElement();
    }

    private static void WriteAllTasks(List<ToDoTask> tasks)
    {
        foreach (var task in tasks)
        {
            WriteToFile(task);
        }
    }

    private static List<ToDoTask> ReadPastTodos()
    {
        XmlReaderSettings settings = new();
        List<ToDoTask> toDoItems = new();
        using var reader = XmlReader.Create(Constants.FilePath, settings);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader is { Name: "task", HasAttributes: true })
            {
                var description = reader.ReadString();
                var status = reader.GetAttribute("status");
                var isCompleted = false;
                if (status != null)
                    bool.TryParse(status, out isCompleted);
                toDoItems.Add(new ToDoTask(description, isCompleted));
            }
        }

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
            var taskAsStr = $"{(task.IsCompleted ? "[x]" : "[]")} {task.Description}";
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

    private static ToDoTask TryToCreate(string? description, bool isCompleted, List<ToDoTask> otherTasks)
    {
        if (description is null)
            throw new ArrayTypeMismatchException("Enter a valid description");

        bool isUniqueTask = otherTasks.All(t => t.Description != description);
        if (isUniqueTask)
            return new ToDoTask(description, isCompleted);

        throw new NonUniqueDescriptionException(
            "A entry with that description already exists. Please pick a different name");
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
                var taskDescription = Console.ReadLine();
                try
                {
                    ToDoTask task = TryToCreate(taskDescription, false, tasks);
                    tasks.Add(task);
                    WriteToFile(task);
                }
                catch (Exception e) when (e is NonUniqueDescriptionException or ArgumentException)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }

                return (tasks, previousChoice);
            }
            case ConsoleKey.Enter:
            {
                if (tasks.Count == 0) break;
                var currentTask = tasks[previousChoice];
                currentTask.IsCompleted = !currentTask.IsCompleted;
                tasks[previousChoice] = currentTask;

                return (tasks, previousChoice);
            }
            case ConsoleKey.E:
            {
                if (tasks.Count == 0) break;
                var previousTask = tasks[previousChoice];

                Console.Write("> ");
                var newDescription = Console.ReadLine();

                try
                {
                    ToDoTask modifiedTask = TryToCreate(newDescription, previousTask.IsCompleted, tasks);
                    tasks[previousChoice] = modifiedTask;
                }
                catch (NonUniqueDescriptionException e)
                {
                    // don't display message if descriptions are the same
                    if (newDescription != previousTask.Description)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }

                // TODO: this is obviously ineffective, we should ideally 
                // somehow only update the specified one, but it works for now
                WriteAllTasks(tasks);

                return (tasks, previousChoice);
            }
            case ConsoleKey.X:
            {
                if (tasks.Count != 0)
                    tasks.RemoveAt(previousChoice);
                return (tasks, --previousChoice);
            }

            case ConsoleKey.Q:
                throw new QuitAppException();
        }

        return (tasks, previousChoice);
    }
}