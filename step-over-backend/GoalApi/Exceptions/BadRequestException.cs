using System;

namespace GoalApi.Exceptions;

public class BadRequestException : Exception 
{
    public BadRequestException(string message) : base(message) { } 
}
