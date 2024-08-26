using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TipoActivoModel
    {
        [Display(Name = "Id")]

        public int idTipoAct { get; set; }

        [Display(Name = "Tipo")]

        public string tipoAct { get; set; } = string.Empty;

        [Display(Name = "Vida Util")]

        public int vidaUtil { get; set; }

        [Display(Name = "Valor residual")]

        public double valorResidual { get; set; }

    }//fn class
}//fn space
