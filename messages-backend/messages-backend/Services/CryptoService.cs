using messages_backend.Models.DTO;
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
        byte[] SignMessagePartition(string messagePartition, Guid senderId, string RSASenderKeyString);
        bool VerifyMessagePartition(byte[] signature, string messagePartition, Guid senderId, string RSASenderKeyString);
      
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

        public byte[] SignMessagePartition(string messagePartition, Guid senderId, string senderRSAKeyString)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
               //  string senderRSAKeyString = _accountService.GetMainKey(senderId);
                RSA.FromXmlString(senderRSAKeyString);

                // we are using sha256 hashing algorithm
                SHA256 shaAlg = SHA256.Create();

                // create hash
                byte[] messageHash = shaAlg.ComputeHash(Encoding.UTF8.GetBytes(messagePartition));

                // sign message with RSA 
                byte[] signature = RSA.SignHash(messageHash, CryptoConfig.MapNameToOID("SHA256"));
                return signature;
            }
           
        }

        public bool VerifyMessagePartition(byte[] signature, string messagePartition, Guid senderId, string senderRSAKeyString)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
               //  string senderRSAKeyString = _accountService.GetMainKey(senderId);
                RSA.FromXmlString(senderRSAKeyString);

                SHA256 shaAlg = SHA256.Create();
                byte[] messageHash = shaAlg.ComputeHash(Encoding.UTF8.GetBytes(messagePartition));

                return RSA.VerifyHash(messageHash, 
                    CryptoConfig.MapNameToOID("SHA256"), signature);
            }
        }
    }
}
