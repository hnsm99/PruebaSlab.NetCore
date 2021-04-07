using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Prueba_Slab.Context;
using Prueba_Slab.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prueba_Slab.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        public LoginController(AppDBContext context, IConfiguration configuration)
        {
            Context = context;
            _configuration = configuration;
        }

        public IConfiguration _configuration;
        public AppDBContext Context { get; }

        // POST api/<controller>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Post([FromBody]LoginRequest login)
        {
            if (login == null)
                return BadRequest();
            login.Password = Encriptar(login.Password);
            Usuario us = Context.Usuario.Where(x => x.UserName.Equals(login.UserName) && x.Contrasena.Equals(login.Password) && x.Estado == true).FirstOrDefault();
            if (us != null)
            {
                return Ok(new { token= GenerateTokenAwt(us) });
            }
            else
            {
                return Unauthorized();
            }
            
        }

        public string Encriptar(string Cadena)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(Cadena);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        public string GenerateTokenAwt(Usuario us)
        {
            // CREAMOS EL HEADER //
            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:ClaveSecreta"])
                );
            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _Header = new JwtHeader(_signingCredentials);
            var _Claims = new[] { new Claim(JwtRegisteredClaimNames.NameId, us.Id + "|" + us.UserName + '|' + us.Rol_Id), };
            // CREAMOS EL PAYLOAD //
            var _Payload = new JwtPayload(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: _Claims,
                    notBefore: DateTime.UtcNow,
                    // Exipra a la 24 horas.
                    expires: DateTime.UtcNow.AddHours(24)
                );
            var _Token = new JwtSecurityToken(
                    _Header,
                    _Payload
                );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }
    }
}
