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

            dataSource.ConnectionDetails = connectionDetails;

            var credential = new BimDTO.Credential();

            credential.Path = $"{connectionDetails.Address.Server};{database}";
            credential.Username = "sa";

            dataSource.Credential = credential;

            return dataSource;
        }
    }
}