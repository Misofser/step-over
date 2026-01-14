using System;

namespace GoalApi.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException() : base("Invalid username or password") { }
}
