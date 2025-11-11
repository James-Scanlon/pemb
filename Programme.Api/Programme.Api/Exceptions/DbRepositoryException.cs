using System;

namespace Programme.Api.Exceptions;

public class DbRepositoryException : Exception
{
    public DbRepositoryException(string message) : base(message)
    {
    }

    public DbRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}