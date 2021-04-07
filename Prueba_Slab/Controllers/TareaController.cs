using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TareaController : Controller
    {
        private readonly IConfiguration _configuration;
        public TareaController(AppDBContext context)
        {
            Context = context;
        }
        public AppDBContext Context { get; }

        [HttpGet("Obj")]
        public ActionResult GetObj()
        {
            ProyectoList PL = new ProyectoList();
            List<ProyectoList> LstPL = new List<ProyectoList>();
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Pendiente" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Realizada" });
            #endregion
            try
            {
                List<Proyecto> LstPro = Context.Proyecto.ToList();
                TareaIndex TI = new TareaIndex();
                if (LstPro.Count > 0)
                {
                    foreach (Proyecto P in LstPro)
                    {
                        PL = new ProyectoList()
                        {
                            Id = P.Id,
                            Nombre = P.Nombre,
                            Descripcion = P.Descripcion,
                            FechaInicio = P.Fecha_Inicio,
                            FechaFin = P.Fecha_Fin,
                            Id_Operario = P.Id_Operario,
                            Operario = Context.Usuario.Where(x => x.Id == P.Id).Select(x => x.Nombre).FirstOrDefault(),
                            Estado = P.Estado == false ? 0 : 1,
                            ValEstado = P.Estado == false ? "Pendiente" : "Realizada"
                        };
                        LstPL.Add(PL);
                    }
                }
                TI = new TareaIndex()
                {
                    Id = 0,
                    Nombre = "",
                    Descripcion = "",
                    Fecha_Ejecucion = DateTime.Now,
                    Id_Proyecto = 0,
                    Estado = 0,
                    ListaEstado = LstEL,
                    LstProyecto = LstPL
                };
                return Ok(TI);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto de Tareas, error: " + ex);
            }
        }

        // GET api/<controller>/Obj/5
        [HttpGet("Obj/{id}")]
        public ActionResult GetObj(int id)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Pendiente" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Realizada" });
            #endregion
            ProyectoList PL = new ProyectoList();
            List<ProyectoList> LstPL = new List<ProyectoList>();
            try
            {
                List<Proyecto> LstPro = Context.Proyecto.ToList();
                Tarea T = Context.Tarea.Where(x => x.Id == id).FirstOrDefault();
                TareaIndex TI = new TareaIndex();
                if (LstPro.Count > 0)
                {
                    foreach (Proyecto P in LstPro)
                    {
                        PL = new ProyectoList()
                        {
                            Id = P.Id,
                            Nombre = P.Nombre,
                            Descripcion = P.Descripcion,
                            FechaInicio = P.Fecha_Inicio,
                            FechaFin = P.Fecha_Fin,
                            Id_Operario = P.Id_Operario,
                            Operario = Context.Usuario.Where(x => x.Id == P.Id).Select(x => x.Nombre).FirstOrDefault(),
                            Estado = P.Estado == false ? 0 : 1,
                            ValEstado = P.Estado == false ? "Pendiente" : "Realizada"
                        };
                        LstPL.Add(PL);
                    }
                }
                TI = new TareaIndex()
                {
                    Id = T.Id,
                    Nombre = T.Nombre,
                    Descripcion = T.Descripcion,
                    Fecha_Ejecucion = T.Fecha_Ejecucion,
                    Id_Proyecto = T.Id_Proyecto,
                    Estado = T.Estado == false ? 0 : 1,
                    ListaEstado = LstEL,
                    LstProyecto = LstPL,
                };
                return Ok(TI);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el objeto de la Tarea, error: " + ex);
            }
        }

        [HttpGet("List")]
        public ActionResult GetList()
        {
            TareaList TL = new TareaList();
            List<TareaList> LstTL = new List<TareaList>();
            try
            {
                List<Tarea> Ta = Context.Tarea.ToList();
                if (Ta.Count > 0)
                {
                    foreach (Tarea T in Ta)
                    {
                        TL = new TareaList()
                        {
                            Id = T.Id,
                            Nombre = T.Nombre,
                            Descripcion = T.Descripcion,
                            Fecha_Ejecucion = T.Fecha_Ejecucion,
                            Id_Proyecto = T.Id_Proyecto,
                            Proyecto = Context.Proyecto.Where(x => x.Id == T.Id_Proyecto).Select(x => x.Nombre).FirstOrDefault(),
                            Estado = T.Estado == false ? 0 : 1,
                            ValEstado = T.Estado == false ? "Pendiente" : "Realizada"
                        };
                        LstTL.Add(TL);
                    }
                }
                return Ok(LstTL);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener el listado de las Tareas, error: "+ex.Message.ToString());
            }
        }

        [HttpGet("List/{id}")]
        public ActionResult GetListId(int id)
        {
            TareaList TL = new TareaList();
            try
            {
                Tarea T = Context.Tarea.Where(x => x.Id == id).FirstOrDefault();

                TL = new TareaList()
                {
                    Id = T.Id,
                    Nombre = T.Nombre,
                    Descripcion = T.Descripcion,
                    Fecha_Ejecucion = T.Fecha_Ejecucion,
                    Id_Proyecto = T.Id_Proyecto,
                    Proyecto = Context.Proyecto.Where(x => x.Id == T.Id_Proyecto).Select(x => x.Nombre).FirstOrDefault(),
                    Estado = T.Estado == false ? 0 : 1,
                    ValEstado = T.Estado == false ? "Pendiente" : "Realizada"
                };
                return Ok(TL);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener la tarea, error: "+ex.Message.ToString());
            }
        }

        // POST api/<controller>
        [HttpPost]
        public ActionResult Post([FromBody]TareaEdit model)
        {
            #region Estados
            List<EstadoList> LstEL = new List<EstadoList>();
            EstadoList EL = new EstadoList();
            LstEL.Add(EL = new EstadoList() { Id = 0, Estado = "Pendiente" });
            LstEL.Add(EL = new EstadoList() { Id = 1, Estado = "Realizada" });
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
                            Proyecto P = Context.Proyecto.Where(x => x.Id == model.Id_Proyecto).FirstOrDefault();
                            if (R != null && P != null)
                            {
                                if (model.Id == 0)
                                {
                                    Tarea T = Context.Tarea.Where(x => x.Nombre.ToUpper().Equals(model.Nombre.ToUpper()) && x.Descripcion.ToUpper().Equals(model.Descripcion.ToUpper()) && x.Id_Proyecto == model.Id_Proyecto).FirstOrDefault();
                                    if (T == null && LstEL.Where(x => x.Id == model.Estado).FirstOrDefault() != null && (P.Fecha_Fin >= model.Fecha_Ejecucion && P.Fecha_Inicio <= model.Fecha_Ejecucion))
                                    {
                                        T = new Tarea()
                                        {
                                            Nombre = model.Nombre,
                                            Descripcion = model.Descripcion,
                                            Fecha_Ejecucion = model.Fecha_Ejecucion,
                                            Id_Proyecto = model.Id_Proyecto,
                                            Estado = model.Estado == 0 ? false : true
                                        };
                                        Context.Tarea.Add(T);
                                        Context.SaveChanges();
                                        transaction.Commit();
                                        return Ok();
                                    }
                                    else
                                    {
                                        return BadRequest("Hay problemas con la informacion dada, revise que la tarea no exista con el mismo nombre y la misma descripcion y que la fecha de ejecucion no sea mayor que el fin del proyecto.");
                                    }
                                }
                                else
                                {
                                    Tarea TR = Context.Tarea.Where(x => x.Id == model.Id).FirstOrDefault();
                                    Proyecto PR = Context.Proyecto.Where(x => x.Id == TR.Id_Proyecto).FirstOrDefault();
                                    if (TR != null && (PR.Fecha_Fin >= model.Fecha_Ejecucion))
                                    {
                                        TR.Nombre = model.Nombre;
                                        TR.Descripcion = model.Descripcion;
                                        TR.Fecha_Ejecucion = model.Fecha_Ejecucion;
                                        Context.Entry(TR).State = EntityState.Modified;
                                        Context.SaveChanges();
                                        transaction.Commit();
                                        return Ok();
                                    }
                                    else
                                    {
                                        //Tarea no existe o la fecha es mayor a la finalizacion del proyecto
                                        return BadRequest("No se logro validar la tarea a actualizar o la fecha de ejecucion de la tarea es mayor que la de finalizacion del proyecto.");
                                    }
                                }
                            }
                            else
                            {
                                //Solo operarios pueden crear Tareas
                                return BadRequest("Recuerde que solo usuarios con rol Operario pueden realizar esta acción.");
                            }
                        }
                        else
                        {
                            //no se pudo verificar el token
                            return BadRequest("No se pudo verificar la autenticidad del token enviado.");
                        }
                    }
                    else
                    {
                        //No se verifico el token
                        return BadRequest("No se pudo verificar la autenticidad del token enviado.");
                    }
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("Error al Crear/Actualizar la tarea, error: "+Exc.Message.ToString());
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
                            if (R != null)
                            {
                                Tarea TR = Context.Tarea.Where(x => x.Id == id).FirstOrDefault();
                                if (TR != null)
                                {
                                    if (TR.Estado == false)
                                    {
                                        TR.Estado = true;
                                        Context.Entry(TR).State = EntityState.Modified;
                                        Context.SaveChanges();
                                        transaction.Commit();
                                        return Ok();
                                    }
                                    else
                                    {
                                        return BadRequest("El proyecto ya esta finalizado.");
                                    }
                                }
                                else
                                {
                                    return BadRequest("No se logró validar el Proyecto.");
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
                            return BadRequest("No se pudo verificar la autenticidad del token enviado.");
                        }
                    }
                    else
                    {
                        //No se valido el token
                        return BadRequest("No se pudo verificar la autenticidad del token enviado.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Error al actualizar el estado de la tarea, error: "+ex.Message.ToString());
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
                    Tarea TR = Context.Tarea.Where(x => x.Id == id).FirstOrDefault();
                    Context.Entry(TR).State = EntityState.Deleted;
                    Context.SaveChanges();
                    transaction.Commit();
                    return Ok();
                }
                catch (Exception Exc)
                {
                    transaction.Rollback();
                    return BadRequest("No se pudo eliminar la tarea, error: "+Exc.Message.ToString());
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
