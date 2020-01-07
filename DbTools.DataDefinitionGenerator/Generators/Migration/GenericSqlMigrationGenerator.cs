namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;

    public interface ISqlMigrationGenerator
    {
        ISqlGenerator Generator { get; }
        string CreateTable(TableNew tableNew);
        string DropTable(TableDelete TableDelete);
    }

    public abstract class GenericSqlMigrationGenerator : ISqlMigrationGenerator
    {
        public virtual ISqlTypeMapper SqlTypeMapper { get; } = new GenericSqlTypeMapper();

        public Context Context { get; }

        protected GenericSqlMigrationGenerator(Context context)
        {
            Context = context;
        }

        public virtual string DropTable(TableDelete tableDelete)
        {
            // Remove indexes, Remove FKs
            // + normal DropTable

            return Generator.DropTable(tableDelete);
        }

        public virtual string RenameTable(TableRename tableRename)
        {
            return "";
        }

        public virtual string CreateTable(TableNew tableNew)
        {
            // TODO Properties (PKs, FKs, Indexes, Defaults, Descriptions)
            return Generator.CreateTable(tableNew);
        }

        protected abstract ISqlGenerator CreateGenerator();

        private ISqlGenerator _generator;

        public ISqlGenerator Generator
        {
            get
            {
                return _generator ?? (_generator = CreateGenerator());
            }
        }
    }

    public class MsSqlMigrationGenerator : GenericSqlMigrationGenerator
    {
        public MsSqlMigrationGenerator(Context context) : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new MsSqlGenerator(Context);
        }
    }

    public class SqLiteMigrationGenerator : GenericSqlMigrationGenerator
    {
        public SqLiteMigrationGenerator(Context context) : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new SqLiteGenerator(Context);
        }
    }

    public class OracleMigrationGenerator : GenericSqlMigrationGenerator
    {
        public OracleMigrationGenerator(Context context) : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new OracleGenerator(Context);
        }
    }
}