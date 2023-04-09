namespace messages_backend.Services
{
    public interface ICryptoService
    {
        IEnumerable<byte[]> DivideMessage(string message);
        string ComposeMessage(IEnumerable<byte[]> dividedMessage);
    }
    public class CryptoService : ICryptoService
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
