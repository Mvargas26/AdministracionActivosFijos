using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SQLServerContext
    {
        //Variables
        SqlConnection? sqlConn = null;


        //Constructor
        public SQLServerContext()
        {
            var _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            if (!string.IsNullOrEmpty(_configuration.GetConnectionString("ActivoFijo")))
                this.sqlConn = new SqlConnection(_configuration.GetConnectionString("ActivoFijo")?.ToString());
            else
                throw new ArgumentNullException("No se ha obtenido un string de conexión desde el archivo de configuración.");
        }//fn constructor

        public void pruebaCon()
        {
            try
            {
                sqlConn.Open();
                sqlConn.Close();
                sqlConn.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void EjecutarSQL(string ConsultaEjecutar)
        {
            try
            {
                // objeto que ejecutará la consulta en la base de datos
                SqlCommand cmd = new(ConsultaEjecutar, this.sqlConn);

                // abrir la conexión a la base de datos
                if (sqlConn.State != ConnectionState.Open)
                {
                    this.sqlConn.Open();
                }

                // ejecuta la consulta en la base de datos
                cmd.ExecuteNonQuery();

                // cierra la conexión con la base de datos
                    this.sqlConn.Close();
            }
            catch (Exception ex)
            {
                if (this.sqlConn.State == ConnectionState.Open)
                    this.sqlConn.Close();

                // retorna el error
                throw new Exception("Se ha producido un error al realizar el registro de la información en la base de datos", ex);
            }
        }

        public void EjecutarSQLconSP(string NombreSP, List<SqlParameter> listaParametro)
        {
            try
            {
                // objeto que ejecutará la consulta en la base de datos
                SqlCommand cmd = new()
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = NombreSP,
                    Connection = this.sqlConn,
                };

                foreach (SqlParameter sqlParam in listaParametro)
                    cmd.Parameters.Add(sqlParam);

                // abrir la conexión a la base de datos
                if (this.sqlConn.State != ConnectionState.Open)
                    this.sqlConn.Open();

                // ejecuta la consulta en la base de datos
                cmd.ExecuteNonQuery();

                // cierra la conexión con la base de datos
                    this.sqlConn.Close();
            }
            catch (Exception ex)
            {
                // valida que la conexión esté abierta para cerrarla
                if (this.sqlConn.State == ConnectionState.Open)
                    this.sqlConn.Close();

                // retorna el error
                throw new Exception("Se ha producido un error al realizar el registro de la información en la base de datos", ex);
            }
        }

        public DataTable EjecutarSqlDT(string consulta)
        {
            try
            {
                // objeto que ejecutará la consulta en la base de datos
                SqlCommand cmd = new( consulta,sqlConn);
                
                SqlDataAdapter adap = new SqlDataAdapter()
                { 
                SelectCommand = cmd
                };

                DataTable dt = new DataTable();

                adap.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                // valida que la conexión esté abierta para cerrarla
                if (this.sqlConn.State == ConnectionState.Open)
                    this.sqlConn.Close();

                // retorna el error
                throw new Exception("Se ha producido un error al traer registroa de la  base de datos", ex);
            }
        }



    }//fn class
}//fn space

