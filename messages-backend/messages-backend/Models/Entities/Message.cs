namespace messages_backend.Models.Entities
{
    public class Message : BaseModel
    {
        public string Text { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecieverId { get; set; }

    }
}
