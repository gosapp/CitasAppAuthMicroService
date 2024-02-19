
namespace Application.Models
{
    public class AuthResponseComplete
    {
        public AuthResponse AuthResponse {  get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
