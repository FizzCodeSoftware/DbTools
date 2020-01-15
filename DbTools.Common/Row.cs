namespace FizzCode.DbTools.Common
{
    using System;
    using System.Collections.Generic;

    public class Row : Dictionary<string, object>
    {
        public T GetAs<T>(string name)
        {
            try
            {
                if (default(T) == null && DBNull.Value.Equals(this[name]))
                    return default;

                return (T)this[name];
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Invalid cast, from: {this[name].GetType().Name} to {typeof(T).Name}, column name: {name}", ex);
            }
        }

        public T GetAs<T>(string name, T defaultValue)
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

        public T CastAs<T>(string name)
        {
            try
            {
                if (default(T) == null && DBNull.Value.Equals(this[name]))
                    return default;

                return (T)this[name];
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Invalid cast, from: {this[name].GetType().Name} to {typeof(T).Name}, column name: {name}", ex);
            }
        }
    }
}
