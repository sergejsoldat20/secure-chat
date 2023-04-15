using messages_backend.Data;
using messages_backend.Helpers;
using messages_backend.Models.DTO;
using messages_backend.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace messages_backend.Services
{
    public interface IMessagesService
    {
        List<MessagePartition> DivideMessage(string message, Guid receiverId, Guid senderId, string signature);
        Message ComposeMessage(List<MessagePartition> dividedMessage, Guid recieverId, Guid senderId);
        void SaveMessage(Message message);
        List<MessagePartition> DivideAndEncrypt(string message, Guid receiverId, Guid senderId);
    }

    public class MessagesService : IMessagesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ICryptoService _cryptoService;

        public MessagesService(ApplicationDbContext context,
            IAccountService accountService,
            ICryptoService cryptoService)
        {
            _context = context;
            _accountService = accountService;
            _cryptoService = cryptoService;
        }

        public Message ComposeMessage(List<MessagePartition> dividedMessage,
            Guid recieverId, Guid senderId)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                string rsaKeyString = _accountService.GetMainKey(recieverId);
                RSA.FromXmlString(rsaKeyString);

                string base64Message = string.Join("", dividedMessage
                    .OrderBy(x => x.CurrentPart)
                    .Select(x => x.MessagePart)
                    .ToList());

                byte[] decryptedMessage = _cryptoService.DecryptMessage(
                    System.Convert.FromBase64String(base64Message),
                    RSA.ExportParameters(true));

                // read sender private key and verify signature 
                string senderKeyString = _accountService.GetMainKey(senderId);

                // get signature from first element
                byte[] signature = System.Convert.FromBase64String(dividedMessage.First().Signature);

                // if signature is verified return message
                if (_cryptoService.VerifyMessagePartition(signature,
                    Encoding.UTF8.GetString(decryptedMessage),
                    senderId, senderKeyString))
                {
                    return new Message
                    {
                        Text = Encoding.UTF8.GetString(decryptedMessage),
                        SenderId = senderId,
                        RecieverId = recieverId
                    };

                }
                else 
                {
                    throw new AppException("Signature is not verified!");
                }
            }
        }

        public List<MessagePartition> DivideAndEncrypt(string message, Guid receiverId, Guid senderId)
        {

            var result = new List<MessagePartition>();
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                // get main key from receiver and encrypt with his public key
                string rsaKeyString = _accountService.GetMainKey(receiverId);
                RSA.FromXmlString(rsaKeyString);

                byte[] encryptedMessage = _cryptoService.
                    EncryptMessage(Encoding.UTF8.GetBytes(message), RSA.ExportParameters(false));
                string messageBase64 = System.Convert.ToBase64String(encryptedMessage);

                // before dividing message sign it with sender's private key
                string senderKeyString = _accountService.GetMainKey(senderId);

                byte[] signature = _cryptoService.SignMessagePartition(message, senderId, senderKeyString);

                result = DivideMessage(messageBase64, receiverId, senderId, 
                    System.Convert.ToBase64String(signature));
                return result;
            }

        }

        public List<MessagePartition> DivideMessage(string message, Guid receiverId, Guid senderId, string signature)
        {
            Random random = new Random();
            List<MessagePartition> result = new List<MessagePartition>();


            int totalParts = random.Next(message.Length / 2) + 1;
            int partLength = message.Length / totalParts;
            List<string> strings = message
                      .Select((c, i) => new { c, i })
                      .GroupBy(x => x.i / partLength)
                      .Select(g => new string(g.Select(x => x.c).ToArray()))
                      .ToList();

            Guid partitionId = new Guid();

            for (int i = 0; i < strings.Count; i++)
            {
                result.Add(new MessagePartition
                {
                    Id = partitionId,
                    TotalParts = strings.Count,
                    CurrentPart = i,
                    MessagePart = strings[i],
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Signature = signature
                });
            }
            return result;
        }

        public void SaveMessage(Message message)
        {
            if (message != null)
            {
                 _context.Add(message);
                 _context.SaveChanges();
            }
        }
    }
}
