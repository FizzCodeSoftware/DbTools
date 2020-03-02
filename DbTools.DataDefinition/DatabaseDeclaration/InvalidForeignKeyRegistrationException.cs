namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;

    public class InvalidForeignKeyRegistrationException : DbToolsException
    {
        public InvalidForeignKeyRegistrationException()
        {
        }

        public InvalidForeignKeyRegistrationException(string message)
            : base(message)
        {
        }

        public InvalidForeignKeyRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
