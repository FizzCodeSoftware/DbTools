using Microsoft.Extensions.Configuration;

namespace FizzCode.DbTools.Common;
public static class Helper
{
    public static Settings GetDefaultSettings(SqlEngineVersion version, IConfigurationRoot configuration)
    {
        var settings = new Settings();

        var sqlVersionSpecificSettings = new SqlVersionSpecificSettings();

        if (version is SqLiteVersion)
        {
            sqlVersionSpecificSettings["ShouldCreateAutoincrementAsPrimaryKey"] = "false";
        }

        if (version is MsSqlVersion)
        {
            sqlVersionSpecificSettings["DefaultSchema"] = "dbo";
        }

        if (version is OracleVersion)
        {
            sqlVersionSpecificSettings["OracleDatabaseName"] = configuration["oracleDatabaseName"]!;

            var upperCaseEscapedNames = configuration["upperCaseEscapedNames"];
            if (upperCaseEscapedNames is null || upperCaseEscapedNames == "")
                upperCaseEscapedNames = "false";

            sqlVersionSpecificSettings["UpperCaseEscapedNames"] = upperCaseEscapedNames;
        }

        settings.SqlVersionSpecificSettings = sqlVersionSpecificSettings;

        return settings;
    }

    public static Settings GetDefaultSettings(SqlEngineVersion version)
    {
        var settings = new Settings();

        var sqlVersionSpecificSettings = new SqlVersionSpecificSettings();

        if (version is MsSqlVersion)
        {
            sqlVersionSpecificSettings["DefaultSchema"] = "dbo";
        }

        settings.SqlVersionSpecificSettings = sqlVersionSpecificSettings;

        return settings;
    }
}