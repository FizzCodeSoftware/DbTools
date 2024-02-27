using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FizzCode.DbTools.Common;
[DebuggerDisplay("{ToString(),nq}")]
public class Row : Dictionary<string, object>
{
    public T? GetAs<T>(string name)
    {
        try
        {
            if (default(T) is null && DBNull.Value.Equals(this[name]))
                return default;

            return (T)this[name];
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Invalid cast, from: {this[name].GetType().Name} to {typeof(T).Name}, column name: {name}", ex);
        }
    }

    public T? GetAs<T>(string name, T? defaultValue)
    {
        if (!ContainsKey(name))
            return defaultValue;

        try
        {
            return (T)this[name];
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Invalid cast, from: {this[name].GetType().Name} to {typeof(T).Name}, column name: {name}", ex);
        }
    }

    public T? CastAs<T>(string name)
    {
        try
        {
            if (default(T) is null && DBNull.Value.Equals(this[name]))
                return default;

            return (T)this[name];
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Invalid cast, from: {this[name].GetType().Name} to {typeof(T).Name}, column name: {name}", ex);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in this)
        {
            sb.Append(kvp.Key);
            sb.Append('(');
            sb.Append(kvp.Value.GetType().Name);
            sb.Append("): ");
            sb.Append(kvp.Value);
            sb.Append(", ");
        }
        var result = sb.ToString();

        if (result != "")
            result = result.Substring(0, result.Length - 2);

        return result;
    }
}
