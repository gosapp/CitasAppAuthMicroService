using Application.Interfaces;
using Application.Models;
using Application.UseCases;
using Domain.Entities;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using System.Linq.Expressions;

namespace UnitTest
{
    public class AuthServicesUnitTest
    {
        [Fact]
        public async Task ChangePasswordShouldReturnNull()
        {
            //Arrange (Preparar)
            var mockCommand = new Mock<IAuthCommands>();
            var mockQuery = new Mock<IAuthQueries>();

            var service = new AuthServices(mockCommand.Object, mockQuery.Object);

            var authId = Guid.NewGuid();
            var email = "test@expresso.com";
            var userId = 1;
            Random rnd = new Random();
            byte[] hash = new byte[64];
            byte[] salt = new byte[128];
            rnd.NextBytes(hash);
            rnd.NextBytes(salt);

            ChangePassReq req = new ChangePassReq
            {
                Password = "Express0.",
                NewPassword = "NewExpress0.",
                RepeatNewPassword = "NewExpress0."
            };

            mockCommand.Setup(q => q.AlterAuth(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(() => null);

            //Act (Probar)
            var result = await service.ChangePassword(authId, req, hash, salt);

            //Assert (Verificar)
            result.Should().BeNull();
        }

        [Fact]
        public async Task ChangePasswordShouldReturnCorrect()
        {
            //Arrange (Preparar)
            var mockCommand = new Mock<IAuthCommands>();
            var mockQuery = new Mock<IAuthQueries>();

            var service = new AuthServices(mockCommand.Object, mockQuery.Object);

            var authId = Guid.NewGuid();
            var email = "test@expresso.com";
            var userId = 1;
            Random rnd = new Random();
            byte[] hash = new byte[64];
            byte[] salt = new byte[128];
            rnd.NextBytes(hash);
            rnd.NextBytes(salt);

            ChangePassReq req = new ChangePassReq
            {
                Password = "Express0.",
                NewPassword = "NewExpress0.",
                RepeatNewPassword = "NewExpress0."
            };

            Authentication auth = new Authentication
            {
                AuthId = authId,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserId = 1
            };

            AuthResponse response = new AuthResponse
            {
                Id = authId,
                Email = email,
                UserId = userId,
            };
            mockCommand.Setup(q => q.AlterAuth(authId, hash, salt)).ReturnsAsync(auth);

            //Act (Probar)
            var result = await service.ChangePassword(authId, req, hash, salt);

            //Assert (Verificar)
            result.Id.Should().Be(authId);
            result.Email.Should().Be(email);
            result.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetAuthenticationShouldReturnCorrect()
        {
            //Arrange (Preparar)
            var mockCommand = new Mock<IAuthCommands>();
            var mockQuery = new Mock<IAuthQueries>();

            var service = new AuthServices(mockCommand.Object, mockQuery.Object);

            Random rnd = new Random();
            byte[] hash = new byte[64];
            byte[] salt = new byte[128];
            rnd.NextBytes(hash);
            rnd.NextBytes(salt);

            AuthReq authReq = new AuthReq
            {
                Email = "mail1@expresso.com",
                Password = "Express0."
            };

            Authentication authentication = new Authentication
            {
                AuthId = Guid.NewGuid(),
                Email = authReq.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserId = 1
            };

            AuthResponseComplete authRespComplete = new AuthResponseComplete
            {
                AuthResponse = new AuthResponse
                {
                    Id = authentication.AuthId,
                    Email = authReq.Email,
                    UserId = 1
                },
                PasswordHash = hash,
                PasswordSalt = salt
            };
            mockQuery.Setup(q => q.GetAuthByEmail(It.Is<string>(x => x.Equals("mail1@expresso.com")))).ReturnsAsync(authentication);

            //Act (Probar)
            var result = await service.GetAuthentication(authReq);

            //Assert (Verificar)
            result.AuthResponse.Id.Should().Be(authentication.AuthId);
            result.AuthResponse.Email.Should().Be("mail1@expresso.com");
            result.AuthResponse.UserId.Should().Be(1);
            result.PasswordHash.Should().BeEquivalentTo<Byte>(hash);
            result.PasswordSalt.Should().BeEquivalentTo<Byte>(salt);
        }

        [Fact]
        public async Task GetAuthenticationShouldReturnException()
        {
            //Arrange (Preparar)
            var mockCommand = new Mock<IAuthCommands>();
            var mockQuery = new Mock<IAuthQueries>();

            var service = new AuthServices(mockCommand.Object, mockQuery.Object);

            AuthReq authReq = new AuthReq
            {
                Email = "noexisteexpresso.com",
                Password = "Express0."
            };

            Authentication authentication = new Authentication();

            mockQuery.Setup(q => q.GetAuthByEmail(It.Is<string>(x => x.Equals("noexisteexpresso.com")))).ReturnsAsync(() => null);

            //Act (Probar)
            var result = await service.GetAuthentication(authReq);

            //Assert (Verificar)
            result.Should().BeNull();
        }

    }
}
