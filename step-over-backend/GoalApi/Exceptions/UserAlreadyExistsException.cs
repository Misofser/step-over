using System;

namespace GoalApi.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() : base("User with this username already exists") { }
    }
}
