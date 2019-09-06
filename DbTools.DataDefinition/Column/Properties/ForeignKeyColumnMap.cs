using System.Linq;

namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyColumnMap
    {
        public SqlColumn ForeignKeyColumn { get; }

        private SqlColumn _referredColumnCached;
        public string ReferredColumnName { get; }

        private readonly ForeignKey _foreignKey;

        public ForeignKeyColumnMap(ForeignKey foreignKey, SqlColumn foreignKeyColumn, string referredColumnName)
        {
            _foreignKey = foreignKey;
            ForeignKeyColumn = foreignKeyColumn;
            ReferredColumnName = referredColumnName;
        }

        public SqlColumn ReferredColumn
        {
            get
            {
                if (_referredColumnCached == null)
                {
                    _referredColumnCached = GetReferredColumn();
                }

                return _referredColumnCached;
            }
        }

        private SqlColumn GetReferredColumn()
        {
            return !string.IsNullOrEmpty(ReferredColumnName)
                                   ? _foreignKey.ReferredTable.Columns[ReferredColumnName]
                                   : _foreignKey.ReferredTable.Properties.OfType<PrimaryKey>().FirstOrDefault()?.SqlColumns.FirstOrDefault()?.SqlColumn;
        }
    }
}
