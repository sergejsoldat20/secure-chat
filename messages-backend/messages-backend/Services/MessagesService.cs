using messages_backend.Data;
using messages_backend.Models.DTO;
using messages_backend.Models.Entities;
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
        List<byte[]> GetEncryptedMessageParts(SendMessage message, Guid senderId);
        MessagePartition DecryptMessagePart(byte[] encryptedData, Guid senderId, Guid receiverId);
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
            string messageText = string.Join("", dividedMessage.OrderBy(x => x.CurrentPart).ToList().Select(x => x.MessagePart));
            var message = new Message
            {
                RecieverId = recieverId,
                SenderId = senderId,
                Text = messageText
            };
            return message;
        }

        public MessagePartition DecryptMessagePart(byte[] encryptedData, Guid senderId, Guid receiverId)
        {
            throw new NotImplementedException();
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

        public List<byte[]> GetEncryptedMessageParts(SendMessage message, Guid senderId)
        {
            UnicodeEncoding ByteEncoder = new UnicodeEncoding();
            var dividedMessage = DivideMessage(message.Text);
            var encryptedMessageParts = new List<byte[]>();    
            using (RSACryptoServiceProvider RSAReceiver = new RSACryptoServiceProvider())
            {
                string RSAReceiverKeyString = _accountService.GetMainKey(message.ReceiverId);
                RSAReceiver.FromXmlString(RSAReceiverKeyString);

                string RSASenderKeyString = _accountService.GetComainKey(senderId);
                RSACryptoServiceProvider RSASender = new RSACryptoServiceProvider();
                RSASender.FromXmlString(RSASenderKeyString);

                foreach (var messagePart in dividedMessage)
                {
                    string messagePartJson = JsonConvert.SerializeObject(messagePart);


                    // Encrypt message with receiver's main public key
                    byte[] firstEncryptionData = _cryptoService.
                        EncryptMessage(ByteEncoder.GetBytes(messagePartJson),
                        RSAReceiver.ExportParameters(false));

                    // Encrypt second time with sender's private comain key
                    byte[] secondEncryption = _cryptoService.
                        EncryptMessage(firstEncryptionData, 
                        RSASender.ExportParameters(true));

                    // Add encrypted data to list 
                    encryptedMessageParts.Add(secondEncryption);
                }
            }
            return encryptedMessageParts;
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
