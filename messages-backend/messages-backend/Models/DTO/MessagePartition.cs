namespace messages_backend.Models.DTO
{
    public class MessagePartition
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Signature { get; set; } = string.Empty;
        public string MessagePart { get; set; } = string.Empty;
        public int TotalParts { get; set; } 
        public int CurrentPart { get; set; }
    }
}
