namespace Poort8.Dataspace.AuthorizationRegistry.Exceptions;
[Serializable]
public class EnforcerException : Exception
{
    public EnforcerException()
    {
    }

    public EnforcerException(string? message) : base(message)
    {
    }

    public EnforcerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}