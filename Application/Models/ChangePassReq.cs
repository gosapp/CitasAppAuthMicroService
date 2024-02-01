

namespace Application.Models
{
    public class ChangePassReq
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string RepeatNewPassword { get; set; }
    }
}
