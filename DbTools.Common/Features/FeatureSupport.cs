namespace FizzCode.DbTools.Common;

public class FeatureSupport(Support support, string description)
{
    public Support Support { get; set; } = support;
    public string Description { get; set; } = description;
}
