using System.Runtime.Serialization;

namespace Poort8.Dataspace.AuthorizationRegistry.Exceptions;
[Serializable]
internal class RepositoryException : Exception
{
    public RepositoryException()
    {
    }

    public RepositoryException(string? message) : base(message)
    {
    }

    public RepositoryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}