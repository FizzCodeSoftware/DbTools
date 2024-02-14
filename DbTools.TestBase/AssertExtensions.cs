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

#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    public static void AreNotNulls(this Assert assert, [NotNull] object? value1, [NotNull] object? value2) => AreNotNulls(value1, value2);

    public static void AreNotNulls(this Assert assert, [NotNull] object? value1, [NotNull] object? value2, [NotNull] object? value3) => AreNotNulls(value1, value2, value3);

    public static void AreNotNulls(this Assert assert, [NotNull] object? value1, [NotNull] object? value2, [NotNull] object? value3, [NotNull] object? value4) => AreNotNulls(value1, value2, value3, value4);

    public static void AreNotNulls(this Assert assert, [NotNull] object? value1, [NotNull] object? value2, [NotNull] object? value3, [NotNull] object? value4, [NotNull] object? value5) => AreNotNulls(value1, value2, value3, value4, value5);
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    private static void AreNotNulls([NotNull] params object?[] values)
    {
        foreach ( var value in values)
            Assert.IsNotNull(value);
    }
}
