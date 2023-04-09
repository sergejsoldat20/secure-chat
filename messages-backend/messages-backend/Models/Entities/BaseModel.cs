using System.ComponentModel.DataAnnotations;

namespace messages_backend.Models.Entities
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[8];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string StrId
        {
            get
            {
                return Id.ToString();
            }
        }

        public BaseModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
