namespace FizzCode.DbTools.Common
{
    using System;
    public abstract class DbToolsException : Exception
    {
        public DbToolsException() : base()
        {
        }

        public DbToolsException(string message) : base(message)
        {
        }

        public DbToolsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
