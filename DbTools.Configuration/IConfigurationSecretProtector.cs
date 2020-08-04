namespace FizzCode.DbTools.Configuration
{
    public interface IConfigurationSecretProtector
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }
}