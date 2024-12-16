namespace Poort8.Dataspace.AuthorizationRegistry.Exceptions;
[Serializable]
public class RepositoryException : Exception
{
    public const string IdNotUnique = "The id is not unique";

    public RepositoryException()
    {
    }

    public RepositoryException(string? message) : base(message)
    {
    }

    public RepositoryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}