namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;

    public class LazySqlTable
    {
        protected string Name { get; set; }
        internal DatabaseDeclaration DatabaseDeclaration { get; private set; }

        public void SetLazyProperties(string name, DatabaseDeclaration databaseDeclaration)
        {
            Name = name;
            DatabaseDeclaration = databaseDeclaration;
        }

        public Lazy<SqlTableDeclaration> LazySqlTableHolder { get; set; }

        public LazySqlTable(Func<SqlTableDeclaration> valueFactory)
        {
            LazySqlTableHolder = new Lazy<SqlTableDeclaration>(valueFactory);
        }

        private SqlTableDeclaration _sqltable;

        public SqlTableDeclaration SqlTable
        {
            get
            {
                if (_sqltable == null)
                {
                    _sqltable = LazySqlTableHolder.Value;
                    _sqltable.SetLazyProperties(Name, DatabaseDeclaration);
                }

                return _sqltable;
            }
        }

        public SqlColumnDeclaration this[string columnName]
        {
            get
            {
                return (SqlColumnDeclaration)SqlTable.Columns[columnName];
            }
        }

        public static implicit operator SqlTable(LazySqlTable lazySqlTable)
        {
            return lazySqlTable.SqlTable;
        }
    }
}
