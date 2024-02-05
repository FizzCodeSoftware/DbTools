using System.Reflection;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.Common.Tests;

[TestClass]
public class PropertyInfoExtensionsTests
{
    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(InvalidOperationException),
        "The property TestProperty cannot be null on TestClassWithProperty.")]
    public void GetValueSafeTest()
    {
        var testClassWithProperty = new TestClassWithProperty();
        testClassWithProperty.GetValueSafeTest();

        testClassWithProperty.Property = null;

        testClassWithProperty.GetValueSafeTest();
    }

    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(InvalidOperationException),
        "The property TestPropertyGenericBase<String> cannot be null on TestClassWithProperty.")]
    public void GetValueSafeTestForGenericProperty()
    {
        var testClassWithProperty = new TestClassWithProperty();

        testClassWithProperty.GetValueSafeTestForGenericProperty();

        testClassWithProperty.TestPropertyGenericStringA = null;

        testClassWithProperty.GetValueSafeTestForGenericProperty();
    }
}

public class TestProperty
{
}

public class TestPropertyGenericBase<T>
{
}

public class TestPropertyGenericStringA : TestPropertyGenericBase<string>
{
}

public class TestPropertyGenericStringB : TestPropertyGenericBase<string>
{
}

public class TestPropertyGenericStringInt : TestPropertyGenericBase<int>
{
}



public class BaseClass
{
    public void GetValueSafeTest()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => typeof(TestProperty).IsAssignableFrom(pi.PropertyType));

        Assert.AreEqual(1, properties.Count());

        var property = properties.First();
        var result = property.GetValueSafe<TestProperty>(this);
    }

    public void GetValueSafeTestForGenericProperty()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                (typeof(TestPropertyGenericStringA).IsAssignableFrom(pi.PropertyType)
                || typeof(TestPropertyGenericStringB).IsAssignableFrom(pi.PropertyType))
                && pi.GetIndexParameters().Length == 0);

        Assert.AreEqual(2, properties.Count());

        var property1 = properties.First();
        var result1 = property1.GetValueSafe<TestPropertyGenericBase<string>>(this);

        var property2 = properties.Skip(1).First();
        var result2 = property2.GetValueSafe<TestPropertyGenericBase<string>>(this);
    }
}

public class TestClassWithProperty : BaseClass
{
    public TestProperty? Property { get; set; } = new TestProperty();
    public TestPropertyGenericStringA? TestPropertyGenericStringA { get; set; } = new TestPropertyGenericStringA();
    public TestPropertyGenericStringB? TestPropertyGenericStringB { get; set; } = new TestPropertyGenericStringB();
    public TestPropertyGenericStringInt? TestPropertyGenericStringInt { get; set; } = new TestPropertyGenericStringInt();
}