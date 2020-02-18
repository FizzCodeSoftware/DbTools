namespace FizzCode.DbTools.Common
{
    using System;

    public abstract class DbToolsException : Exception
    {
        protected DbToolsException()
        {
        }

        protected DbToolsException(string message)
            : base(message)
        {
        }

        protected DbToolsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
