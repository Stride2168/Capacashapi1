using System;

namespace Capacash.Application.Common.Interfaces
{
    public interface IQrCodeService
    {
        string EncryptPayload(object payload);               // JSON -> AES256 -> base64
        string GenerateQrCodeBase64(string encryptedPayload); // base64 payload -> base64 QR image
        T DecryptPayload<T>(string encryptedPayload);  
    }
}
