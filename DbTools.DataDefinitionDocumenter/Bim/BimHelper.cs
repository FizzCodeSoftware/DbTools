using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
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

    public static void SetDefaultDataSources(BimDTO.BimGeneratorModel bimGeneratorModel, string database, string? server = null)
    {
        bimGeneratorModel.DataSources.Add(GetDefaultDataSource(database, server));
    }

    public static BimDTO.DataSource GetDefaultDataSource(string database, string? server = null)
    {
        var address = new BimDTO.Address()
        {
            Database = database
        };

        if (server != null)
            address.Server = server;

        var connectionDetails = new BimDTO.ConnectionDetails
        {
            Address = address
        };

        var credential = new BimDTO.Credential
        {
            Path = $"{connectionDetails.Address.Server};{database}",
            Username = "sa"
        };

        var dataSource = new BimDTO.DataSource
        {
            Type = "structured",
            // TODO SQL
            Name = $"SQL/{connectionDetails.Address.Server};{database}",
            ConnectionDetails = connectionDetails,
            Credential = credential
        };

        return dataSource;
    }

    public static void SetDefaultPartition(BimDTO.Table table, SqlTable sqlTable, string database, string? server = null)
    {
        var partition = new BimDTO.Partition()
        {
            Name = "Partition",
            DataView = "full",
            Source = GetDefaultPartitionSource(sqlTable, database, server)
        };

        table.Partitions.Add(partition);
    }

    public static BimDTO.PartitionSource GetDefaultPartitionSource(SqlTable sqlTable, string database, string? server = null)
    {
        var partitionSource = new BimDTO.PartitionSource
        {
            Type = "m"
        };

        partitionSource.Expression.Add("let");
        // TODO SQL
        partitionSource.Expression.Add($"    Source = #\"SQL/{server ?? "localhost"};{database}\",");
        partitionSource.Expression.Add($"    {sqlTable.SchemaAndTableName!.Schema}_{sqlTable.SchemaAndTableName.TableName} = Source{{[Schema =\"{sqlTable.SchemaAndTableName.Schema}\",Item=\"{sqlTable.SchemaAndTableName.TableName}\"]}}[Data]");
        partitionSource.Expression.Add("in");
        partitionSource.Expression.Add($"     {sqlTable.SchemaAndTableName.Schema}_{sqlTable.SchemaAndTableName.TableName}");

        return partitionSource;
    }
}