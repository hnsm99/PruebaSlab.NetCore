using Prueba_Slab.Models.MProyecto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Slab.Models.MTarea
{
    public class TareaIndex
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha_Ejecucion { get; set; }
        public int Id_Proyecto { get; set; }
        public List<ProyectoList> LstProyecto { get; set; }
        public int Estado { get; set; }
        public List<EstadoList> ListaEstado { get; set; }
    }
}
