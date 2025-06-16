using CineAPI.Entities;
using CineAPI.Entities.DTO;
using System.Data;
using System.Security.Claims;

namespace CineAPI.Datos
{
    public interface IConexion
    {
        Task<List<UsuarioDTO>> BuscarGeneroDescrip(List<UsuarioDTO> usuarioDTOs);
        Task<bool> EliminarFuncion(string id);
        Task EliminarPelicula(string guid);
        Task<bool> ExisteFuncion(string id);
        Task<bool> ExisteUsuario(string correo);
        Task<List<Claim>> GetClaims(string correo);
        Task<IEnumerable<Funcion>> GetFunciones();
        Task<string> GetPasswordUsr(string corre);
        Task<string> GetPeliculaGUIDNombre(string nombre);
        Task<string> GetPeliculaNombreGUID(string guid);
        Task<IEnumerable<Pelicula>> GetPeliculas();
        Task<string> GetPosterGuid(string guid);
        Task<string> GetSalaNombreGUID(int guid);
        Task<List<Sala>> GetSalas();
        Task<IEnumerable<Usuario>> GetUsuarios();
        Task HacerAdmin(string correo);
        Task NuevaFuncion(Funcion funcion);
        Task NuevaPelicula(Pelicula pelicula);
        Task NuevoPermisoUsuario(RolesUsuario rolesUsuario);
        Task NuevoUsuarioCredenciales(Credenciales credenciales);
        Task NuevoUsuarioDatos(Usuario usuario);
        Task PutPelicula(string peliDBGUID, Pelicula pelicula);
        Task<bool> ValidarUsr(Credenciales credenciales);
    }
}
