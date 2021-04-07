using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Slab.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        public string RolName { get; set; }
    }
}
