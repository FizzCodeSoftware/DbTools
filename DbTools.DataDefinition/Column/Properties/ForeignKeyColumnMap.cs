using System.Linq;

namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyColumnMap
    {
        public SqlColumn ForeignKeyColumn { get; }

        private SqlColumn _referredColumnCached;

        // TODO it's possible that this should be SqlColumn
        private readonly string _referredColumnName;

        private readonly ForeignKey _foreignKey;

        public ForeignKeyColumnMap(ForeignKey foreignKey, SqlColumn foreignKeyColumn, string referredColumnName)
        {
            _foreignKey = foreignKey;
            ForeignKeyColumn = foreignKeyColumn;
            _referredColumnName = referredColumnName;
        }

        public SqlColumn ReferredColumn => _referredColumnCached ?? (_referredColumnCached = GetReferredColumn());

        private SqlColumn GetReferredColumn()
        {
            return !string.IsNullOrEmpty(_referredColumnName)
                                   ? _foreignKey.ReferredTable.Columns[_referredColumnName]
                                   : _foreignKey.ReferredTable.Properties.OfType<PrimaryKey>().FirstOrDefault()?.SqlColumns.FirstOrDefault()?.SqlColumn;
        }
    }
}
