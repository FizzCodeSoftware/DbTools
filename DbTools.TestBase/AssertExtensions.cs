using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FizzCode.DbTools.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.TestBase;

public static class AssertExtensions
{
    public static T CheckAndReturnInstanceOfType<T>(this Assert assert, [NotNull] object? value)
    {
        if (value is null)
            throw new AssertFailedException($"AssertExtensions.CheckAndReturnInstanceOfType failed, value is null (type is {typeof(T).GetFriendlyTypeName()}).");

        var elementTypeInfo = value.GetType().GetTypeInfo();
        var expectedTypeInfo = typeof(T).GetTypeInfo();
        if (!expectedTypeInfo.IsAssignableFrom(elementTypeInfo))
            throw new AssertFailedException($"AssertExtensions.CheckAndReturnInstanceOfType failed, {expectedTypeInfo.GetFriendlyTypeName()} is not assignable from {elementTypeInfo.GetFriendlyTypeName()}.");

        return (T)value;
    }
}
