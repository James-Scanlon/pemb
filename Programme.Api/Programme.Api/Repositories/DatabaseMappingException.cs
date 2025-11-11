using System;
namespace Programme.Api.Repositories;

public class DatabaseMappingException : Exception
{
    public DatabaseMappingException()
    {
    }

    public DatabaseMappingException(string message) : base(message)
    {
    }

    public DatabaseMappingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}