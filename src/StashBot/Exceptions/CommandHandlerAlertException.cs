using System;

namespace StashBot.Exceptions
{
    public class CommandHandlerAlertException : Exception
    {
        public CommandHandlerAlertException()
        {
        }

        public CommandHandlerAlertException(string message)
            : base(message)
        {
        }

        public CommandHandlerAlertException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}