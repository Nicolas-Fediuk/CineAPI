using CineAPI.Entities;
using CineAPI.Entities.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CineAPI.Datos.ADO.NET
{
    public class ConexionDB : IConexion
    {
        private readonly ConexionSQL conexionSQL;
        private readonly ILogger logger;

        public ConexionDB( ConexionSQL conexionSQL, ILogger<ConexionDB> logger) 
        {
            this.conexionSQL = conexionSQL;
            this.logger = logger;
        }

        public async Task<List<Sala>> GetSalas()
        {
            string query = "select * from Salas";

            DataTable dt = await conexionSQL.Search(query);

            var salas = dt.AsEnumerable().Select(row => new Sala
            {
                SALA_ID = row.Field<int>("SALA_ID"),
                SALA_NOMBRE = row.Field<string>("SALA_NOMBRE"),
                SALA_CAPACIDAD = row.Field<int>("SALA_CAPACIDAD")
            }).ToList();

            return salas;
        }

        public async Task NuevoUsuarioCredenciales(Credenciales credenciales)
        {
            conexionSQL.AddParameter("CREDEN_CORREO", DbType.String, credenciales.CREDEN_CORREO);
            conexionSQL.AddParameter("CREDEN_PASSWORD", DbType.Int32, credenciales.CREDEN_PASSWORD);

            string query = @"insert into CREDENCIALES
                                        (CREDEN_CORREO,
                                         CREDEN_PASSWORD
                                        )
                                       Values
                                        (@CREDEN_CORREO,
                                         @CREDEN_PASSWORD
                                        )";

            await conexionSQL.ExecuteQueryWithParameters(query);
        }

        public async Task NuevoUsuarioDatos(Usuario usuario)
        {
            conexionSQL.AddParameter("USER_GUID", DbType.String, Guid.NewGuid());
            conexionSQL.AddParameter("USER_NOMBRE", DbType.String, usuario.USER_NOMBRE);
            conexionSQL.AddParameter("USER_APELLIDO", DbType.String, usuario.USER_APELLIDO);
            conexionSQL.AddParameter("USER_CORREO", DbType.String, usuario.USER_CORREO);
            conexionSQL.AddParameter("USER_FECNAC", DbType.DateTime, usuario.USER_FECNAC);
            conexionSQL.AddParameter("USER_TELEFONO", DbType.String, usuario.USER_TELEFONO);
            conexionSQL.AddParameter("USER_GENERO", DbType.Int32, usuario.USER_GENERO);
            conexionSQL.AddParameter("USER_RECALERT", DbType.Boolean, usuario.USER_RECALERT);

            string query = @"insert into USUARIOS
                                        (USER_GUID,
                                         USER_NOMBRE,
                                         USER_APELLIDO,
                                         USER_CORREO,
                                         USER_FECNAC,
                                         USER_TELEFONO,
                                         USER_GENERO,
                                         USER_RECALERT
                                        )
                                       Values
                                        (@USER_GUID,
                                         @USER_NOMBRE,
                                         @USER_APELLIDO,
                                         @USER_CORREO,
                                         @USER_FECNAC,
                                         @USER_TELEFONO,
                                         @USER_GENERO,
                                         @USER_RECALERT
                                        )";

            await conexionSQL.ExecuteQueryWithParameters(query);
        }

        public async Task NuevoPermisoUsuario(RolesUsuario rolesUsuario)
        {
            foreach (var roles in rolesUsuario.ROLUSER_ROLID)
            {
                conexionSQL.AddParameter("ROLUSER_CORREO", DbType.String, rolesUsuario.ROLUSER_CORREO);
                conexionSQL.AddParameter("ROLUSER_ROLID", DbType.Int32, roles.ROL_ID);

                string query = @"insert into ROLES_USUARIOS
                                        (ROLUSER_CORREO,
                                         ROLUSER_ROLID
                                        )
                                       Values
                                        (@ROLUSER_CORREO,
                                         @ROLUSER_ROLID
                                        )";

                await conexionSQL.ExecuteQueryWithParameters(query);
            }
        }

        public async Task<string> GetPasswordUsr(string corre)
        {
            conexionSQL.AddParameter("correo", DbType.String, corre);

            string query = @"select CREDEN_PASSWORD from CREDENCIALES 
                             where CREDEN_CORREO = @correo";

            try
            {
                DataTable dt = await conexionSQL.SearchWithParameters(query);
                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<bool> ValidarUsr(Credenciales credenciales)
        {
            conexionSQL.AddParameter("correo", DbType.String, credenciales.CREDEN_CORREO);
            conexionSQL.AddParameter("password", DbType.String, credenciales.CREDEN_PASSWORD);

            string query = @"select * from CREDENCIALES 
                             where CREDEN_CORREO = @correo
                             and CREDEN_PASSWORD = @password";

            DataTable dt = await conexionSQL.SearchWithParameters(query);

            return dt.Rows.Count == 1;
        }

        
        public async Task<List<Claim>> GetClaims(string correo)
        {
            conexionSQL.AddParameter("correo", DbType.String, correo);

            string query = @"select ROL_ID, ROL_NOMBRE from ROLES_USUARIOS join ROLES
                             on ROLUSER_ROLID = ROL_ID
                             where ROLUSER_CORREO = @correo";

            DataTable dt = await conexionSQL.SearchWithParameters(query);

            var claims = new List<Claim>();

            foreach(DataRow row in dt.Rows)
            {
                string nombre = row["ROL_NOMBRE"].ToString();

                Claim claim = new Claim("rol", nombre);

                claims.Add(claim);
            }

            return claims;
        }

        public async Task<IEnumerable<Pelicula>> GetPeliculas()
        {
            string query = "select * from Peliculas";

            DataTable dt = await conexionSQL.Search(query);

            var peliculas = dt.AsEnumerable().Select(row => new Pelicula
            {
                PELI_TITULO = row.Field<string>("PELI_TITULO"),
                PELI_POSTER = row.Field<string>("PELI_POSTER"),
                PELI_DESCRIP = row.Field<string>("PELI_DESCRIP")
            });

            return peliculas;
        }

        public async Task NuevaPelicula(Pelicula pelicula)
        {
            conexionSQL.AddParameter("GUID", DbType.String, pelicula.PELI_GUID);
            conexionSQL.AddParameter("TITULO", DbType.String, pelicula.PELI_TITULO);
            conexionSQL.AddParameter("DESCRIP", DbType.String, pelicula.PELI_DESCRIP);
            conexionSQL.AddParameter("POSTER", DbType.String, pelicula.PELI_POSTER);
            conexionSQL.AddParameter("GENEROID", DbType.Int32, pelicula.PELI_GENEROID);

            string query = @"insert into PELICULAS
                                            (PELI_GUID,
                                             PELI_TITULO,
                                             PELI_DESCRIP,
                                             PELI_POSTER,
                                             PELI_GENEROID
                                            )
                                         select
                                             @GUID,
                                             @TITULO,
                                             @DESCRIP,
                                             @POSTER,
                                             @GENEROID";

            conexionSQL.ExecuteQueryWithParameters(query);
        }

        public async Task<string> GetPeliculaGUIDNombre(string nombre)
        {
            conexionSQL.AddParameter("nombre", DbType.String, nombre);

            string query = @"select PELI_GUID from PELICULAS where PELI_TITULO = @nombre";

            try
            {
                DataTable dt = await conexionSQL.SearchWithParameters(query);
                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task PutPelicula(string peliDBGUID, Pelicula pelicula)
        {
            conexionSQL.AddParameter("GUID", DbType.String, peliDBGUID);
            conexionSQL.AddParameter("TITULO", DbType.String, pelicula.PELI_TITULO);
            conexionSQL.AddParameter("POSTER", DbType.String, pelicula.PELI_POSTER);
            conexionSQL.AddParameter("DESCRIP", DbType.String, pelicula.PELI_DESCRIP);
            conexionSQL.AddParameter("GENERO", DbType.Int32, pelicula.PELI_GENEROID);

            string query = @"update PELICULAS
                             set PELI_TITULO = @TITULO,
                             PELI_POSTER = @POSTER,
                             PELI_DESCRIP = @DESCRIP,
                             PELI_GENEROID = @GENERO
                            where PELI_GUID = @GUID";

            conexionSQL.ExecuteQueryWithParameters(query);  
        }

        public async Task<string> GetPosterGuid(string guid)
        {
            conexionSQL.AddParameter("GUID", DbType.String, guid);

            string query = @"select PELI_POSTER from PELICULAS
                             where PELI_GUID = @GUID";

            try
            {
                DataTable dt = await conexionSQL.SearchWithParameters(query);
                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task EliminarPelicula(string guid)
        {
            conexionSQL.AddParameter("GUID", DbType.String, guid);

            string query = @"delete from PELICULAS
                              where PELI_GUID = @GUID";

            conexionSQL.ExecuteQueryWithParameters(query);
        }

        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            string query = @"select * from USUARIOS";

            DataTable dt = await conexionSQL.Search(query);

            var usuarios = dt.AsEnumerable().Select(row => new Usuario
            {
                USER_NOMBRE = row.Field<string>("USER_NOMBRE"),
                USER_APELLIDO = row.Field<string>("USER_APELLIDO"),
                USER_CORREO = row.Field<string>("USER_CORREO"),
                USER_FECNAC = row.Field<DateTime>("USER_FECNAC"),
                USER_TELEFONO = row.Field<string>("USER_TELEFONO"),
                USER_GENERO = row.Field<int>("USER_GENERO"),
                USER_RECALERT = row.Field<bool>("USER_RECALERT")
            });  

            return usuarios;
        }

        public async Task<List<UsuarioDTO>> BuscarGeneroDescrip(List<UsuarioDTO> usuarioDTOs)
        {
            foreach(var usuario in usuarioDTOs)
            {
                conexionSQL.AddParameter("CORREO", DbType.String, usuario.USER_CORREO);

                string query = @"select USREGEN_DESCRIP from USER_GENEROS JOIN USUARIOS
                                 on USER_GENERO = USERGEN_ID
                                 where USER_CORREO = @CORREO";

                DataTable dt = await conexionSQL.SearchWithParameters(query);

                usuario.USER_GENERO = dt.Rows[0][0].ToString();
            }

            return usuarioDTOs;

            var a = 0;
        }

        public async Task<bool> ExisteUsuario(string correo)
        {
            conexionSQL.AddParameter("correo", DbType.String, correo);

            string query = @"select * from USUARIOS where USER_CORREO = @correo";

            try
            {
                DataTable dt = await conexionSQL.Search(query);

                if(dt.Rows.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task HacerAdmin(string correo)
        {
            conexionSQL.AddParameter("correo", DbType.String, correo);

            string query = @"update ROLES_USUARIOS
                             set ROLUSER_ROLID = 1
                             where ROLUSER_CORREO = @correo";

            conexionSQL.ExecuteQueryWithParameters(query);
        }


        public async Task NuevaFuncion(Funcion funcion)
        {
            conexionSQL.AddParameter("GUID", DbType.String, funcion.FUNCION_GUID);
            conexionSQL.AddParameter("PELICULA", DbType.String, funcion.FUNCION_PELIGUID);
            conexionSQL.AddParameter("SALA", DbType.String, funcion.FUNCION_SALAID);
            conexionSQL.AddParameter("FECHA", DbType.DateTime, funcion.FUNCION_FECHA);
            conexionSQL.AddParameter("HORA", DbType.Time, funcion.FUNCION_HORA);
            conexionSQL.AddParameter("DURACION", DbType.Time, funcion.FUNCION_DURACION);

            string query = @"insert into FUNCIONES
                                         (FUNCION_GUID,
                                        FUNCION_PELIGUID,
                                        FUNCION_SALAID,
                                        FUNCION_FECHA,
                                        FUNCION_HORA,
                                        FUNCION_DURACION
                                         )
                                       select
                                        @GUID,
                                        @PELICULA,
                                        @SALA,
                                        @FECHA,
                                        @HORA,
                                        @DURACION";

            await conexionSQL.ExecuteQueryWithParameters(query);
        }

        public async Task<IEnumerable<Funcion>> GetFunciones()
        {
            string query = @"select * from FUNCIONES";

            DataTable dt = await conexionSQL.Search(query);

            var funciones = dt.AsEnumerable().Select(row => new Funcion
            {
                FUNCION_GUID = row.Field<string>("FUNCION_GUID"),
                FUNCION_PELIGUID = row.Field<string>("FUNCION_PELIGUID"),
                FUNCION_SALAID = row.Field<int>("FUNCION_SALAID"),
                FUNCION_FECHA = row.Field<DateTime>("FUNCION_FECHA"),
                FUNCION_HORA = row.Field<TimeSpan>("FUNCION_HORA").ToString(),
                FUNCION_DURACION = row.Field<TimeSpan>("FUNCION_DURACION").ToString()
            });

            return funciones;
        }

        public async Task<string> GetPeliculaNombreGUID(string guid)
        {
            conexionSQL.AddParameter("guid", DbType.String, guid);

            string query = @"select PELI_TITULO from PELICULAS where PELI_GUID = @guid";

            try
            {
                DataTable dt = await conexionSQL.SearchWithParameters(query);

                if(dt.Rows.Count == 0)
                {
                    return "";
                }

                return dt.Rows[0][0].ToString();

            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetSalaNombreGUID(int guid)
        {
            conexionSQL.AddParameter("guid", DbType.Int32, guid);

            string query = @"select SALA_NOMBRE from SALAS where SALA_ID = @guid";

            try
            {
                DataTable dt = await conexionSQL.SearchWithParameters(query);

                if (dt.Rows.Count == 0)
                {
                    return "";
                }

                return dt.Rows[0][0].ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> ExisteFuncion(string id)
        {
            conexionSQL.AddParameter("id", DbType.String, id);

            string query = @"select 1 from FUNCIONES where FUNCION_GUID = @id";

            DataTable dt = await conexionSQL.SearchWithParameters(query);

            return dt.Rows.Count > 0;
            
        }

        public async Task<bool> EliminarFuncion(string id)
        {
            conexionSQL.AddParameter("id", DbType.String, id);

            string query = @"delete from FUNCIONES where FUNCION_GUID = @id";

            try
            {
                await conexionSQL.ExecuteQueryWithParameters(query);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

        public async Task EditarFuncion(string id, FuncionDTO funcionDTO)
        {
            conexionSQL.AddParameter("id", DbType.String, id);
            conexionSQL.AddParameter("pelicula", DbType.String, funcionDTO.FUNCION_PELICULA);
            conexionSQL.AddParameter("sala", DbType.Int32, funcionDTO.FUNCION_SALA);
            conexionSQL.AddParameter("fecha", DbType.DateTime, funcionDTO.FUNCION_FECHA);
            conexionSQL.AddParameter("hora", DbType.Time, funcionDTO.FUNCION_HORA);
            conexionSQL.AddParameter("duracion", DbType.Time, funcionDTO.FUNCION_DURACION);

            string query = @"update FUNCIONES 
                             set FUNCION_PELIGUID = @pelicula,
                            FUNCION_SALAID = @sala,
                            FUNCION_FECHA = @fecha,
                            FUNCION_HORA = @hora,
                            FUNCION_DURACION = @duracion
                            where FUNCION_GUID = @id";
            try
            {
               await conexionSQL.ExecuteQueryWithParameters(query);
            }
            catch(Exception ex)
            {
                logger.LogError($"Error al modificar la funcion: {ex.Message}");
                throw;
            }
        }
    }
}
