namespace ToDo;

public struct ToDoTask
{
    public string Description { get; }
    public bool IsCompleted { get; set; } = false;

    public ToDoTask(string description, bool isCompleted)
    {
        Description = description;
        IsCompleted = isCompleted;
    }

    public override string ToString()
    {
        return Description;
    }
}