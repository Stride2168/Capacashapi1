using Capacash.Application.Common.Interfaces;
using QRCoder;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using SixLabors.ImageSharp; // ImageSharp
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Capacash.Infrastructure.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly string _encryptionKey = "u2VSRyk9F4mjQYx1bpl9Y+vkLqdaTBhs+wm3Qi3FJJ4="; // should be in config/env

        // Ensure the key is always 32 bytes for AES-256 encryption
        private byte[] GetAesKey()
        {
            var keyBytes = Convert.FromBase64String(_encryptionKey);

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
            aes.Key = GetAesKey();
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(json);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var combined = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, combined, aes.IV.Length, cipherBytes.Length);

            return Convert.ToBase64String(combined);
        }

        // Implement the missing method from the interface
        public string GenerateQrCodeBase64(string encryptedPayload)
        {
            // Generate the QR Code base image
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(encryptedPayload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(20); // Resolution

            // Convert QR Code bytes to a base64 string
            return Convert.ToBase64String(qrBytes);
        }

        public string GenerateQrCodeBase64WithLogo(string payload, byte[] logoBytes)
        {
            // Generate the QR Code base image
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(20); // Resolution

            // Load QR code as an ImageSharp image
            using var qrImage = Image.Load<Rgba32>(qrBytes);

            // Load the logo as an ImageSharp image
            using var logoImage = Image.Load<Rgba32>(logoBytes);

            var logoWidth = qrImage.Width / 5;  // Size the logo appropriately
            var logoHeight = qrImage.Height / 5;

            // Place the logo in the center of the QR code
            logoImage.Mutate(x => x.Resize(logoWidth, logoHeight));
            qrImage.Mutate(x => x.DrawImage(logoImage, new Point((qrImage.Width - logoWidth) / 2, (qrImage.Height - logoHeight) / 2), 1));

            // Save the final image to a MemoryStream and return it as a Base64 string
            using var ms = new MemoryStream();
            qrImage.SaveAsPng(ms);
            return Convert.ToBase64String(ms.ToArray());
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
