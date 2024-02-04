using System;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;

namespace FizzCode.DbTools.DataDefinition;
public class Generic1TypeMapper : AbstractTypeMapper
{
    public override SqlEngineVersion SqlVersion => GenericVersion.Generic1;

    public override ISqlType MapFromGeneric1(ISqlType genericType)
    {
        throw new NotImplementedException();
    }

    public override ISqlType MapToGeneric1(ISqlType sqlType)
    {
        throw new NotImplementedException();
    }
}
