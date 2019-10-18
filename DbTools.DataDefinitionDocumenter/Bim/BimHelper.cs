using System;
using FizzCode.DbTools.DataDefinition;

namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public static class BimHelper
    {
        public static void SetDefaultAnnotations(BimDTO.BimGeneratorModel bimGeneratorModel)
        {
            bimGeneratorModel.Annotations.Add(new BimDTO.Annotation()
            {
                Name = "ClientCompatibilityLevel",
                Value = "500"
            });
        }

        public static void SetDefaultDataSources(BimDTO.BimGeneratorModel bimGeneratorModel, string database, string server = null)
        {
            bimGeneratorModel.DataSources.Add(GetDefaultDataSource(database, server));
        }

        public static BimDTO.DataSource GetDefaultDataSource(string database, string server = null)
        {
            var dataSource = new BimDTO.DataSource();
            dataSource.Type = "structured";

            var connectionDetails = new BimDTO.ConnectionDetails();

            var address = new BimDTO.Address();

            if (server != null)
                address.Server = server;

            address.Database = database;

            connectionDetails.Address = address;

            // TODO SQL
            dataSource.Name = $"SQL/{connectionDetails.Address.Server};{database}";

            dataSource.ConnectionDetails = connectionDetails;

            var credential = new BimDTO.Credential();

            credential.Path = $"{connectionDetails.Address.Server};{database}";
            credential.Username = "sa";

            dataSource.Credential = credential;

            return dataSource;
        }

        public static void SetDefaultPartition(BimDTO.Table table, SqlTable sqlTable, string database, string server = null)
        {
            var partition = new BimDTO.Partition()
            {
                Name = "Partition",
                DataView = "full",
            };

            SetDefaultPartitionSource(partition, sqlTable, database, server);

            table.Partitions.Add(partition);
        }

        public static void SetDefaultPartitionSource(BimDTO.Partition partition, SqlTable sqlTable, string database, string server = null)
        {
            var partitionSource = new BimDTO.PartitionSource();
            partitionSource.Type = "m";

            partitionSource.Expression.Add("let");
            // TODO SQL
            partitionSource.Expression.Add($"    Source = #\"SQL/{server ?? "localhost"};{database}\",");
            partitionSource.Expression.Add($"    {sqlTable.SchemaAndTableName.Schema}_{sqlTable.SchemaAndTableName.TableName} = Source{{[Schema =\"{sqlTable.SchemaAndTableName.Schema}\",Item=\"{sqlTable.SchemaAndTableName.TableName}\"]}}[Data]");
            partitionSource.Expression.Add("in");
            partitionSource.Expression.Add($"     {sqlTable.SchemaAndTableName.Schema}_{sqlTable.SchemaAndTableName.TableName}");

            partition.Source = partitionSource;
        }

        public static string MapType(SqlType type)
        {
            return type switch
            {
                SqlType.Int32 => "int64",
                SqlType.Int16 => "int64",
                SqlType.Byte => "int64",
                SqlType.Int64 => "int64",
                SqlType.Decimal => "double",
                // SqlType.Money => "currency",
                // TODO 
/*
"formatString": "#,0.00 \"Ft\";-#,0.00 \"Ft\";#,0.00 \"Ft\"",
"annotations": [
    {
    "name": "Format",
    "value": "<Format Format=\"Currency\" Accuracy=\"2\" ThousandSeparator=\"True\"><Currency LCID=\"1038\" DisplayName=\"Ft Hungarian (Hungary)\" Symbol=\"Ft\" PositivePattern=\"3\" NegativePattern=\"8\" /></Format>"
    }
] 
*/
                // SqlType.Money => "decimal",
                SqlType.NVarchar => "string",
                SqlType.NChar => "string",
                SqlType.Varchar => "string",
                SqlType.Char => "string",
                // TODO ragne outside is used as string
                // https://docs.microsoft.com/en-us/analysis-services/tabular-models/data-types-supported-ssas-tabular
                SqlType.DateTime => "dateTime",
                // TODO
                SqlType.DateTimeOffset => "string",
                SqlType.Date => "string",
                SqlType.Boolean => "boolean",
                // TODO normally "decimal"
                SqlType.Single => "currency",
                SqlType.Double => "currency",

                /*"xml" => SqlType.Xml,
                "uniqueidentifier" => SqlType.Guid,
                "binary" => SqlType.Binary,
                "image" => SqlType.Image,
                "varbinary" => SqlType.VarBinary,
                "ntext" => SqlType.NText,*/
                _ => throw new NotImplementedException($"Unmapped SqlType: {type}."),
            };
        }
    }
}