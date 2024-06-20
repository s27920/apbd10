namespace apbd10.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string? message) : base(message)
    {
    }
}