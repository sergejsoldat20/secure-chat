using messages_backend.Data;
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
        List<MessagePartition> DivideMessage(string message);
        Message ComposeMessage(List<MessagePartition> dividedMessage, Guid recieverId, Guid senderId);
        void SaveMessage(Message message);
        List<MessagePartition> DivideAndEncrypt(string message, Guid id);
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

                return new Message
                {
                    Text = Encoding.UTF8.GetString(decryptedMessage),
                    SenderId = senderId,
                    RecieverId = recieverId
                };
            }
        }   

        public List<MessagePartition> DivideAndEncrypt(string message, Guid id)
        {
            
            var result = new List<MessagePartition>();
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string rsaKeyString = _accountService.GetMainKey(id);
                rsa.FromXmlString(rsaKeyString);

                byte[] encryptedMessage = _cryptoService.
                    EncryptMessage(Encoding.UTF8.GetBytes(message), rsa.ExportParameters(false));
                 string messageBase64 = System.Convert.ToBase64String(encryptedMessage);
                result = DivideMessage(messageBase64);

                return result;
            }
            
        }

        public List<MessagePartition> DivideMessage(string message)
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

            for (int i = 0; i < strings.Count; i++)
            {
                result.Add(new MessagePartition
                {
                    TotalParts = totalParts,
                    CurrentPart = i,
                    MessagePart = strings[i]
                });
            }
            return result;
        }

        public void SaveMessage(Message message)
        {
            if (message != null)
            {
                _context.Messages.Add(message);
                _context.SaveChanges();
            }
        }
    }
}
