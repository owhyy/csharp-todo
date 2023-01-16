namespace ToDo;

public struct ToDoTask
{
    public string Description { get; set; }
    public bool Status { get; set; }

    public ToDoTask(string description, bool status = false)
    {
        Description = description;
        Status = status;
    }

    public override string ToString()
    {
        return Description;
    }
}