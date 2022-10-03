namespace FizzCode.DbTools.DataDefinition.Checker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Migration;

    // TODO - pattern/skip
    // TODO - move base doc. functions to doc.wr. base

    // TODO - Schema check
    // FK column referencing pattern filtered out table // Check
    // Check Lastmodified (simlar / any) column // Error

    // TODO - Schema check - index check
    // unneccessary index - another index already contains the columns

    // TODO - Schema check - naming conventions
    // table naming
    // FK

    // TODO - Schema check - schema conventions
    // avoid multi PKs, FKs
    // datetime types (sql version dependent db types)

    // TODO - Schema check - metatable reality check

    // TODO - Content check
    // FKs (NOCHECK) for missing references
    //  + accept (but document on summary level) "special" values - NULL on NOTNULL, -1 as value
    // Expected "types" example: CreatedId People

    // ? nullable FKs // Info

    // Conventions
    //  general: lowercase with "_" / camelcase

    //  single pk name
    //  no multi pk
    //  single fk name

    public class SchemaChecker
    {
        public ContextWithLogger Context { get; }

        public SchemaChecker(ContextWithLogger context)
        {
            Context = context;
        }

        public List<SchemaCheck> Check(IDatabaseDefinition dd)
        {
            var schemaChecks = new List<SchemaCheck>();

            foreach (var table in dd.GetTables())
            {
                schemaChecks.AddRange(CheckSelfReferencingFk(table));
                schemaChecks.AddRange(CheckFkAndPkAreTheSame(table));
                schemaChecks.AddRange(CheckFkContainsIdentity(table));
                schemaChecks.AddRange(CheckTableSingularNameConvention(table));
            }

            return schemaChecks;
        }

        private readonly PluralChecker PluralChecker = new();

        private IEnumerable<SchemaCheck> CheckTableSingularNameConvention(SqlTable table)
        {
            var schemaChecks = new List<SchemaCheck>();
            if (!PluralChecker.CheckValidity(table.SchemaAndTableName.TableName))
            {
                schemaChecks.Add(
                    new TableSingularNameConvention(table.SchemaAndTableName)
                    );
            }

            return schemaChecks;
        }

        private static List<SchemaCheck> CheckSelfReferencingFk(SqlTable table)
        {
            var schemaChecks = new List<SchemaCheck>();

            var selfReferencingFks = table.Properties.OfType<ForeignKey>().Where(fk => fk.SqlTable.SchemaAndTableName == fk.ReferredTable.SchemaAndTableName);

            foreach (var selfReferencingFkCandidate in selfReferencingFks)
            {
                var pk = selfReferencingFkCandidate.ReferredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
                var uCs = selfReferencingFkCandidate.ReferredTable.Properties.OfType<UniqueConstraint>();

                var areReferencedAndReferredColumnsTheSame = false;
                var sameUcCandidates = uCs.ToList();
                foreach (var columnMap in selfReferencingFkCandidate.ForeignKeyColumns)
                {
                    if (!areReferencedAndReferredColumnsTheSame
                        && Comparer.ColumnChanged(columnMap.ForeignKeyColumn, columnMap.ReferredColumn))
                    {
                        areReferencedAndReferredColumnsTheSame = true;
                        continue;
                    }

                    foreach (var uc in uCs)
                    {
                        foreach (var ucColumn in uc.SqlColumns)
                        {
                            if (sameUcCandidates.Contains(uc)
                                && !Comparer.ColumnChanged(columnMap.ReferredColumn, ucColumn.SqlColumn))
                            {
                                sameUcCandidates.Remove(uc);
                            }
                        }
                    }
                }

                if (areReferencedAndReferredColumnsTheSame)
                {
                    var selfReferencingFk = new SelfReferencingFk()
                    {
                        ForeignKey = selfReferencingFkCandidate,
                    };

                    // TODO
                    if (sameUcCandidates.Skip(1).Any())
                        throw new ApplicationException("sameUcCandidates should not countain more than 1 elment.");

                    var sameUcCandidate = sameUcCandidates.FirstOrDefault(); // should be only one

                    selfReferencingFk.Comment = sameUcCandidate == null ? "PK" : "UC";

                    schemaChecks.Add(selfReferencingFk);
                }
            }

            return schemaChecks;
        }

        private static List<SchemaCheck> CheckFkAndPkAreTheSame(SqlTable table)
        {
            var schemaChecks = new List<SchemaCheck>();
            var fksOnPkCandidates = table.Properties.OfType<ForeignKey>()
                .Where(fk => fk.SqlTable.Properties.OfType<PrimaryKey>()
                    .Any(pk => pk.SqlColumns
                        .Any(co => fk.ForeignKeyColumns
                            .Any(fkc => fkc.ForeignKeyColumn.Name == co.SqlColumn.Name))));

            foreach (var fksOnPkCandidate in fksOnPkCandidates)
            {
                var pk = fksOnPkCandidate.SqlTable.Properties.OfType<PrimaryKey>().First();
                schemaChecks.Add(new FkAndPkAreTheSame()
                {
                    ForeignKey = fksOnPkCandidate,
                    PrimaryKey = pk
                });
            }

            return schemaChecks;
        }

        private static List<SchemaCheck> CheckFkContainsIdentity(SqlTable table)
        {
            var schemaChecks = new List<SchemaCheck>();

            var identities = table.Columns.SelectMany(c => c.Properties.OfType<Identity>()).ToList();
            var identity = identities.FirstOrDefault();

            if (identity != null)
            {
                var fkIsIdentites = table.Properties.OfType<ForeignKey>()
                    .Where(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn.Name == identity.SqlColumn.Name)).ToList();

                foreach (var fkIsIdentity in fkIsIdentites)
                {
                    schemaChecks.Add(new FkContainsIdentity()
                    {
                        ForeignKey = fkIsIdentity,
                        Identity = identity
                    });
                }
            }

            return schemaChecks;
        }
    }
}
