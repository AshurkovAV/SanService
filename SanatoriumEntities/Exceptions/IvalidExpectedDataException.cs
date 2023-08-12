using System;

namespace SanatoriumEntities.Exceptions
{
    public class IvalidExpectedDataException: Exception
    {
        public IvalidExpectedDataException()
        {
        }

        public IvalidExpectedDataException(string message): base(message)
        {
        }

        public IvalidExpectedDataException(string message, Exception inner): base(message, inner)
        {
        }
    }
}
