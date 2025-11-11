using System;

public class ProgramLimitException : Exception
{
    public ProgramLimitException()
    {
    }

    public ProgramLimitException(string message) : base(message)
    {
    }

    public ProgramLimitException(string message, Exception innerException) : base(message, innerException)
    {
    }
}