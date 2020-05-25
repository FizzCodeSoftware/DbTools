namespace FizzCode.DbTools.DataDefinition.Checker
{
    using System.Collections.Generic;
    using System;

    public class PluralChecker
    {
        private readonly string _singularsInput = @"
os
us
bus
gas
yes
abs
cos
ems
his
ops
sos
boss
basis
abyss
arcus
lens
mass";
        private readonly List<string> _singulars = new List<string>();

        public PluralChecker()
        {
            _singulars.AddRange(_singularsInput.Split("\r\n"));
        }

        public bool CheckValidity(string tableName)
        {
            return !tableName.EndsWith('s')
                || _singularsInput.IndexOf(tableName, StringComparison.InvariantCulture) != -1;
        }
    }
}
