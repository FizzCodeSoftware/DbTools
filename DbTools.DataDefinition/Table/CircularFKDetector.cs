namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public static class CircularFKDetector
    {
        public static void DectectCircularFKs(IEnumerable<SqlTable> tables)
        {
            foreach (var table in tables)
            {
                var cFKs = new List<CircularFK>();
                foreach (var fk in table.Properties.OfType<ForeignKey>())
                {
                    var cfk = new CircularFK(table);

                    var visitedFks = new List<ForeignKey>();
                    var visitedTables = new List<SqlTable>();

                    if (GetCircularFKs(visitedFks, visitedTables, fk))
                    {
                        cfk.ForeignKeyChain = visitedFks;
                        cFKs.Add(cfk);
                    }
                    else
                    {
                        cFKs = new List<CircularFK>();
                    }
                }

                table.Properties.AddRange(cFKs);
            }
        }

        private static bool GetCircularFKs(List<ForeignKey> visitedFks, List<SqlTable> visitedTables, ForeignKey fk)
        {
            visitedFks.Add(fk);
            visitedTables.Add(fk.SqlTable);

            if (visitedTables.Contains(fk.ReferredTable))
            {
                return true;
            }

            var nextFKs = fk.ReferredTable.Properties.OfType<ForeignKey>().ToList();

            var any = false;
            foreach (var nextFk in nextFKs)
            {
                /*if (GetCircularFKs(visitedFks, visitedTables, nextFk))
                    return true;*/
                if (GetCircularFKs(visitedFks, visitedTables, nextFk))
                    any = true;
            }

            // End of chain FK without circular reference
            return any;
        }
    }
}
