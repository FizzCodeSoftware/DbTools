using System;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinition;
public class InvalidForeignKeyRegistrationException : DbToolsException
{
    public InvalidForeignKeyRegistrationException()
    {
    }

    public InvalidForeignKeyRegistrationException(string message)
        : base(message)
    {
    }

    public InvalidForeignKeyRegistrationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
