using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using EllipticCurve.Utils;


namespace Application.UseCases
{
    public class AuthServices : IAuthServices
    {
        private readonly IAuthCommands _commands;
        private readonly IAuthQueries _queries;

        public AuthServices(IAuthCommands commands, IAuthQueries queries)
        {
            _commands = commands;
            _queries = queries;
        }

        public async Task<AuthResponse> CreateAuthentication(AuthReq req, byte[] passwordHash, byte[] passwordSalt)
        {
            Authentication auth = new Authentication
            {
                Email = req.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            Authentication create = await _commands.InsertAuthentication(auth);

            if(create == null)
            {
                return null;
            }
            else
            {
                AuthResponse authResponse = new AuthResponse
                {
                    Id = create.AuthId,
                    Email = req.Email,
                    UserId = create.UserId,
                };

                return authResponse;
            }
        }

        public async Task<AuthResponseComplete> GetAuthentication(AuthReq req)
        {
            var password = req.Password;
            var mail = req.Email;

            var auth = await _queries.GetAuthByEmail(mail);

            if (auth == null)
            {
                return null;
            }

            AuthResponse response = new AuthResponse
            {
                Id = auth.AuthId,
                Email = auth.Email,
                UserId = auth.UserId
            };

            AuthResponseComplete responseComplete = new AuthResponseComplete
            {
                AuthResponse = response,
                PasswordHash = auth.PasswordHash,
                PasswordSalt = auth.PasswordSalt
            };

            return responseComplete;
        }

        public async Task<AuthResponse> GetMail(Guid authId)
        {
            var response = await _queries.SelectMailByAuthId(authId);

            return response;
        }

        public async Task<AuthResponse> ChangePassword(Guid authId, ChangePassReq req, byte[] passwordHash, byte[] passwordSalt)
        {
            var query = await _commands.AlterAuth(authId, passwordHash, passwordSalt);

            if (query != null && query.PasswordHash == passwordHash && query.PasswordSalt == passwordSalt)
            {
                AuthResponse resp = new AuthResponse
                {
                    Id = query.AuthId,
                    Email = query.Email,
                    UserId = query.UserId,
                };

                return resp;
            }
            else
            {
                return null;
            }
        }
    }
}
