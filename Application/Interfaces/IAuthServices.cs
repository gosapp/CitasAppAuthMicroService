using Application.Models;


namespace Application.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponse> CreateAuthentication(AuthReq req, byte[] passwordHash, byte[] passwordSalt);
        Task<AuthResponseComplete> GetAuthentication(AuthReq req);
        Task<AuthResponse> GetMail(Guid authId);
        Task<AuthResponse> ChangePassword(Guid authId, ChangePassReq req, byte[] passwordHash, byte[] passwordSalt);
    }
}
