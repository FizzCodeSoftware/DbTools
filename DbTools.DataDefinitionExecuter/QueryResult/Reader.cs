namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Reader
    {
        public Reader()
        {
            Rows = new List<Row>();
        }

        // public List<string> Headers { get; set; }

        public List<Row> Rows { get; }

        public IEnumerable<T> GetRows<T>()
        {
            foreach (var row in Rows)
            {
                yield return row.GetAsByIndex<T>(0);
            }
        }

        public IEnumerable<(T1, T2)> GetRows<T1, T2>()
        {
            foreach (var row in Rows)
            {
                var v1 = row.GetAsByIndex<T1>(0);
                var v2 = row.GetAsByIndex<T2>(0);

                yield return (v1, v2);
            }
        }
    }
}
