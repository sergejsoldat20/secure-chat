using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace messages_backend.Services
{
    public interface ICryptoService
    {
        byte[] DecryptMessage(byte[] encryptedPart, RSAParameters RSAKeyInfo);
        byte[] EncryptMessage(byte[] messagePart, RSAParameters RSAKeyInfo);
        string GenerateRSA();
      
    }
    public class CryptoService : ICryptoService
    {
        public byte[] DecryptMessage(byte[] encryptedPart, RSAParameters RSAKeyInfo)
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
                return decryptedData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public byte[] EncryptMessage(byte[] dataForEncryption, RSAParameters RSAKeyInfo)
        {
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
