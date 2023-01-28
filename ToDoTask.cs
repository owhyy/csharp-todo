namespace ToDo;

public struct ToDoTask
{
    private static int _tCounter = 0;
    public int Id { get; }

    public string Description { get; }
    public bool IsCompleted { get; set; } = false;

    public ToDoTask(string description, bool isCompleted)
    {
        _tCounter++;
        Id = _tCounter;
        Description = description;
        IsCompleted = isCompleted;
    }

    public override string ToString()
    {
        return Description;
    }
}