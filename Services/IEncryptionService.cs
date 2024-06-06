namespace HttpServer.Services;

public interface IEncryptionService
{
    (string Key, string IV) GenerateKeyAndIV();

    string Encrypt(string value, string key, string iv);

    string Decrypt(string value, string key, string iv);
}