namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;

    public class GeneratorContext
    {
        public Settings Settings { get; set; }
        public Logger Logger { get; set; }
    }
}