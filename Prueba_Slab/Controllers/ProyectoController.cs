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
using Prueba_Slab.Models.MProyecto;
using Prueba_Slab.Models.MTarea;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prueba_Slab.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ProyectoController : Controller
    {
        
        public ProyectoController(AppDBContext context, IConfiguration configuration)
        {
            Context = context;
            _configuration = configuration;
        }
        public AppDBContext Context { get; }
        private readonly IConfiguration _configuration;

        [HttpGet("Obj")]
        public ActionResult GetObj()
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
            #endregion
            try
            {
                ProyectoIndex PI = new ProyectoIndex();
                PI = new ProyectoIndex()
                {
                    Id = 0,
                    Nombre = "",
                    Descripcion = "",
                    FechaInicio = DateTime.Now,
                    FechaFin = DateTime.Now,
                    Estado = 0,
                    LstEstado = LstEL
                };
                return Ok(PI);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto de Proyectos, error: " + ex);
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
                LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
                LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
                #endregion
                ProyectoIndex PI = new ProyectoIndex();
                Proyecto P = Context.Proyecto.Where(x => x.Id == id).FirstOrDefault();
                PI = new ProyectoIndex()
                {
                    Id = P.Id,
                    Nombre = P.Nombre,
                    Descripcion = P.Descripcion,
                    FechaInicio = P.Fecha_Inicio,
                    FechaFin = P.Fecha_Fin,
                    Estado = P.Estado == false ? 0 : 1,
                    LstEstado = LstEL
                };
                return Ok(PI);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto del Proyecto, error: " + ex);
            }
        }

        [HttpGet("List")]
        public ActionResult GetList()
        {
            ProyectoList PL = new ProyectoList();
            List<ProyectoList> LstPL = new List<ProyectoList>();
            try
            {
                List<Rol> R = Context.Rol.ToList();
                List<Proyecto> Pro = Context.Proyecto.ToList();

                if (Pro.Count > 0)
                {
                    foreach (Proyecto P in Pro)
                    {
                        PL = new ProyectoList()
                        {
                            Id = P.Id,
                            Nombre = P.Nombre,
                            Descripcion = P.Descripcion,
                            FechaInicio = P.Fecha_Inicio,
                            FechaFin = P.Fecha_Fin,
                            Id_Operario = P.Id_Operario,
                            Operario = Context.Usuario.Where(x => x.Id == P.Id_Operario).Select(x => x.Nombre).FirstOrDefault(),
                            Estado = P.Estado == false ? 0 : 1,
                            ValEstado = P.Estado == false ? "En Proceso" : "Finalizado"
                        };
                        LstPL.Add(PL);
                    }
                }
                return Ok(LstPL);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el listado de Proyectos, error: "+ex.Message.ToString());
            }
        }

        [HttpGet("List/{id}")]
        public ActionResult GetListId(int id)
        {
            ProyectoList PL = new ProyectoList();
            try
            {
                List<Rol> R = Context.Rol.ToList();
                Proyecto P = Context.Proyecto.Where(x=>x.Id==id).FirstOrDefault();

                PL = new ProyectoList()
                {
                    Id = P.Id,
                    Nombre = P.Nombre,
                    Descripcion = P.Descripcion,
                    FechaInicio = P.Fecha_Inicio,
                    FechaFin = P.Fecha_Fin,
                    Id_Operario = P.Id_Operario,
                    Operario = Context.Usuario.Where(x => x.Id == P.Id_Operario).Select(x => x.Nombre).FirstOrDefault(),
                    Estado = P.Estado == false ? 0 : 1,
                    ValEstado = P.Estado == false ? "En Proceso" : "Finalizado"
                };
                return Ok(PL);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener la tarea solicitada, error: "+ex.Message.ToString());
            }
        }

        // POST api/<controller>
        [HttpPost]
        public ActionResult Post([FromBody]ProyectoEdit model)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "En Proceso" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Finalizado" });
            #endregion
            int Id_Usuario;
            int Id_Rol;
            string result;
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
                    {
                        result = DecodeTokenAwt(token.ToString().Replace("Bearer ", ""));
                        if (!string.IsNullOrEmpty(result))
                        {
                            Id_Usuario = int.Parse(result.Split('|')[0]);
                            Id_Rol = int.Parse(result.Split('|')[2]);
                            Rol R = Context.Rol.Where(x => x.Id == Id_Rol && x.RolName.ToUpper().Equals("OPERARIO")).FirstOrDefault();
                            Usuario us = Context.Usuario.Where(x => x.Id == Id_Usuario).FirstOrDefault();
                            if (R != null && us != null)
                            {
                                if (model.Id == 0)
                                {
                                    Proyecto Pro = Context.Proyecto.Where(x => x.Nombre.ToUpper() == model.Nombre.ToUpper() && x.Descripcion.ToUpper() == model.Descripcion.ToUpper()).FirstOrDefault();
                                    if (Pro == null && LstEL.Where(x => x.Id == model.Estado).FirstOrDefault() != null)
                                    {
                                        Pro = new Proyecto()
                                        {
                                            Nombre = model.Nombre,
                                            Descripcion = model.Descripcion,
                                            Fecha_Inicio = model.FechaInicio,
                                            Fecha_Fin = model.FechaFin,
                                            Id_Operario = Id_Usuario,
                                            Estado = model.Estado == 0 ? false : true
                                        };
                                        Context.Proyecto.Add(Pro);
                                        Context.SaveChanges();
                                        transaction.Commit();
                                        return Ok();
                                    }
                                    else
                                    {
                                        //Existe el proyecto
                                        return BadRequest("El proyecto con nombre: "+model.Nombre+" y con descripcion: "+model.Descripcion+" ya existen.");
                                    }
                                }
                                else
                                {
                                    bool OkFechas = true;
                                    Proyecto PR = Context.Proyecto.Where(x => x.Id == model.Id).FirstOrDefault();
                                    List<Tarea> T = Context.Tarea.Where(x => x.Id_Proyecto == model.Id).ToList();
                                    if (PR != null)
                                    {
                                        foreach (Tarea tarea in T)
                                        {
                                            if (tarea.Fecha_Ejecucion > PR.Fecha_Fin)
                                            {
                                                OkFechas = false;
                                            }
                                        }
                                        if (OkFechas)
                                        {
                                            PR.Nombre = model.Nombre;
                                            PR.Descripcion = model.Descripcion;
                                            PR.Fecha_Fin = model.FechaFin;
                                            Context.Entry(PR).State = EntityState.Modified;
                                            Context.SaveChanges();
                                            transaction.Commit();
                                            return Ok();
                                        }
                                        else
                                        {
                                            //Hay tareas pendientes
                                            return BadRequest("Hay tareas que se ejecutan despues de la fecha fin del proyecto, revise la informacion.");
                                        }
                                    }
                                    else
                                    {
                                        //Proyecto no existe
                                        return BadRequest("No se logro validar el proyecto a actualizar.");
                                    }
                                }
                            }
                            else
                            {
                                //Solo operarios pueden crear proyectos
                                return BadRequest("Recuerde que solo usuarios con rol Operario pueden realizar esta accion.");
                            }
                        }
                        else
                        {
                            //no se pudo verificar el token
                            return BadRequest("No se logro validar la autenticidad del token enviado.");
                        }
                    }
                    else
                    {
                        //No se verifico el token
                        return BadRequest("No se logro validar la autenticidad del token enviado.");
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("Error al Crear/Actualizar el proyecto. Error: "+Exc.Message.ToString());
                }
            }
        }

        // PUT api/<controller>/ChEstate/5
        [HttpPut("ChEstate/{id}")]
        public ActionResult ChangeEst(int id)
        {
            int Id_Usuario;
            int Id_Rol;
            bool TComplete = true;
            string result;
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
                    {
                        result = DecodeTokenAwt(token.ToString().Replace("Bearer ", ""));
                        if (!string.IsNullOrEmpty(result))
                        {
                            Id_Usuario = int.Parse(result.Split('|')[0]);
                            Id_Rol = int.Parse(result.Split('|')[2]);
                            Rol R = Context.Rol.Where(x => x.Id == Id_Rol && x.RolName.ToUpper().Equals("OPERARIO")).FirstOrDefault();
                            Usuario us = Context.Usuario.Where(x => x.Id == Id_Usuario).FirstOrDefault();
                            if (R != null && us != null)
                            {
                                Proyecto PR = Context.Proyecto.Where(x => x.Id == id).FirstOrDefault();
                                List<Tarea> T = Context.Tarea.Where(x => x.Id_Proyecto == id).ToList();
                                int RolId = Context.Rol.Where(x => x.RolName.ToUpper().Equals("ADMINISTRADOR")).Select(x => x.Id).FirstOrDefault();
                                List<Usuario> LstU = Context.Usuario.Where(x => x.Rol_Id == RolId).ToList();
                                if (PR != null)
                                {
                                    if (PR.Estado == false)
                                    {
                                        foreach (Tarea tarea in T)
                                        {
                                            if (tarea.Estado == false)
                                            {
                                                TComplete = false;
                                            }
                                        }
                                        if (TComplete)
                                        {
                                            PR.Estado = true;
                                            Context.Entry(PR).State = EntityState.Modified;
                                            Context.SaveChanges();
                                            #region Email
                                            var msg = new MailMessage();
                                            foreach (Usuario U in LstU)
                                            {
                                                msg.To.Add(new MailAddress(U.Correo));
                                            }
                                            msg.From = new MailAddress(_configuration["MailFromCredentials:Mail"]);
                                            string fromPassword = _configuration["MailFromCredentials:Password"];
                                            msg.Subject = "Proyecto finalizado.";
                                            msg.Body = "El proyecto: " + PR.Nombre + ", ha finalizado con exito.";

                                            var smtp = new SmtpClient
                                            {
                                                Host = "smtp.gmail.com",
                                                Port = 587,
                                                EnableSsl = true,
                                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                                UseDefaultCredentials = false,
                                                Credentials = new NetworkCredential(msg.From.Address, fromPassword)
                                            };
                                            smtp.Send(msg);
                                            #endregion
                                            transaction.Commit();
                                            return Ok();
                                        }
                                        else
                                        {
                                            //   response.Message = "Hay tareas por finalizar.";
                                            return BadRequest("No se puede actualizar el estado del proyecto ya que hay tareas por finalizar.");
                                        }
                                    }
                                    else
                                    {
                                        //   response.Message = "El proyecto ya esta finalizado.";
                                        return BadRequest("El proyecto ya se encuentra finalizado.");
                                    }
                                }
                                else
                                {
                                    //response.Message = "No se logró validar el Proyecto.";
                                    return BadRequest("No se logro validar el proyecto a cambiar el estado.");
                                }
                            }
                            else
                            {
                                //Solo operador puede realizar el cambio
                                return BadRequest("Recuerde que solo usuarios con rol Operario pueden realizar esta acción.");
                            }
                        }
                        else
                        {
                            //No se valido el token
                            return BadRequest("No se logro validar la autenticidad del token enviado.");
                        }
                    }
                    else
                    {
                        //No se valido el token
                        return BadRequest("No se logro validar la autenticidad del token enviado.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Error al actualizar el estado del proyecto, error: "+ex.Message.ToString());
                }
            }
         }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    List<Tarea> LT = Context.Tarea.Where(x => x.Id_Proyecto == id).ToList();
                    foreach (Tarea item in LT)
                    {
                        Context.Entry(item).State = EntityState.Deleted;
                        Context.SaveChanges();
                    }
                    Proyecto PR = Context.Proyecto.Where(x => x.Id == id).FirstOrDefault();
                    Context.Entry(PR).State = EntityState.Deleted;
                    Context.SaveChanges();
                    transaction.Commit();
                    return Ok();
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("Error al eliminar el proyecto, error: "+Exc.Message.ToString());
                }
            }
        }

        public string DecodeTokenAwt(string token)
        {
            var ValUser = "";
            if (!string.IsNullOrEmpty(token))
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var StringTokenAwt = tokenHandler.ReadJwtToken(token);
                var ValueUser = StringTokenAwt.Claims.ToList()[0].Value;
                ValUser = ValueUser;
            }
            return ValUser;
        }
    }
}
