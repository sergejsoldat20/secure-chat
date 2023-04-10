namespace messages_backend.Models.DTO
{
    public class MessagePartition
    {
        public string MessagePart { get; set; } = string.Empty;
        public int TotalParts { get; set; } 
        public int CurrentPart { get; set; }
    }
}
