namespace messages_backend.Models.DTO
{
    public class SendMessage
    {
        public Guid ReceiverId { get; set; }
        public byte[] Message { get; set; } = new byte[8];
    }
}
