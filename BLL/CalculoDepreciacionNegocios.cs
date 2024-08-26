using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Models;

namespace BLL
{
    public class CalculoDepreciacionNegocios
    {
        //VARIABLES
        SQLServerContext objDatos = new SQLServerContext();
        ActivosNegocios obj_Activos = new ActivosNegocios();
        public List<PeriodoDeprecModel> listarPeriodosPorTipoActivo(int codActivo)
        {
            List<PeriodoDeprecModel> listaPeriodos = new List<PeriodoDeprecModel>();
            DataTable dt = new DataTable();
            TipoActivoModel objTipoAct = new TipoActivoModel();
            DateTime fechaCompra = DateTime.Now;

            try
            {
                string query = "SELECT * FROM tbTipoAct " +
                    "JOIN tbActivo ON tbTipoAct.idTipoAct = tbActivo.idTipoAct WHERE tbActivo.cod = "+ codActivo + ";";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                     objTipoAct = new TipoActivoModel
                    {
                        idTipoAct = (int)dr["idTipoAct"],
                        tipoAct = (string)dr["tipoAct"],
                        vidaUtil = (int)dr["vidaUtil"],
                        valorResidual = (double)dr["valorResidual"],
                    };
                    //capturamos tambien la fecha en que se compro
                    fechaCompra = (DateTime)dr["fechaCompra"];
                    break;
                }

                // Sacamos en meses la vida util
                int totalMeses = objTipoAct.vidaUtil * 12;

                DateTime fechaInicio = fechaCompra; // Empezamos el calculo desde la fecha que se compró
                for (int i = 0; i < totalMeses; i++)
                {
                    PeriodoDeprecModel periodo = new PeriodoDeprecModel
                    {
                        idPeriodo = i + 1, 
                        fechaInicio = fechaInicio.AddMonths(i),
                        fechaFin = fechaInicio.AddMonths(i + 1).AddDays(-1),
                        idEstado = 1 
                    };

                    // Formateamos las fechas en "mm/yy"
                    string fechaInicioFormateada = periodo.fechaInicio.ToString("MM/yy");
                    string fechaFinFormateada = periodo.fechaFin.ToString("MM/yy");

                    listaPeriodos.Add(periodo);
                }

                return listaPeriodos;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en CalculoDepreciacionNegocios: " + ex);
            }
        }//fn listarPeriodosPorTipoActivo

