using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FuncionarioModel
    {
        [Display(Name = "Identificacion")]

        public int idFuncionario { get; set; }
        [Display(Name = "Correo")]

        public string correoFuncionario { get; set; } = string.Empty;
        [Display(Name = "Contraseña")]

        public string password { get; set; } = string.Empty;
        [Display(Name = "Nombre")]

        public string nombre { get; set; } = string.Empty;
        [Display(Name = "Primer Apellido")]

        public string apellido1 { get; set; } = string.Empty;
        [Display(Name = "Segundo Apellido")]

        public string? apellido2 { get; set; } = string.Empty;
        [Display(Name = "Departamento")]

        public int idDepartamento { get; set; }

        [Display(Name = "Usuario Activo")]
        public Boolean estado { get; set; }

    }//fn class
}//fn space
