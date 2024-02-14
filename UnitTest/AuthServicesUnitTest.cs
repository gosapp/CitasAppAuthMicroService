using Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //Act (Probar)

            //Assert (Verificar)
        }
    }
}
