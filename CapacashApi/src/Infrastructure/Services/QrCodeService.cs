using Capacash.Application.Common.Interfaces;
using QRCoder;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Capacash.Infrastructure.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly string _encryptionKey = "u2VSRyk9F4mjQYx1bpl9Y+vkLqdaTBhs+wm3Qi3FJJ4="; // should be in config/env

        // Ensure the key is always 32 bytes for AES-256 encryption
        private byte[] GetAesKey()
        {
            // Convert the encryption key to a byte array (Base64 decoded)
            var keyBytes = Convert.FromBase64String(_encryptionKey);
            
            // If the key is not 32 bytes, hash it to create a 32-byte key (e.g., using SHA256)
            if (keyBytes.Length != 32)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    return sha256.ComputeHash(keyBytes); // Produces a 32-byte hash
                }
            }

            return keyBytes; // Return the key if it's already 32 bytes
        }

        public string EncryptPayload(object payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            using var aes = Aes.Create();
            aes.Key = GetAesKey(); // Use the valid 32-byte key
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(json);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var combined = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

            return Convert.ToBase64String(combined);
        }

        public string GenerateQrCodeBase64(string payload)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrBytes);
        }

        public T DecryptPayload<T>(string encryptedPayload)
        {
            var combined = Convert.FromBase64String(encryptedPayload);

            using var aes = Aes.Create();
            aes.Key = GetAesKey(); // Use the valid 32-byte key
            var iv = new byte[aes.BlockSize / 8];
            var cipherBytes = new byte[combined.Length - iv.Length];

            Buffer.BlockCopy(combined, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(combined, iv.Length, cipherBytes, 0, cipherBytes.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            var json = Encoding.UTF8.GetString(plainBytes);

            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}
