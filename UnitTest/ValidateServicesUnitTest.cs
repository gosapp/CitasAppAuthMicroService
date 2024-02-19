using Application.Interfaces;
using Application.UseCases;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class ValidateServicesUnitTest
    {
        [Fact]
        public async Task ValidateCharactersShouldReturnFalse()
        {
            //Arrange (Preparacion)
            var mockQuery = new Mock<IAuthQueries>();
            var service = new ValidateServices(mockQuery.Object);
            var strToVerify = "mail@expre$$o.com";
            var tag = "mail";

            //Act (Probar)
            var result = await service.ValidateCharacters(strToVerify, tag);

            //Assert (Verificar)
            result.Should().Be(false);
        }

        [Fact]
        public async Task ValidateCharactersShouldReturnTrue()
        {
            //Arrange (Preparacion)
            var mockQuery = new Mock<IAuthQueries>();
            var service = new ValidateServices(mockQuery.Object);
            var strToVerify = "mail123@expresso.com";
            var tag = "mail";

            //Act (Probar)
            var result = await service.ValidateCharacters(strToVerify, tag);

            //Assert (Verificar)
            result.Should().Be(true);
        }

        [Fact]
        public async Task ValidateLengthMax()
        {
            //Arrange (Preparacion)
            var mockQuery = new Mock<IAuthQueries>();
            var service = new ValidateServices(mockQuery.Object);
            var strToVerify = "UnaCadenaMuyLargaUnaCadenaMuyLarga";
            var tag = "CualquierTag";

            //Act (Probar)
            var result = await service.ValidateLenght(strToVerify, tag, 20, 10);

            //Assert (Verificar)
            result.Should().Be(false);
        }

        [Fact]
        public async Task ValidateLengthMin()
        {
            //Arrange (Preparacion)
            var mockQuery = new Mock<IAuthQueries>();
            var service = new ValidateServices(mockQuery.Object);
            var strToVerify = "UnCort";
            var tag = "CualquierTag";

            //Act (Probar)
            var result = await service.ValidateLenght(strToVerify, tag, 20, 10);

            //Assert (Verificar)
            result.Should().Be(false);
        }

        [Fact]
        public async Task ValidateLengthNoContent()
        {
            //Arrange (Preparacion)
            var mockQuery = new Mock<IAuthQueries>();
            var service = new ValidateServices(mockQuery.Object);
            var strToVerify = "";
            var tag = "CualquierTag";

            //Act (Probar)
            var result = await service.ValidateLenght(strToVerify, tag, 20, 10);

            //Assert (Verificar)
            result.Should().Be(false);
        }

        [Fact]
        public async Task ValidateLengthShouldReturnTrue()
        {
            //Arrange (Preparacion)
            var mockQuery = new Mock<IAuthQueries>();
            var service = new ValidateServices(mockQuery.Object);
            var strToVerify = "UnaCadenaCorrecta";
            var tag = "CualquierTag";

            //Act (Probar)
            var result = await service.ValidateLenght(strToVerify, tag, 20, 10);

            //Assert (Verificar)
            result.Should().Be(true);
        }
    }
}
