using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class UniqueName
{
    private readonly int _maxNameLength;

    public UniqueName(int maxNameLength = 31)
    {
        _maxNameLength = maxNameLength;
    }

    private readonly Dictionary<string, string> _originalNamesToUniqueNames = [];
    private readonly Dictionary<string, int> _maxLenghtNamePartToNumber = [];

    public string GetUniqueName(string originalName)
    {
        if (!_originalNamesToUniqueNames.ContainsKey(originalName))
            _originalNamesToUniqueNames.Add(originalName, CreateUniqueName(originalName));

        return _originalNamesToUniqueNames[originalName];
    }

    private string CreateUniqueName(string name)
    {
        var uniqueName = name;
        var maxLengthName = name.Substring(0, Math.Min(name.Length, _maxNameLength));

        if (!_maxLenghtNamePartToNumber.ContainsKey(maxLengthName))
            _maxLenghtNamePartToNumber[maxLengthName] = 1;

        if (name.Length > _maxNameLength)
        {
            var existingNumber = _maxLenghtNamePartToNumber[maxLengthName];

            uniqueName = existingNumber == 1
                ? maxLengthName
                : GetNameWithNumberAtEnd(name);
        }

        while (_originalNamesToUniqueNames.Any(i => string.Equals(i.Value, uniqueName, StringComparison.OrdinalIgnoreCase)))
        {
            uniqueName = GetNameWithNumberAtEnd(name);
            _maxLenghtNamePartToNumber[maxLengthName]++;
        }

        return uniqueName;
    }

    private string GetNameWithNumberAtEnd(string name)
    {
        var maxLengthName = name.Substring(0, _maxNameLength);

        return name.Substring(0, _maxNameLength - _maxLenghtNamePartToNumber[maxLengthName].ToString(CultureInfo.InvariantCulture).Length) + _maxLenghtNamePartToNumber[maxLengthName].ToString(CultureInfo.InvariantCulture);
    }
}