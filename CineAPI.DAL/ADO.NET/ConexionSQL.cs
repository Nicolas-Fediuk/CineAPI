using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CineAPI.Datos.ADO.NET
{
	public class ConexionSQL
	{
		private string rutaConexionProduccion;
		private SqlConnection conector;
		private SqlCommand comando;
		private SqlDataAdapter adaptador;
		private SqlParameter parametro;
		private SqlDataReader rd;
		private DataTable dtResultados;
		private DataSet ds;
		private int timeOut;

		private bool disposed = false;
        private readonly IConfiguration configuration;

        ~ConexionSQL()
        {

			Dispose();
        }

		public void Dispose()
		{
            if (disposed)
            {
				return;
            }

            try { 

				if (conector != null) { 

					if(conector.State == ConnectionState.Open)
					{
						conector.Close();
					}

					conector.Dispose();
				}

				if(comando != null) { 
					comando.Dispose();
				}

				if(adaptador != null)
				{
					adaptador.Dispose();
				}

				if(rd != null)
				{
					rd.DisposeAsync();
				}

				if(dtResultados != null)
				{
					dtResultados.Dispose();
				}

            }
            finally
            {
				disposed = true;
            }
		}

		public void InstanciarComando()
		{
			comando = new SqlCommand();
			comando.Connection = conector;
			comando.CommandTimeout = timeOut;
		}
		public void InstanciarComando(string c)
		{
			comando = new SqlCommand();
			comando.Connection = conector;
			comando.CommandTimeout = timeOut;
			comando.CommandText = c;
		}

        public ConexionSQL(string conexion)
		{
			string strConexion = conexion; 
            this.timeOut = 500;
			this.rutaConexionProduccion = strConexion;
			PrepararConexion();
            this.configuration = configuration;
        }

        public void PrepararConexion()
		{
			conector = new SqlConnection(rutaConexionProduccion);
		}
		public SqlConnection ObtenerConexion()
		{
			return conector;
		}

		public void InstanciarComandoProcAlmacenado(string strConsulta)
		{
			comando = new SqlCommand();
			comando.Connection = ObtenerConexion();
			comando.CommandText = strConsulta;
			comando.CommandType = CommandType.StoredProcedure;
			comando.CommandTimeout = this.timeOut;
		}

		public void InstanciarComandoProcAlmacenado()
		{
			comando = new SqlCommand();
			comando.Connection = ObtenerConexion();
			comando.CommandType = CommandType.StoredProcedure;
		}

		private void Conectar()
		{
			conector.Open();
		}
		private void Desconectar()
		{
			try
			{
				conector.Close();
			}
			catch (Exception) { }
		}

		public bool TestConnection()
		{
			try
			{
				conector.Open();
				conector.Close();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private SqlDbType GetSqlDbTypeFromType(DbType type)
		{
			return TypeConverter.ToSqlDbType(type);
		}

		public void AddNullParameter(string parameterName, DbType type)
		{
			if (comando == null)
			{
				InstanciarComando();
			}
			parameterName = "@" + parameterName;
			parametro = new SqlParameter(parameterName, type);
			parametro.Value = DBNull.Value;
			comando.Parameters.Add(parametro);
		}

		public void AddNullParameter(string parameterName, DbType type, int lenght)
		{
			if (comando == null)
			{
				InstanciarComando();
			}
			parameterName = "@" + parameterName;

			parametro = new SqlParameter(parameterName, GetSqlDbTypeFromType(type), lenght);
			parametro.Value = DBNull.Value;
			comando.Parameters.Add(parametro);
		}

		public void AddParameter(string parameterName, DbType type, object data)
		{
			if (comando == null)
			{
				InstanciarComando();
			}

			if (data == null)
			{
				AddNullParameter(parameterName, type);
				return;
			}
			parameterName = "@" + parameterName;
			parametro = new SqlParameter(parameterName, type);
			parametro.Value = data;
			comando.Parameters.Add(parametro);
		}

		public void AddParameter(string parameterName, DbType type, int length, object data)
		{
			if (comando == null)
			{
				InstanciarComando();
			}

			if (data == null)
			{
				AddNullParameter(parameterName, type, length);
				return;
			}
			parameterName = "@" + parameterName;
			parametro = new SqlParameter(parameterName, GetSqlDbTypeFromType(type), length);
			parametro.Value = data;
			comando.Parameters.Add(parametro);
		}

		public int ExecuteSP(string spName)
		{

			int result = 0;

			try
			{
				Conectar();

				if (comando == null)
				{
					InstanciarComandoProcAlmacenado(spName);
				}
				else
				{
					comando.CommandText = spName;
				}

				result = comando.ExecuteNonQuery();

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return result;
		}

		public DataTable ExecuteSPGettingResults(string pName)
		{

			try
			{
				Conectar();

				InstanciarComandoProcAlmacenado(pName);

				adaptador = new SqlDataAdapter(comando);
				dtResultados = new DataTable();
				adaptador.Fill(dtResultados);

				comando.Parameters.Clear();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return dtResultados;
		}

		public int ExecuteSPWithParameters(string spName)
		{
			int result = 0;
			try
			{
				Conectar();

				comando.CommandText = spName;
				comando.CommandType = CommandType.StoredProcedure;
				result = comando.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
				comando?.Parameters?.Clear();
				comando?.Dispose();
			}

			return result;
		}

		public DataTable ExecuteSPWithParametersGettingResults(string pName)
		{
			try
			{
				Conectar();

				comando.Connection = this.conector;
				comando.CommandText = pName;
				comando.CommandType = CommandType.StoredProcedure;
				adaptador = new SqlDataAdapter(comando);
				dtResultados = new DataTable();
				adaptador.Fill(dtResultados);
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
				comando?.Parameters?.Clear();
				comando?.Dispose();
			}

			return dtResultados;
		}

		public DataSet ExecuteSPGettingResultsDataSet(string pName)
		{
			DataSet ds = new DataSet();

			try
			{
				Conectar();

				InstanciarComandoProcAlmacenado(pName);

				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(ds);

				comando.Parameters.Clear();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return ds;
		}

		public DataSet ExecuteSPWithParametersGettingResultsDataSet(string pName)
		{
			DataSet ds = new DataSet();

			try
			{
				Conectar();

				comando.CommandText = pName;
				comando.CommandType = CommandType.StoredProcedure;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(ds);
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
				comando?.Parameters?.Clear();
				comando?.Dispose();
			}

			return ds;
		}


		public DataTable Search(IDbCommand command)
		{
			try
			{
				Conectar();
				dtResultados = new DataTable();

				if (comando == null)
				{
					InstanciarComando();
				}

				adaptador = new SqlDataAdapter((SqlCommand)command);
				adaptador.Fill(dtResultados);
			}
			catch
			{
				throw;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return dtResultados;
		}

		public async Task<DataTable> Search(string query)
		{
			try
			{
				Conectar();
				dtResultados = new DataTable();

				if (comando == null)
				{
					InstanciarComando();
				}

				comando.CommandText = query;
				comando.CommandType = CommandType.Text;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(dtResultados);
			}
			catch
			{
				throw;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return dtResultados;
		}

		public async Task<DataTable> SearchWithParameters(string query)
		{
			try
			{ 
				Conectar();
				dtResultados = new DataTable();
				comando.CommandText = query;
				comando.CommandType = CommandType.Text;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(dtResultados);
			}
			catch
			{
				throw;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return dtResultados;
		}

		public DataSet SearchDataSet(string query)
		{
			DataSet ds = new DataSet();
			try
			{
				Conectar();

				if (comando == null)
				{
					InstanciarComando();
				}

				comando.CommandText = query;
				comando.CommandType = CommandType.Text;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(ds);
			}
			catch
			{
				throw;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return ds;
		}

		public DataSet SearchDataSet(IDbCommand command)
		{
			DataSet ds = new DataSet();
			try
			{
				Conectar();

				if (comando == null)
				{
					InstanciarComando();
				}

				adaptador = new SqlDataAdapter((SqlCommand)command);
				adaptador.Fill(ds);
			}
			catch
			{
				throw;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return ds;
		}

		public DataSet SearchWithParametersDataSet(string query)
		{
			DataSet ds = new DataSet();
			try
			{
				Conectar();
				comando.CommandText = query;
				comando.CommandType = CommandType.Text;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(ds);
			}
			catch
			{
				throw;
			}
			finally
			{
				comando?.Parameters?.Clear();
				comando?.Dispose();
				Desconectar();
			}

			return ds;
		}


		public int ExecuteQuery(string query)
		{
			int result = 0;

			try
			{
				Conectar();
				InstanciarComando(query);
				comando.CommandType = CommandType.Text;
				result = comando.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
				comando?.Parameters?.Clear();
				comando?.Dispose();
			}

			return result;
		}

		public async Task<int> ExecuteQueryWithParameters(string query)
		{
			int result = 0;
			try
			{
				Conectar();
				comando.CommandText = query;
				comando.CommandType = CommandType.Text;
				result = comando.ExecuteNonQuery();
				comando.Parameters.Clear();
			}
			catch(Exception ex)
			{
				throw;
			}
			finally
			{
				Desconectar();
				comando?.Parameters?.Clear();
				comando?.Dispose();
			}

			return result;
		}

		public void SetCommandTimeOut(int timeOut)
		{
			this.timeOut = timeOut;
		}

		public int ExecuteCommand(IDbCommand command)
		{

			int result = 0;
			try
			{
				Conectar();
				command.Connection = this.conector;
				result = command.ExecuteNonQuery();
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
			}

			return result;
		}

		public DataTable ExecuteCommandGettingResults(IDbCommand command)
		{
			try
			{
				dtResultados = new DataTable();
				Conectar();
				command.Connection = this.conector;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(dtResultados);
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
			}

			return dtResultados;
		}

		public DataSet ExecuteCommandGettingResultsDataSet(IDbCommand command)
		{
			DataSet dsResultados = new DataSet();

			try
			{
				Conectar();
				command.Connection = this.conector;
				adaptador = new SqlDataAdapter(comando);
				adaptador.Fill(dsResultados);
			}
			catch
			{
				throw;
			}
			finally
			{
				Desconectar();
			}

			return dsResultados;
		}

   
    }
}
