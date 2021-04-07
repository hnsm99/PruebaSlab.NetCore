using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Slab
{
    public class Response
    {
        public bool Successfully { get; set; }

        //public int Code { get; set; }

        public string Message { get; set; }

        [NotMapped]
        public dynamic Result { get; set; }

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
