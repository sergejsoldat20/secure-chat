namespace messages_backend.Models.DTO
{
    public class SendMessage
    {
        public Guid ReceiverId { get; set; }
        public string Text { get; set; }
    }
}
