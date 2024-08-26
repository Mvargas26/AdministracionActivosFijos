using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DepartamentoNegocios
    {
        //VARIABLES
        SQLServerContext objDatos = new SQLServerContext();

        public List<DepartamentoModel> consultarTodosDepartamentos()
        {
            DataTable dt = new DataTable();
            List<DepartamentoModel> lsDepartamentos = new List<DepartamentoModel>();

            try
            {
                string query = "Select * from tbDepartamento;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    //por cada linea del datatable crea un objeto de tipo funcionario y lo add a la lista
                    lsDepartamentos.Add(new DepartamentoModel()
                    {
                        idDepartamento = (int)dr["idDepartamento"],
                        departamento = (string)dr["departamento"]
                    });
                }

                return lsDepartamentos;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios " + ex);
            }

        }//fn consultarTodosFuncionarios

        public void crearNuevoDepartamento(DepartamentoModel departamento)
        {
            try
            {
                string query = "insert into tbDepartamento values ('"+departamento.idDepartamento+"','"+departamento.departamento+"')";
                objDatos.EjecutarSqlDT(query);
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en Departamento Negocios");
            }
        }
        public void borraroDepartamento(int idDepartamento)
        {
            try
            {
                string query = "delete from tbDepartamento where idDepartamento = " + idDepartamento + "";
                objDatos.EjecutarSqlDT(query);
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en Departamento Negocios");
            }
        }

        public DepartamentoModel consultarDepartamentoID(int idDepartamento)
        {
            try
            {
                DataTable dt = new DataTable();
                DepartamentoModel departamentoTraido = new DepartamentoModel();

                string query = "select * from tbDepartamento where idDepartamento = "+idDepartamento+";";

                dt = objDatos.EjecutarSqlDT(query);


                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    departamentoTraido.idDepartamento = (int)dr["idDepartamento"];
                    departamentoTraido.departamento = (string)dr["departamento"];
                  break;
                }


                return departamentoTraido;
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios departamentos: "+ex);
            }

        }

        public void editarDepartamento (DepartamentoModel departamentoEdiatar)
        {
            try
            {
                string query = "update tbDepartamento set " +
                    " departamento = '"+departamentoEdiatar.departamento+"' " +
                    "where idDepartamento = "+departamentoEdiatar.idDepartamento+";";

                objDatos.EjecutarSqlDT(query);
            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en Departamento Negocios");
            }
        }//fn departamentoEdiatar

    }//fn class
}//fn space
