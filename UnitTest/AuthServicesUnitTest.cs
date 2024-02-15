using Application.Interfaces;
using Application.Models;
using Application.UseCases;
using Domain.Entities;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;

namespace UnitTest
{
    public class AuthServicesUnitTest
    {
        [Fact]
        public async Task ChangePasswordShouldReturnCorrect()
        {
            //Arrange (Preparar)

            //Act (Probar)

            //Assert (Verificar)

        }

        [Fact]
        public async Task GetAuthenticationShouldReturnCorrect()
        {
            //Arrange (Preparar)
            var mockCommand = new Mock<IAuthCommands>();
            var mockQuery = new Mock<IAuthQueries>();
            var mockEncryptServices = new Mock<IEncryptServices>();

            var service = new AuthServices(mockCommand.Object, mockQuery.Object, mockEncryptServices.Object);

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
            var mockEncryptServices = new Mock<IEncryptServices>();

            var service = new AuthServices(mockCommand.Object, mockQuery.Object, mockEncryptServices.Object);

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
