using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _services;
        private readonly IValidateServices _validations;
        private readonly ITokenServices _tokenServices;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IEncryptServices _encryptServices;

        public AuthController(IAuthServices services, IValidateServices validations, ITokenServices tokenServices, IConfiguration configuration, IEmailSender emailSender, IEncryptServices encryptServices)
        {
            _services = services;
            _validations = validations;
            _tokenServices = tokenServices;
            _configuration = configuration;
            _emailSender = emailSender;
            _encryptServices = encryptServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuth(AuthReq req)
        {
            try
            {
                var errors = _validations.Validate(req).Result;

                if (errors.Count > 0)
                {
                    return new JsonResult(new { Message = "Existen errores en la petición.", Response = errors }) { StatusCode = 400 };
                }
                else
                {
                    _encryptServices.CreatePasswordHash(req.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    var response = await _services.CreateAuthentication(req, passwordHash, passwordSalt);

                    string emailSubject = "ExpressoDeLasDiez ---> Cuenta Creada Satisfactoriamente";
                    string toEmail = response.Email;
                    string[] partes = response.Email.Split('@');
                    string userName = partes[0];
                    string emailMesagge = "Hola " + userName +"\n" +
                        "¡Bienvenido/a a nuestra plataforma! \n" +
                        "Nos complace informarte que tu cuenta ha sido creada exitosamente.\n" +
                        " Ahora puedes iniciar sesión en nuestra plataforma y comenzar a explorar todas las características y funcionalidades que ofrecemos.\n" +
                        " Si tienes alguna pregunta o problema, no dudes en ponerte en contacto con nosotros.Estaremos encantados de ayudarte en todo lo que podamos.\n" +
                        "¡Que tengas un excelente día!\n" +
                        "Saludos cordiales.";

                    await _emailSender.SendEmail(emailSubject,toEmail,userName,emailMesagge);
                    return new JsonResult(new { Message = "Exito.", Response = response }) { StatusCode = 201 };

                }
            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Problema interno del servidor." }) { StatusCode = 500 };
            }
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthReq req)
        {
            try
            {
                AuthResponseComplete auth = await _services.GetAuthentication(req);

                if (!_encryptServices.VerifyPassword(req.Password, auth.PasswordHash, auth.PasswordSalt))
                {
                    return new JsonResult(new { Message = "Credenciales Incorrectas" }) { StatusCode = 401 };
                }

                if (auth == null)
                {
                    return new JsonResult(new { Message = "Verifique los datos ingresados" }) { StatusCode = 400 };
                }

                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                var token = _tokenServices.GenerateToken(jwt, auth.AuthResponse);

                return new JsonResult(new { Message = "Ha iniciado sesión", Token = token }) { StatusCode = 200};

            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Problema interno del servidor." }) { StatusCode = 500 };
            }
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePassReq req)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var mail = identity.Claims.FirstOrDefault(x => x.Type == "Mail").Value;
                AuthReq req2 = new AuthReq()
                {
                    Email = mail,
                    Password = req.Password,
                };

                AuthResponseComplete auth = await _services.GetAuthentication(req2);

                if (!_encryptServices.VerifyPassword(req.Password, auth.PasswordHash, auth.PasswordSalt))
                {
                    return new JsonResult(new { Message = "Credenciales Incorrectas" }) { StatusCode = 401 };
                }

                if (auth == null)
                {
                    return new JsonResult(new { Message = "Credenciales Incorrectas" }) { StatusCode = 401 };
                }
                else
                {
                    var errors = await _validations.ValidateChangePassReq(req);

                    if (errors.Count > 0)
                    {
                        return new JsonResult(new { Message = "Existen errores en la petición", Response = errors }) { StatusCode = 200 };
                    }
                    else
                    {
                        var authId = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "AuthId").Value);
                        _encryptServices.CreatePasswordHash(req.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

                        var response = await _services.ChangePassword(authId, req, passwordHash, passwordSalt);

                        return new JsonResult(response);
                    }
                }
            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Problema interno del servidor." }) { StatusCode = 500 };
            }
        }
    }
}
