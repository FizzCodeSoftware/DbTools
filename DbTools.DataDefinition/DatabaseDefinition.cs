namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class DatabaseDefinition
    {
        private Tables _tables = new Tables();

        internal Tables Tables
        {
            get
            {
                return _tables;
            }
            set
            {
                _isDirty = true;
                _tables = value;
                Initialize();
            }
        }

        private bool _isDirty;

        public void AddTable(SqlTable sqlTable)
        {
            _isDirty = true;
            Tables.Add(sqlTable);
        }

        public virtual List<SqlTable> GetTables()
        {
            Initialize();
            return Tables.ToList();
        }

        private void Initialize()
        {
            if (_isDirty)
                CircularFKDetector.DectectCircularFKs(Tables.ToList());
        }

        public SqlTable GetTable(string name)
        {
            return Tables[name];
        }
    }
}
