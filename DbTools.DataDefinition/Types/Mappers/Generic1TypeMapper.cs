namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Configuration;

    public class Generic1TypeMapper : AbstractTypeMapper
    {
        public override SqlEngineVersion SqlVersion => GenericVersion.Generic1;

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            throw new NotImplementedException();
        }

        public override SqlType MapToGeneric1(SqlType sqlType)
        {
            throw new NotImplementedException();
        }
    }
}
