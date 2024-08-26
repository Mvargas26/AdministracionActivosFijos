using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BLL
{
    public class ActivosNegocios
    {
        //VARIABLES
        SQLServerContext objDatos = new SQLServerContext();

        public List<ActivoModel> consultarTodosActivos()
        {
            DataTable dt = new DataTable();
            List<ActivoModel> lsActivos = new List<ActivoModel>();

            try
            {
                string query = "Select * from tbActivo;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    //por cada linea del datatable crea un objeto de tipo Activo y lo add a la lista
                    lsActivos.Add(new ActivoModel()
                    {
                        cod = (int)dr["cod"],
                        descrip = (string)dr["descrip"],
                        fechaCompra = (DateTime)dr["fechaCompra"],
                        caracteris = (string)dr["caracteris"],
                        costo = (double)dr["costo"],
                        idMetDepre = (int)dr["idMetDepre"],
                        valorLibros = (double)dr["valorLibros"],
                        depreAcumulada = (double)dr["depreAcumulada"],
                        codFuncionario = (int)dr["codFuncionario"],
                        idTipoActivo = (int)dr["idTipoAct"],
                    });
                }

                return lsActivos;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en activos negocios " + ex);
            }

        }//fn consultarTodosActivos

        public ActivoModel consultarActivoPorCod(int cod)
        {
            DataTable dt = new DataTable();
            ActivoModel activoConsultado = new ActivoModel();

            try
            {
                string query = "Select * from tbActivo where cod="+cod+";";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    //por cada linea del datatable crea un objeto de tipo Activo
                    activoConsultado = new ActivoModel()
                    {
                        cod = (int)dr["cod"],
                        descrip = (string)dr["descrip"],
                        fechaCompra = (DateTime)dr["fechaCompra"],
                        caracteris = (string)dr["caracteris"],
                        costo = (double)dr["costo"],
                        idMetDepre = (int)dr["idMetDepre"],
                        valorLibros = (double)dr["valorLibros"],
                        depreAcumulada = (double)dr["depreAcumulada"],
                        codFuncionario = (int)dr["codFuncionario"],
                        idTipoActivo = (int)dr["idTipoAct"]
                    };
                    break;
                }

                return activoConsultado;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en activos negocios " + ex);
            }

        }//fn consultarTodosActivos

        public void insertarNuevoActivo(ActivoModel activo)
        {
            try
            {
                string fechaSinHora = activo.fechaCompra.ToString("yyyy/MM/dd");

                //hacemos el insert con sintaxis sql
                string query = "INSERT INTO tbActivo" +
                 "(cod,descrip,fechaCompra,caracteris,costo,idMetDepre,valorLibros,DepreAcumulada,codFuncionario,idTipoAct) " +
                    "     VALUES  ("+activo.cod+",'"+activo.descrip+"','"+ fechaSinHora + "'," +
                    "'"+activo.caracteris+"',"+activo.costo+","+activo.idMetDepre+","+activo.valorLibros+","+activo.depreAcumulada+"," +
                    ""+activo.codFuncionario+","+activo.idTipoActivo+");";

                objDatos.EjecutarSqlDT(query);

                //despues que insertamos el activo
                //insertamos los periodos basado en su tipo de activo
                insertarPeriodosdelActivo(activo.cod);
            }
            catch (Exception ex)
            {

                throw new Exception("Error en negocios: "+ex);
            }
        }//fin insertarNuevoActivo

        public void insertarPeriodosdelActivo(int codActivo)
        {
                List<PeriodoDeprecModel> listaPeriodos = new List<PeriodoDeprecModel>();
                DataTable dt = new DataTable();
                TipoActivoModel objTipoAct = new TipoActivoModel();
                DateTime fechaCompra = DateTime.Now;
            try
            {
                    string query = "SELECT * FROM tbTipoAct " +
                        "JOIN tbActivo ON tbTipoAct.idTipoAct = tbActivo.idTipoAct WHERE tbActivo.cod = " + codActivo + ";";
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
                            idEstado = 1,
                            idActivo= codActivo
                            
                        };
                        listaPeriodos.Add(periodo);
                    }

                //Recorremos la lista y hacemos un insert x cada registro
                foreach (var periodo in listaPeriodos)
                {
                    //formateamos las fechas
                    string fechaInicioSinHora = periodo.fechaInicio.ToString("yyyy/MM/dd");
                    string fechaFinSinHora = periodo.fechaInicio.ToString("yyyy/MM/dd");

                    //el estado es 3 xq es "No calculado" y la depreMensual es 0 xq no se a calculado
                    string queryInsert = "insert into  tbPeriodosDeprec  (idActivo,fechaInicio,fechaFin,idEstado,depreMensual)" +
                        " values ("+codActivo+",'"+ fechaInicioSinHora + "','"+ fechaFinSinHora + "',3,0)";

                    objDatos.EjecutarSqlDT(queryInsert);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios: " + ex);
            }

        }//fn insertarPeriodosdelActivo

        public List<TipoActivoModel> consultarTiposActivos()
        {
            DataTable dt = new DataTable();
            List<TipoActivoModel> lsTiposActivos = new List<TipoActivoModel>();

            try
            {
                string query = "Select * from tbTipoAct;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    //por cada linea del datatable crea un objeto de tipo Activo y lo add a la lista
                    lsTiposActivos.Add(new TipoActivoModel()
                    {
                        idTipoAct = (int)dr["idTipoAct"],
                        tipoAct = (string)dr["tipoAct"],
                        vidaUtil = (int)dr["vidaUtil"],
                        valorResidual = (double)dr["valorResidual"]
                    });
                }

                return lsTiposActivos;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en activos negocios " + ex);
            }

        }//fn consultarTiposActivos

        public void updateActivo(ActivoModel activo)
        {
            try
            {
                string fechaSinHora = activo.fechaCompra.ToString("yyyy/MM/dd");

                //hacemos el insert con sintaxis sql
                string query = "UPDATE tbActivo SET " +
                                "descrip = '" + activo.descrip + "', " +
                                "fechaCompra = '" + fechaSinHora + "', " +
                                "caracteris = '" + activo.caracteris + "', " +
                                "costo = " + activo.costo + ", " +
                                "idMetDepre = " + activo.idMetDepre + ", " +
                                "valorLibros = " + activo.valorLibros + ", " +
                                "DepreAcumulada = " + activo.depreAcumulada + ", " +
                                "codFuncionario = " + activo.codFuncionario + ", " +
                                "idTipoAct = " + activo.idTipoActivo + " " +
                                "WHERE cod = " + activo.cod + ";";


                objDatos.EjecutarSqlDT(query);

            }
            catch (Exception ex)
            {

                throw new Exception("Error en negocios: " + ex);
            }

        }

        public void eliminarActivo(int cod)
        {

            try
            {
                DataTable dt = new DataTable();
                string query = "delete from tbActivo where cod=" + cod + ";";

                dt = objDatos.EjecutarSqlDT(query);


            }
            catch (Exception ex)
            {

                throw new Exception("Error al eliminar " + ex);
            }

        }//fn consultarTodosActivos

        public List<ActivoModel> verActivosxFuncionario(int codFuncionario)
        {
            DataTable dt = new DataTable();
            List<ActivoModel> lsActivos = new List<ActivoModel>();

            try
            {
                string query = "Select * from tbActivo where codFuncionario="+ codFuncionario + ";";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    //por cada linea del datatable crea un objeto de tipo Activo y lo add a la lista
                    lsActivos.Add(new ActivoModel()
                    {
                        cod = (int)dr["cod"],
                        descrip = (string)dr["descrip"],
                        fechaCompra = (DateTime)dr["fechaCompra"],
                        caracteris = (string)dr["caracteris"],
                        costo = (double)dr["costo"],
                        idMetDepre = (int)dr["idMetDepre"],
                        valorLibros = (double)dr["valorLibros"],
                        depreAcumulada = (double)dr["depreAcumulada"],
                        codFuncionario = (int)dr["codFuncionario"],
                        idTipoActivo = (int)dr["idTipoAct"],
                    });
                }

                return lsActivos;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en activos negocios " + ex);
            }

        }//fn consultarTodosActivos
    }//fn class
}//fn space
