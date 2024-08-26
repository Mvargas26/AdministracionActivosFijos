using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class acercaDeNegocios
    {
        public List<string> CargarAcercade(string rutaArchivo)
        {
            List<string> contenidoArchivo = new List<string>();

            using (StreamReader sr = File.OpenText(rutaArchivo))
            {
                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    contenidoArchivo.Add(linea); // Agrega cada línea a la lista
                }
            }

            return contenidoArchivo;
        }
    }//fn class
}//fn space
