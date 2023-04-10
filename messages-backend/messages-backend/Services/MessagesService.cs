namespace messages_backend.Services
{
    public interface IMessagesService
    {
        IEnumerable<byte[]> DivideMessage(string message);
        string ComposeMessage(IEnumerable<byte[]> dividedMessage);

    }
    public class MessagesService : IMessagesService
    {
        public string ComposeMessage(IEnumerable<byte[]> dividedMessage)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte[]> DivideMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
