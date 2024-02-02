namespace FizzCode.DbTools.TestBase;

using System;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Expected exception for testing, with expected start of exception message.
/// </summary>
public class ExpectedExceptionMessageStartsWithAttribute : ExpectedExceptionBaseAttribute
{
    private readonly Type _expectedExceptionType;
    private readonly string[] _expectedExceptionMessages;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectedExceptionMessageStartsWithAttribute"/> class.
    /// </summary>
    /// <param name="expectedExceptionType">The type of the expected exception.</param>
    /// <param name="expectedExceptionMessage">A message, which should be the start of the message of the expected exception.</param>
    public ExpectedExceptionMessageStartsWithAttribute(Type expectedExceptionType, string expectedExceptionMessage)
    {
        _expectedExceptionType = expectedExceptionType;
        _expectedExceptionMessages = new[] { expectedExceptionMessage };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectedExceptionMessageStartsWithAttribute"/> class.
    /// </summary>
    /// <param name="expectedExceptionType">The type of the expected exception.</param>
    /// <param name="expectedExceptionMessages">Possible messages, which should be the start of the message of the of the expected exception.</param>
    public ExpectedExceptionMessageStartsWithAttribute(Type expectedExceptionType, string[] expectedExceptionMessages)
    {
        _expectedExceptionType = expectedExceptionType;
        _expectedExceptionMessages = expectedExceptionMessages;
    }

    /// <summary>
    /// Get the exception to inspect.
    /// </summary>
    /// <param name="exception">The exception that is thrown by the unit test.</param>
    /// <returns>The <see cref="Exception"/> to inspect by the class.</returns>
    protected virtual Exception GetExceptionToInspect(Exception exception)
    {
        return exception;
    }

    /// <inheritdoc />
    protected override void Verify(Exception exception)
    {
        Assert.IsNotNull(exception);

        Assert.IsInstanceOfType(exception, _expectedExceptionType, "Wrong type of exception was thrown.");

        Exception expectionToInspect = GetExceptionToInspect(exception);

        if (CheckIfExceptionMessageStartsWithAny(expectionToInspect))
            return;

        var additionalMessage = GetMessageComparisonMessage(exception);

        throw new AssertFailedException($"ExpectedExceptionMessageStartsWith.Verify failed. Exception message does not start with any of the specified message(s).{additionalMessage}");
    }

    private string GetMessageComparisonMessage(Exception exception)
    {
        const string LongestPrefix = "Expected messsage: ";
        var longestPrefixLength = LongestPrefix.Length + 2;

        var additionalMessage = new StringBuilder($"\r\n{LongestPrefix}");
        var isFirst = true;

        try
        {
            foreach (var expectedExceptionMessage in _expectedExceptionMessages)
            {
                if (isFirst)
                {
                    additionalMessage.Append(expectedExceptionMessage);
                    isFirst = false;
                }
                else
                {
                    additionalMessage.Append("\r\n");
                    additionalMessage.Append(expectedExceptionMessage.PadRight(longestPrefixLength));
                }
            }

            var actualMessageLine = "\r\nActual message: ".PadRight(longestPrefixLength);

            Exception expExceptionToInspect = GetExceptionToInspect(exception);

            additionalMessage.Append($"{actualMessageLine}{expExceptionToInspect.Message}");
        }
        catch (Exception)
        {
            additionalMessage.AppendLine("Error getting exception message");
        }

        return additionalMessage.ToString();
    }

    private bool CheckIfExceptionMessageStartsWithAny(Exception exception)
    {
        var message = exception.Message;

        foreach (var expectedExceptionMessage in _expectedExceptionMessages)
        {
            if (message.StartsWith(expectedExceptionMessage))
                return true;
        }

        return false;
    }
}
