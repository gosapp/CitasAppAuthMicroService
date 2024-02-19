using Application.Interfaces;
using Application.Models;
using Application.UseCases;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace UnitTest
{
    public class AuthServicesUnitTest
    {
        private readonly Guid _authId = Guid.NewGuid();
        private readonly int _userId = 1;
        private readonly string _testEmail = "test@expresso.com";
        private readonly Random _rnd = new Random();
        private byte[] _hash = new byte[64];
        private byte[] _salt = new byte[128];
        private readonly string _password = "Express0.";
        private readonly Mock<IAuthCommands> _mockCommand = new Mock<IAuthCommands>();
        private readonly Mock<IAuthQueries> _mockQuery = new Mock<IAuthQueries>();

        [Fact]
        public async Task ChangePasswordShouldReturnNull()
        {
            //Arrange (Preparar)
            var service = new AuthServices(_mockCommand.Object, _mockQuery.Object);

            _rnd.NextBytes(_hash);
            _rnd.NextBytes(_salt);

            ChangePassReq req = new ChangePassReq
            {
                Password = _password,
                NewPassword = "NewExpress0.",
                RepeatNewPassword = "NewExpress0."
            };
            _mockCommand.Setup(q => q.AlterAuth(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(() => null);

            //Act (Probar)
            var result = await service.ChangePassword(_authId, req, _hash, _salt);

            //Assert (Verificar)
            result.Should().BeNull();
        }

        [Fact]
        public async Task ChangePasswordShouldReturnCorrect()
        {
            //Arrange (Preparar)
            var service = new AuthServices(_mockCommand.Object, _mockQuery.Object);

            _rnd.NextBytes(_hash);
            _rnd.NextBytes(_salt);

            ChangePassReq req = new ChangePassReq
            {
                Password = _password,
                NewPassword = "NewExpress0.",
                RepeatNewPassword = "NewExpress0."
            };

            Authentication auth = new Authentication
            {
                AuthId = _authId,
                Email = _testEmail,
                PasswordHash = _hash,
                PasswordSalt = _salt,
                UserId = _userId
            };

            AuthResponse response = new AuthResponse
            {
                Id = _authId,
                Email = _testEmail,
                UserId = _userId,
            };
            _mockCommand.Setup(q => q.AlterAuth(_authId, _hash, _salt)).ReturnsAsync(auth);

            //Act (Probar)
            var result = await service.ChangePassword(_authId, req, _hash, _salt);

            //Assert (Verificar)
            result.Id.Should().Be(_authId);
            result.Email.Should().Be(_testEmail);
            result.UserId.Should().Be(_userId);
        }

        [Fact]
        public async Task GetAuthenticationShouldReturnCorrect()
        {
            //Arrange (Preparar)
            var service = new AuthServices(_mockCommand.Object, _mockQuery.Object);

            _rnd.NextBytes(_hash);
            _rnd.NextBytes(_salt);

            AuthReq authReq = new AuthReq
            {
                Email = _testEmail,
                Password = _password
            };

            Authentication authentication = new Authentication
            {
                AuthId = _authId,
                Email = authReq.Email,
                PasswordHash = _hash,
                PasswordSalt = _salt,
                UserId = _userId
            };

            AuthResponseComplete authRespComplete = new AuthResponseComplete
            {
                AuthResponse = new AuthResponse
                {
                    Id = authentication.AuthId,
                    Email = authReq.Email,
                    UserId = _userId
                },
                PasswordHash = _hash,
                PasswordSalt = _salt
            };
            _mockQuery.Setup(q => q.GetAuthByEmail(It.Is<string>(x => x.Equals(_testEmail)))).ReturnsAsync(authentication);

            //Act (Probar)
            var result = await service.GetAuthentication(authReq);

            //Assert (Verificar)
            result.AuthResponse.Id.Should().Be(authentication.AuthId);
            result.AuthResponse.Email.Should().Be(_testEmail);
            result.AuthResponse.UserId.Should().Be(_userId);
            result.PasswordHash.Should().BeEquivalentTo<Byte>(_hash);
            result.PasswordSalt.Should().BeEquivalentTo<Byte>(_salt);
        }

        [Fact]
        public async Task GetAuthenticationShouldReturnNull()
        {
            //Arrange (Preparar)
            var service = new AuthServices(_mockCommand.Object, _mockQuery.Object);

            AuthReq authReq = new AuthReq
            {
                Email = _testEmail,
                Password = _password
            };

            _mockQuery.Setup(q => q.GetAuthByEmail(It.Is<string>(x => x.Equals(_testEmail)))).ReturnsAsync(() => null);

            //Act (Probar)
            var result = await service.GetAuthentication(authReq);

            //Assert (Verificar)
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAuthenticationShouldReturnCorrect()
        {
            //Arrange
            var service = new AuthServices(_mockCommand.Object, _mockQuery.Object);

            AuthReq authReq = new AuthReq
            {
                Email = _testEmail,
                Password = _password
            };

            Authentication authentication2 = new Authentication
            {
                AuthId = _authId,
                Email = _testEmail,
                PasswordHash = _hash,
                PasswordSalt = _salt,
                UserId = _userId
            };

            AuthResponse authResponse = new AuthResponse
            {
                Id = _authId,
                Email = _testEmail,
                UserId = _userId
            };

           _mockCommand.Setup(c => c.InsertAuthentication(It.IsAny<Authentication>())).ReturnsAsync(authentication2);

            //Act
            var result = await service.CreateAuthentication(authReq, _hash, _salt);

            //Assert
            result.Id.Should().Be(_authId);
            result.Email.Should().Be(authReq.Email);
            result.UserId.Should().Be(_userId);
        }

        [Fact]
        public async Task CreateAuthenticationShouldReturnNull()
        {
            //Arrange
            var service = new AuthServices(_mockCommand.Object, _mockQuery.Object);

            _rnd.NextBytes(_hash);
            _rnd.NextBytes(_salt);

            AuthReq authReq = new AuthReq
            {
                Email = _testEmail,
                Password = _password
            };

            AuthResponse authResponse = new AuthResponse
            {
                Id = _authId,
                Email = _testEmail,
                UserId = _userId
            };

            _mockCommand.Setup(c => c.InsertAuthentication(It.IsAny<Authentication>())).ReturnsAsync(() => null);

            //Act
            var result = await service.CreateAuthentication(authReq, _hash, _salt);

            //Assert
            result.Should().BeNull();
        }

    }
}
