using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace messages_backend.Services
{
    public interface ICryptoService
    {
        string DecryptMessage(byte[] encryptedPart, RSAParameters RSAKeyInfo);
        byte[] EncryptMessage(string messagePart, RSAParameters RSAKeyInfo);
        string GenerateRSA();
      
    }
    public class CryptoService : ICryptoService
    {
        public string DecryptMessage(byte[] encryptedPart, RSAParameters RSAKeyInfo)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();

            try
            {
                byte[] decryptedData;

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    decryptedData = RSA.Decrypt(encryptedPart, false);
                }
                return ByteConverter.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public byte[] EncryptMessage(string messagePart, RSAParameters RSAKeyInfo)
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataForEncryption = ByteConverter.GetBytes(messagePart);
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo); 
                    encryptedData = RSA.Encrypt(dataForEncryption, false);
                }
                return encryptedData;
            } catch (CryptographicException ex) 
            {
                return null;            
            }
        }

        public string GenerateRSA()
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                return RSA.ToXmlString(true);
            }
        }
    }
}
