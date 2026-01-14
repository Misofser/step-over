using System;

namespace GoalApi.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
