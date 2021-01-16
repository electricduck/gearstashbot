using System;

namespace GearstashBot.Exceptions
{
    public class CommandHandlerException : Exception
    {
        public CommandHandlerException()
        {
        }

        public CommandHandlerException(string message)
            : base(message)
        {
        }

        public CommandHandlerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}