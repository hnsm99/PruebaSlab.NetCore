using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Slab.Models.MTarea
{
    public class TareaList
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha_Ejecucion { get; set; }
        public int Id_Proyecto { get; set; }
        public string Proyecto { get; set; }
        public int Estado { get; set; }
        public string ValEstado { get; set; }
    }
}