        public PeriodoDeprecModel primerPeriodoSinCalcular(int codActivo)
        {
            PeriodoDeprecModel periodoSinCalc = new PeriodoDeprecModel();
            DataTable dt = new DataTable();

            try
            {
                //llamamos el SP
                string query = "exec SP_primerPeriodoNoCalculado @idActivo = "+codActivo+";";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    periodoSinCalc = new PeriodoDeprecModel
                    {
                        idPeriodo = (int)dr["idPeriodo"],
                        idActivo = (int)dr["idActivo"],
                        fechaInicio = (DateTime)dr["fechaInicio"],
                        fechaFin = (DateTime)dr["fechaFin"],
                        depreMensual = (double)dr["depreMensual"]
                    };
                    break;
                }

                return periodoSinCalc;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en primer Periodo Sin Calcular: " + ex);
            }



        }//primerPeriodoSinCalcular

        public PeriodoDeprecModel primerPeriodoSinAprobar(int codActivo)
        {
            PeriodoDeprecModel periodoSinCalc = new PeriodoDeprecModel();
            DataTable dt = new DataTable();

            try
            {
                //llamamos el SP
                string query = "SELECT TOP 1 idPeriodo, idActivo, fechaInicio, fechaFin, idEstado, depreMensual" +
                    "    FROM tbPeriodosDeprec   WHERE idActivo = "+codActivo+" AND idEstado = 1;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    periodoSinCalc = new PeriodoDeprecModel
                    {
                        idPeriodo = (int)dr["idPeriodo"],
                        idActivo = (int)dr["idActivo"],
                        fechaInicio = (DateTime)dr["fechaInicio"],
                        fechaFin = (DateTime)dr["fechaFin"],
                        idEstado = (int)dr["idEstado"],
                        depreMensual = (double)dr["depreMensual"]
                    };
                    break;
                }

                return periodoSinCalc;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en primer Periodo Sin Calcular: " + ex);
            }



        }//primerPeriodoSinCalcular
        public List<TipoDeprecModel> listarTiposDepre()
        {
            List<TipoDeprecModel> listTiposDepre = new List<TipoDeprecModel>();
            DataTable dt = new DataTable();

            try
            {
                string query = "Select * from tbTipoDepre;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    //por cada linea del datatable crea un objeto de tipo funcionario y lo add a la lista
                    listTiposDepre.Add(new TipoDeprecModel()
                    {
                        idTipoDepre = (int)dr["idTipoDepre"],
                        tipo = (string)dr["tipo"]
                    });
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios " + ex);
            }
            return listTiposDepre;
        }//fn listarTiposDepre

        public void Calcular( int codAct,int tipoMetodoDepre)
        {
            try
            {
                TipoActivoModel tipoActivoDelSeleccioando = new TipoActivoModel();
                PeriodoDeprecModel periodoCambiarEstado = new PeriodoDeprecModel();
                ActivoModel activoACalcular = new ActivoModel();
                List<TipoActivoModel> listaTipos = new List<TipoActivoModel>();


                //Obtenemos el activo con el cod
                activoACalcular = obj_Activos.consultarActivoPorCod(codAct);

                //obtenemos el periodo a calcular
                periodoCambiarEstado = primerPeriodoSinCalcular(codAct);

                //obtenemos lista de tipos de activo
                 listaTipos = obj_Activos.consultarTiposActivos();

                //buscamos que tipo es nuestro activo y guardamos el tipo
                foreach (TipoActivoModel tipoAct in listaTipos) {
                    if (tipoAct.idTipoAct == activoACalcular.idTipoActivo)
                    {
                        tipoActivoDelSeleccioando = tipoAct;
                        break;
                    }
                }
                //calculamos su VR
               
                double valorResidual = activoACalcular.costo * (tipoActivoDelSeleccioando.valorResidual/100);
                int VUR = tipoActivoDelSeleccioando.vidaUtil * 12;//vida util restante en meses

                switch (tipoMetodoDepre)
                {
                    case 1:
                        periodoCambiarEstado.depreMensual= CalcDepreciMensual_LineaRecta(activoACalcular.costo,valorResidual,VUR);

                        break;
                    case 2:
                        double factor = CalcFactor(tipoActivoDelSeleccioando.vidaUtil);
                        periodoCambiarEstado.depreMensual = CalcDepreciMensual_SumDIgitos(periodoCambiarEstado.idPeriodo,factor,activoACalcular.costo,valorResidual);
                        break;
                    case 3:
                        periodoCambiarEstado.depreMensual = CalcularDepreciMensual_Acelerada(activoACalcular.costo,valorResidual,VUR);
                        break;
                    case 4:
                        periodoCambiarEstado.depreMensual = CalcDepreciMensual_DigitosDobles(activoACalcular.costo,valorResidual,tipoActivoDelSeleccioando.vidaUtil,VUR);
                        break;
                    default:
                        break;
                }

                //cambiamos el estado del perido a 1 que es "Calculado"
                periodoCambiarEstado.idEstado = 1;

                // Al final de los calculos actualizamos el periodo
                updateEstadoPeriodo(periodoCambiarEstado);

                //Luego cargamos la informacion del activo
                activoACalcular.idMetDepre = tipoMetodoDepre;
                activoACalcular.valorLibros = activoACalcular.costo - periodoCambiarEstado.depreMensual;
                activoACalcular.depreAcumulada = sumarDepreAcumulada(activoACalcular.cod);

                //lo editamos en BD
                obj_Activos.updateActivo(activoACalcular);

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo calcular: "+ex);
            }
        }//fn Calcular

        public void updateEstadoPeriodo(PeriodoDeprecModel periodoEditar)
        {
            try
            {

                DataTable dt = new DataTable();

                string query = "UPDATE tbPeriodosDeprec SET" +
                    " depreMensual = "+periodoEditar.depreMensual+", " +
                    "idEstado = "+periodoEditar.idEstado+" " +
                    "WHERE idPeriodo = "+periodoEditar.idPeriodo+" AND idActivo = "+periodoEditar.idActivo+";";

                dt = objDatos.EjecutarSqlDT(query);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public double sumarDepreAcumulada(int idActivo)
        {
            try
            {
                DataTable dt = new DataTable();
                double depreacumulada=0;

                string query = "SELECT SUM(depreMensual) AS totalDepreciacion FROM tbPeriodosDeprec WHERE idActivo = "+idActivo+ " AND idEstado = 1;";

                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable para tomar la respuesta
                foreach (DataRow dr in dt.Rows)
                {

                    depreacumulada = (double)dr["totalDepreciacion"];
                    break;
                }


                return depreacumulada;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public List<PeriodoDeprecModel> listarPeriodosCalculadosPorActivo(int codActivo)
        {
            List<PeriodoDeprecModel> listaPeriodos = new List<PeriodoDeprecModel>();
            DataTable dt = new DataTable();
            PeriodoDeprecModel periodo = new PeriodoDeprecModel();
            DateTime fechaCompra = DateTime.Now;

            try
            {
                string query = " select * from tbPeriodosDeprec  where idActivo="+codActivo+" AND idEstado=1;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                        periodo = new PeriodoDeprecModel
                        {
                            idPeriodo = (int)dr["idPeriodo"],
                            idActivo = (int)dr["idActivo"],
                            fechaInicio = (DateTime)dr["fechaInicio"],
                            fechaFin = (DateTime)dr["fechaFin"],
                            idEstado = (int)dr["idEstado"],
                            depreMensual = (double)dr["depreMensual"]
                        };
                   
                    listaPeriodos.Add(periodo);
                }

                return listaPeriodos;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en listarPeriodosCalculadosPorActivo: " + ex);
            }
        }//fn listarPeriodosPorTipoActivo

        public List<PeriodoDeprecModel> listarPeriodosAprobadosPorActivo(int codActivo)
        {
            List<PeriodoDeprecModel> listaPeriodos = new List<PeriodoDeprecModel>();
            DataTable dt = new DataTable();
            PeriodoDeprecModel periodo = new PeriodoDeprecModel();
            DateTime fechaCompra = DateTime.Now;

            try
            {
                string query = " select * from tbPeriodosDeprec  where idActivo=" + codActivo + " AND idEstado=2;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    periodo = new PeriodoDeprecModel
                    {
                        idPeriodo = (int)dr["idPeriodo"],
                        idActivo = (int)dr["idActivo"],
                        fechaInicio = (DateTime)dr["fechaInicio"],
                        fechaFin = (DateTime)dr["fechaFin"],
                        idEstado = (int)dr["idEstado"],
                        depreMensual = (double)dr["depreMensual"]
                    };

                    listaPeriodos.Add(periodo);
                }

                return listaPeriodos;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en listarPeriodosCalculadosPorActivo: " + ex);
            }
        }//fn listarPeriodosPorTipoActivo

        public void cambiarPeriodoAEstadoAprobado(int idPeriodo)
        {
            try
            {
                string query = "UPDATE tbPeriodosDeprec SET " +
                    "  idEstado = 2 where idPeriodo = "+ idPeriodo + ";";

                 objDatos.EjecutarSqlDT(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en negocios : "+ex);
            }
        }//fn cambiarPeriodoEstadoAprobado

        #region Metodos Depreciacion
        private double CalcDepreciMensual_LineaRecta(double costoOriginal, double valorResidual, int vidaUtilEnAnios)
        {
            // Calculamos la vida útil en meses
            int vidaUtilEnMeses = vidaUtilEnAnios * 12;

            //  fórmula de línea recta
            double depreciacionMensual = (costoOriginal - valorResidual) / vidaUtilEnMeses;

            return depreciacionMensual;
        }

        private double CalcFactor(int vidaUtilEnAnios)
        {
            int vidaUtilEnMeses = vidaUtilEnAnios * 12;
            return vidaUtilEnMeses * (vidaUtilEnMeses + 1) / 2.0;
        }

        private double CalcDepreciMensual_SumDIgitos(int periodo, double factor, double costoOriginal, double valorResidual)
        {
            return (periodo / factor) * (costoOriginal - valorResidual);
        }


        private double CalcularDepreciMensual_Acelerada(double costoOriginal, double valorResidual, int vidaUtilEnAnios)
        {
            int vidaUtilEnMeses = vidaUtilEnAnios * 12;

            // Calculamos el factor (2/VU)
            double factor = 2.0 / vidaUtilEnMeses;

            // Aplicamos la fórmula 
            double depreciacionMensual = factor * (costoOriginal - valorResidual);

            return depreciacionMensual;
        }

        private double CalcDepreciMensual_DigitosDobles(double costoOriginal, double valorResidual, int vidaUtilEnAnios, int vidaUtilRestanteEnMeses)
        {
            // Calculamos el factor (2/VU*(VU+1))
            double factor = (2.0 / vidaUtilEnAnios) * (vidaUtilEnAnios + 1);

            // Aplicamos la fórmula 
            double depreciacionMensual = factor * (costoOriginal - valorResidual) / vidaUtilRestanteEnMeses;

            return depreciacionMensual;
        }
        #endregion
    }//fn class
}//fin space
