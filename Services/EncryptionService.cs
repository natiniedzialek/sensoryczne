using System.Security.Cryptography;

namespace HttpServer.Services
{
    public class EncryptionService : IEncryptionService
    {
        public (string Key, string IV) GenerateKeyAndIV()
        {
            using var aes = Aes.Create();
            aes.KeySize = 256; // Ensure AES 256 is used
            aes.GenerateKey();
            aes.GenerateIV();

            var keyBase64 = Convert.ToBase64String(aes.Key);
            var ivBase64 = Convert.ToBase64String(aes.IV);

            return (Key: keyBase64, IV: ivBase64);
        }

        public string Encrypt(string value, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256; // Ensure AES 256 is used
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(value);
                }
            }

            var encrypted = msEncrypt.ToArray();
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string value, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256; // Ensure AES 256 is used
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var msDecrypt = new MemoryStream(Convert.FromBase64String(value));
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using var srDecrypt = new StreamReader(csDecrypt);
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
