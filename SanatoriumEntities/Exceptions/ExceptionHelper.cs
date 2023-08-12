using System;

namespace SanatoriumEntities.Exceptions
{
    public class ExceptionHelper
    {
        public static string buildExceptionMessage(string className, string methodName, string message)
        {
            return String.Format(
                "Class: [{0}], method: [{1}], message: [{2}]",
                className,
                methodName,
                message
            );
        }
    }
}