using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ActivoModel
    {
        [Display(Name = "Código")]

        public int cod {  get; set; }

        [Display(Name = "Descripción")]

        public string descrip { get; set; } = string.Empty;

        [Display(Name = "Fecha Compra")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime fechaCompra {  get; set; }

        [Display(Name = "Caracteristicas")]

        public string caracteris {  get; set; } = string.Empty ;

        [Display(Name = "Costo")]

        public double costo { get; set; }

        [Display(Name = "Metd. Depreciación")]

        public int idMetDepre {  get; set; }

        [Display(Name = "Valor Libros")]

        public double valorLibros { get; set; }

        [Display(Name = "Dep. Acumulada")]

        public double depreAcumulada {  get; set; }

        [Display(Name = "Responsable")]

        public int codFuncionario { get; set; }

        [Display(Name = "Tipo Activo")]

        public int idTipoActivo { get; set; }

       
    }//fn class
}//fn space
