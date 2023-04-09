using System.ComponentModel.DataAnnotations;

namespace messages_backend.Models.DTO
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public string JwtToken { get; set; }
    }
}
