using System;

namespace SanatoriumEntities.Exceptions
{
    public class InvalidInputDataException: Exception
    {
        public InvalidInputDataException()
        {
        }

        public InvalidInputDataException(string message): base(message)
        {
        }

        public InvalidInputDataException(string message, Exception inner): base(message, inner)
        {
        }
    }
}
