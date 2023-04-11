namespace messages_backend.Models.DTO
{
    public class TorMessage
    {
        public Guid RecieverId { get; set; }
        public byte[] MessagePartition { get; set; }
        public byte[] Signature { get; set; }
    }
}
