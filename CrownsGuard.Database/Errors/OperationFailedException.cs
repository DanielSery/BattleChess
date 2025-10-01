namespace CrownsGuard.Database.Errors;

public class OperationFailedException : Exception
{
    public OperationFailedException(string msg) : base(msg)
    {
    }
}