using System;

namespace GoalApi.Exceptions
{
    public class GoalNotFoundException : Exception
    {
        public GoalNotFoundException() : base("Goal not found") { }
    }
}
