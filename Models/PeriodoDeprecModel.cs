using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models
{
    public class PeriodoDeprecModel
    {
        public int idPeriodo { get; set; }

        public int idActivo { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int idEstado { get; set; }
        public double depreMensual { get; set; }

    }//fn class
}//fn space
