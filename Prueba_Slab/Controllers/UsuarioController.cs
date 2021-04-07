using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prueba_Slab.Context;
using Prueba_Slab.Models;
using Prueba_Slab.Models.MUsuario;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prueba_Slab.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {
        public UsuarioController(AppDBContext context, IConfiguration configuration)
        {
            Context = context;
            _configuration = configuration;
        }

        public IConfiguration _configuration;
        public AppDBContext Context { get; }
        // GET: api/<controller>
        [HttpGet("Obj")]
        
        public ActionResult GetObj()
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Inactivo" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Activo" });
            #endregion
            try
            {
                UsuarioIndex UI = new UsuarioIndex();
                UI = new UsuarioIndex()
                {
                    Id = 0,
                    Nombre = "",
                    Apellido = "",
                    UserName = "",
                    Correo = "",
                    Estado = 0,
                    ListaEstados = LstEL
                };
                return Ok(UI);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto de Usuarios, error: " + ex);
            }
        }

        // GET api/<controller>/Obj/5
        [HttpGet("Obj/{id}")]
        public ActionResult GetObj(int id)
        {
            try
            {
                #region Estados
                List<EstadoList> LstEL = new List<EstadoList>();
                EstadoList EL = new EstadoList();
                LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Inactivo" });
                LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Activo" });
                #endregion
                UsuarioIndex UI = new UsuarioIndex();
                Usuario U = Context.Usuario.Where(x => x.Id == id).FirstOrDefault();
                UI = new UsuarioIndex()
                {
                    Id = U.Id,
                    Nombre = U.Nombre,
                    Apellido = U.Apellido,
                    UserName = U.UserName,
                    Correo = U.Correo,
                    Estado = U.Estado == false ? 0 : 1,
                    ListaEstados = LstEL
                };
                return Ok(UI);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto de Usuarios, error: " + ex);
            }
        }

        // GET api/<controller>/ObjChPass
        [HttpGet("ObjChPass")]
        public ActionResult GetObjPass()
        {
            try
            {
                UsuarioPass UP = new UsuarioPass();
                return Ok(UP);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto de Usuarios para cambio de Contraseña, error: "+ex.Message.ToString());
            }
        }

        [HttpGet("List")]
        public ActionResult GetList()
        {
            UsuarioList UL = new UsuarioList();
            List<UsuarioList> LstUL = new List<UsuarioList>();
            try
            {
                List<Rol> R = Context.Rol.ToList();
                List<Usuario> Usr = Context.Usuario.ToList();

                if (Usr.Count > 0)
                {
                    foreach (Usuario U in Usr)
                    {
                        UL = new UsuarioList()
                        {
                            Id = U.Id,
                            Nombre = U.Nombre,
                            Apellido = U.Apellido,
                            Correo = U.Correo,
                            Rol = R?.Where(x => x.Id == U.Rol_Id).Select(x => x.RolName).FirstOrDefault(),
                            NombreUsuario = U.Nombre,
                            Estado = U.Estado == false ? "Inactivo" : "Activo"
                        };
                        LstUL.Add(UL);
                    }
                }
                return Ok(LstUL);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el listado de Usuarios, error: "+ex.Message.ToString());
            }
        }

        [HttpGet("List/{id}")]
        public ActionResult GetListId(int id)
        {
            UsuarioList UL = new UsuarioList();
            try
            {
                List<Rol> R = Context.Rol.ToList();
                Usuario Usr = Context.Usuario.Where(x => x.Id == id).FirstOrDefault();
                UL = new UsuarioList()
                {
                    Id = Usr.Id,
                    Nombre = Usr.Nombre,
                    Apellido = Usr.Apellido,
                    Correo = Usr.Correo,
                    Rol = R?.Where(x => x.Id == Usr.Rol_Id).Select(x => x.RolName).FirstOrDefault(),
                    NombreUsuario = Usr.Nombre,
                    Estado = Usr.Estado == false ? "Inactivo" : "Activo"
                };
                return Ok(UL);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el usuario, error: "+ex.Message.ToString());
            }
        }

        // POST api/<controller>
        [HttpPost]
        public ActionResult Post([FromBody]UsuarioEdit model)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Inactivo" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Activo" });
            #endregion
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Usuario User = Context.Usuario.Where(x => x.UserName.ToUpper().Equals(model.UserName.ToUpper()) && x.Correo.ToUpper().Equals(model.Correo.ToUpper()) && x.Rol_Id == model.Rol_Id).FirstOrDefault();
                    if (User == null && LstEL.Where(x => x.Id == model.Estado).FirstOrDefault() != null)
                    {
                        User = new Usuario()
                        {
                            Nombre = model.Nombre,
                            Apellido = model.Apellido,
                            Rol_Id = Context.Rol.Where(x => x.RolName.ToUpper().Equals("OPERARIO")).Select(x => x.Id).FirstOrDefault(),
                            UserName = model.UserName,
                            Correo = model.Correo,
                            Contrasena = Encriptar("Slab" + model.UserName + "Code"),
                            Estado = model.Estado == 0 ? false : true
                        };
                        Context.Usuario.Add(User);
                        Context.SaveChanges();
                        #region Email
                        var fromAddress = new MailAddress(_configuration["MailFromCredentials:Mail"], "Slab Code");
                        var toAddress = new MailAddress(model.Correo, model.Nombre + ' ' + model.Apellido);
                        string fromPassword = _configuration["MailFromCredentials:Password"];
                        const string subject = "Creacion de Usuario en SlabCode.";
                        string body = "Su usuario es: " + model.UserName + " y su contraseña es: " + "Slab" + model.UserName + "Code";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                        };
                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body
                        })
                        {
                            smtp.Send(message);
                        }
                        #endregion
                        transaction.Commit();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Ya existe un usuario con el nombre de usuario: "+model.UserName+", y con el correo: "+model.Correo+"");
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("Error al guardar el usuario, error: "+Exc.Message.ToString());                    
                }
            }
        }

        // PUT api/<controller>/ChEstate/5
        [HttpPut("ChEstate/{id}")]
        public ActionResult ChangeEst(int id)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Usuario Usr = Context.Usuario.Where(x => x.Id == id).FirstOrDefault();
                    if (Usr != null)
                    {
                        Usr.Estado = Usr.Estado == false ? true : false;
                        Context.Entry(Usr).State = EntityState.Modified;
                        Context.SaveChanges();
                        transaction.Commit();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("No se logro validar el usuario para el cambio de estado.");
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("Error al actualizar el estado del usuario, error: "+Exc.Message.ToString());
                }
            }
        }

        [HttpPut("ChPass")]
        public ActionResult ChangePass([FromBody]UsuarioPass model)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Usuario Usr = Context.Usuario.Where(x => x.Correo.ToUpper() == model.Correo.ToUpper()&&Desencriptar(x.Contrasena)==model.Password&&x.UserName.ToUpper()==model.NombreUsuario.ToUpper()).FirstOrDefault();
                    if (Usr != null)
                    {
                        Usr.Contrasena = Encriptar(model.NewPasswrd);
                        Context.Entry(Usr).State = EntityState.Modified;
                        Context.SaveChanges();
                        transaction.Commit();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("No se logro validar el usuario para el cambio de la contraseña.");
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("Error al actualizar la contraseña del usuario, error: "+Exc.Message.ToString());
                }
            }
        }

        public string Encriptar(string Cadena)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(Cadena);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        public string Desencriptar(string cadena)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(cadena);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }
    }
}
