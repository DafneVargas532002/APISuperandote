using apiPlanetFitness.Models.Tools;
using APISuperandote.Models;
using APISuperandote.Models.Request.Usuario;
using APISuperandote.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APISuperandote.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly db_CMSuperandoteContext _context;

        public LoginController(db_CMSuperandoteContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login(usuario_login_request auth)
        {
            Response oResponse = new Response();
            try
            {
                var userSession = _context.Usuarios
                    .Where(i => i.Ci == auth.Ci && i.Contraseña == Encrypt.GetSHA256(auth.Contraseña) && i.Estado)
                    .Select(i => new
                    {
                        i.Ci,
                        i.Estado,
                        i.IdRol
                    })
                    .FirstOrDefault(); 

                if (userSession == null) 
                {
                    oResponse.message = "Error al ingresar, verifique sus datos.";
                    return NotFound(oResponse);
                }

                oResponse.success = 1;
                oResponse.data = userSession;

                // Creación de token
                //var token = GenerateToken(userSession.Ci.ToString(), userSession.IdRol.ToString());

                //oResponse.token = token; // Incluimos el token en la respuesta
            }
            catch (Exception ex)
            {
                oResponse.message = ex.Message;
                return BadRequest(oResponse);
            }
            return Ok(oResponse);
        }

        // Método para generar el token JWT
        private string GenerateToken(string ci, string roleId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, ci),
                new Claim(ClaimTypes.Role, roleId)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
