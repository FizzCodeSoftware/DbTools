namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Row : Dictionary<string, object>
    {
        public T GetAs<T>(string name)
        {
            try
            {
                return (T)this[name];
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Invalid cast, from: {this[name].GetType().Name} to {typeof(T).Name}, column name: {name}", ex);
            }
        }

        public T GetAsByIndex<T>(int i)
        {
            try
            {
                return (T)this.ElementAt(i).Value;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException("error raised during a cast operation", ex);
            }
        }
    }
}
