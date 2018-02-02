using System;

namespace ButtonChallenge.BusinessRules
{
    /// <summary>
    /// Exception thrown if there is not enought points.
    /// </summary>
    public class InsufficientPointsException : Exception
    {
        public InsufficientPointsException(string message) : base(message)
        {
        }
    }
}
