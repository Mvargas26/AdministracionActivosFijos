using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL
{
    public class FuncionariosNegocios
    {
        //VARIABLES
        SQLServerContext objDatos = new SQLServerContext();


        public List<FuncionarioModel> consultarTodosFuncionarios()
        {
            DataTable dt = new DataTable();
            List<FuncionarioModel>lsFuncionarios = new List<FuncionarioModel>();

            try
            {
                string query = "Select * from tbFuncionario;";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {
                 //por cada linea del datatable crea un objeto de tipo funcionario y lo add a la lista
                    lsFuncionarios.Add(new FuncionarioModel()
                    {
                        idFuncionario = (int)dr["idFuncioanrio"],
                        correoFuncionario = (string)dr["correoFuncionario"],
                        password = (string)dr["password"],
                        nombre = (string)dr["nombre"],
                        apellido1= (string)dr["apellido1"],
                        apellido2 = (string)dr["apellido2"],
                        idDepartamento = (int)dr["idDepartamento"],
                        estado = (bool)dr["estado"]
                    }) ;
                }

                return lsFuncionarios;

            }
            catch (Exception ex)
            {

                throw new Exception ("Fallo en negocios "+ex);
            }

        }//fn consultarTodosFuncionarios

        public void insertarNuevoFuncionario(FuncionarioModel newFuncionario)
        {
            try
            {
                //le damos un tratamiento dieferente a estado xq viene en string
                int estadoConvertido = 1;

                if (newFuncionario.estado == false)
                {
                    estadoConvertido = 0;
                }
                //hacemos el insert con sintaxis sql
                string query = "INSERT INTO tbFuncionario " +
                    "VALUES ("+newFuncionario.idFuncionario+"," +
                    "'"+newFuncionario.correoFuncionario+"'," +
                    "'"+newFuncionario.password+"'," +
                    "'"+newFuncionario.nombre+"'," +
                    "'"+newFuncionario.apellido1+"'," +
                    "'"+newFuncionario.apellido2+"'," +
                    "'"+newFuncionario.idDepartamento+"'," +
                    ""+estadoConvertido+")"; //ojo: aqui va el estado en # y no en string

                objDatos.EjecutarSqlDT(query);

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios: "+ ex);
            }

        }//fn insertarNuevoFuncionario

        public FuncionarioModel funcionarioXid(int id)
        {
            DataTable dt = new DataTable();
            FuncionarioModel Funcionario = new FuncionarioModel();

            try
            {
                string query = "Select * from tbFuncionario where idFuncioanrio =" + id+";";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {

                    Funcionario.idFuncionario = (int)dr["idFuncioanrio"];
                    Funcionario.correoFuncionario = (string)dr["correoFuncionario"];
                    Funcionario.password = (string)dr["password"];
                    Funcionario.nombre = (string)dr["nombre"];
                    Funcionario.apellido1 = (string)dr["apellido1"];
                    Funcionario.apellido2 = (string)dr["apellido2"];
                    Funcionario.idDepartamento = (int)dr["idDepartamento"];
                    Funcionario.estado = (bool)dr["estado"];
                    break;
                }

                    return Funcionario;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios " + ex);
            }

        }// fn funcionarioXid

        public void eliminarFuncionario(int id)
        {
            try
            {
                
                //hacemos el insert con sintaxis sql
                string query = "delete from tbFuncionario where idFuncioanrio=" + id+";"; 

                objDatos.EjecutarSqlDT(query);

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios: " + ex);
            }

        }//fn eliminarFuncionario

        public void editarFuncionario(FuncionarioModel funcionarioEdit)
        {
            try
            {
                //le damos un tratamiento dieferente a estado xq viene en string
                int estadoConvertido = 1;

                if (funcionarioEdit.estado == false)
                {
                    estadoConvertido = 0;
                }

                //hacemos el edit con sintaxis sql
                string query = "UPDATE dbo.tbFuncionario " +
                    " SET correoFuncionario = '"+funcionarioEdit.correoFuncionario+"'," +
                    "password = '"+funcionarioEdit.password+"'," +
                    "nombre = '"+funcionarioEdit.nombre+"'," +
                    "apellido1 = '"+funcionarioEdit.apellido1+"'," +
                    "apellido2 = '"+funcionarioEdit.apellido2+"'," +
                    "idDepartamento = "+funcionarioEdit.idDepartamento+"," +
                    "estado = "+estadoConvertido+"" +
                    " WHERE idFuncioanrio = "+funcionarioEdit.idFuncionario+";";

                objDatos.EjecutarSqlDT(query);

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios: " + ex);
            }
        }//fn editarFuncionario

        public Boolean funcionarioEstaActivo(  FuncionarioModel funcionario)
        {
            DataTable dt = new DataTable();
            FuncionarioModel Funcionario = new FuncionarioModel();

            try
            {
                string query = "EXEC SP_usuarioActivo  " +
                    "@correo = '"+funcionario.correoFuncionario+"', @password = '"+funcionario.password+"';";

                dt = objDatos.EjecutarSqlDT(query);

                if (dt.Rows.Count <= 0)
                {
                    //si es 0 entonces no encontro un usuario 
                    return false;
                }
                else
                {
                    //si no es 0 encontro un usuario y esta activo
                    return true;
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios " + ex);
            }

        }// FN funcionarioEstaActivo

        public Boolean funcionarioTieneActivos(FuncionarioModel funcionario)
        {
            DataTable dt = new DataTable();
            FuncionarioModel Funcionario = new FuncionarioModel();

            try
            {
                string query = " SELECT * FROM tbFuncionario f " +
                    " JOIN tbActivo a ON f.idFuncioanrio = a.codFuncionario WHERE" +
                    " f.correoFuncionario = '"+funcionario.correoFuncionario+"'" +
                    "  AND f.password = '"+funcionario.password+"';";

                dt = objDatos.EjecutarSqlDT(query);

                if (dt.Rows.Count <= 0)
                {
                    //si es 0 entonces no encontro un usuario 
                    return false;
                }
                else
                {
                    //si no es 0 encontro un usuario y esta activo
                    return true;
                }

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios " + ex);
            }

        }// FN funcionarioEstaActivo

        public FuncionarioModel obtenerFuncdelLogin(FuncionarioModel funcionarioInconpleto) {
            DataTable dt = new DataTable();
            FuncionarioModel Funcionario = new FuncionarioModel();

            try
            {
                string query = "EXEC SP_usuarioActivo  " +
                     "@correo = '" + funcionarioInconpleto.correoFuncionario + "', @password = '" + funcionarioInconpleto.password + "';";
                dt = objDatos.EjecutarSqlDT(query);

                //recorremos el DataTable
                foreach (DataRow dr in dt.Rows)
                {

                    Funcionario.idFuncionario = (int)dr["idFuncioanrio"];
                    Funcionario.correoFuncionario = (string)dr["correoFuncionario"];
                    Funcionario.password = (string)dr["password"];
                    Funcionario.nombre = (string)dr["nombre"];
                    Funcionario.apellido1 = (string)dr["apellido1"];
                    Funcionario.apellido2 = (string)dr["apellido2"];
                    Funcionario.idDepartamento = (int)dr["idDepartamento"];
                    Funcionario.estado = (bool)dr["estado"];
                    break;
                }

                return Funcionario;

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios " + ex);
            }


        }

        public bool PasswordCumple(string password)
        {
            string pattern = @"^(?=.*\d)(?=.*[\u0021-\u002b\u003c-\u0040])(?=.*[A-Z])(?=.*[a-z])\S{8,16}$";
            //devuelve true si cumple y devuelve false si no cumple con la expresion regular
            return Regex.IsMatch(password, pattern);
        }//PasswordCumple

        public bool CorreoCumple(string correo)
        {
            string pattern = @"^(([^<>()\[\]\\.,;:\s@\""]+(\.[^<>()\[\]\\.,;:\s@\""]+)*)|(\""[^\""]+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
            return Regex.IsMatch(correo, pattern);
        }//fn CorreoCumple

        public void cambiarPassword(FuncionarioModel funcionarioEdit)
        {
            try
            {
                //hacemos el edit con sintaxis sql
                string query = "update tbFuncionario SET " +
                    "password='"+funcionarioEdit.password+"' where idFuncioanrio = "+funcionarioEdit.idFuncionario+";";

                objDatos.EjecutarSqlDT(query);

            }
            catch (Exception ex)
            {

                throw new Exception("Fallo en negocios: " + ex);
            }
        }//fn editarFuncionario

    }//fn class
}//fn space
