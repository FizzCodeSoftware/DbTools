using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.Oracle12c;
using FizzCode.DbTools.SqlExecuter;

namespace FizzCode.DbTools.DataDefinitionReader;
public abstract class OracleTableOrViewReader12c : OracleDataDefinitionElementReader
{
    protected ILookup<string, Row> QueryResult = null!;
    protected Oracle12cTypeMapper TypeMapper { get; } = new Oracle12cTypeMapper();

    protected OracleTableOrViewReader12c(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
        : base(executer, schemaNames)
    {
    }

    protected SqlType GetSqlTypeFromRow(Row row)
    {
        // char_col_decl_length

        var type = Throw.IfNull(row.GetAs<string>("DATA_TYPE"));
        var dataPrecisionDecimal = row.GetAs<decimal?>("DATA_PRECISION");
        var dataScaleDecimal = row.GetAs<decimal?>("DATA_SCALE");
        var dataPrecision = (int?)dataPrecisionDecimal;
        var dataScale = (int?)dataScaleDecimal;

        var charLengthDecimal = row.GetAs<decimal?>("CHAR_COL_DECL_LENGTH");
        var charLength = (int?)charLengthDecimal;

        var isNullable = row.GetAs<string>("NULLABLE") != "N";

        var sqlType = TypeMapper.MapSqlTypeFromReaderInfo(type, isNullable, charLength, dataPrecision, dataScale);
        return sqlType;
    }
}