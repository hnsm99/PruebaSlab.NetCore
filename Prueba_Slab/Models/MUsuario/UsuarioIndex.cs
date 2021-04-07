using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Slab.Models.MUsuario
{
    public class UsuarioIndex
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UserName { get; set; }
        public string Correo { get; set; }
        public int Estado { get; set; }
        public List<EstadoList> ListaEstados { get; set; }
    }
}
