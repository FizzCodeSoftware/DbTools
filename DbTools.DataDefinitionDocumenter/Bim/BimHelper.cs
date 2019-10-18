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
    }
}