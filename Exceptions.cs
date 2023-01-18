namespace ToDo;

public class QuitAppException : Exception
{
}

public class NonUniqueDescriptionException : Exception
{
    public NonUniqueDescriptionException(string message)
        : base(message)
    {
    }
}
