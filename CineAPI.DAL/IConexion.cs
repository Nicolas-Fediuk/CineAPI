using CineAPI.Entities;
using CineAPI.Entities.DTO;
using System.Data;
using System.Security.Claims;

namespace CineAPI.Datos
{
    public interface IConexion
    {
        Task<List<UsuarioDTO>> BuscarGeneroDescrip(List<UsuarioDTO> usuarioDTOs);
        Task CargarAsientosReservados(AsientoReservado asientoReservado);
        Task CargarReserva(Reserva reserva);
        Task EditarFuncion(string id, Funcion funcion);
        Task EliminarAsientosReservados(string reserva);
        Task EliminarFuncion(string id);
        Task EliminarPelicula(string guid);
        Task EliminarReserva(string reserva);
        Task<bool> ExisteFuncion(string id);
        Task<bool> ExisteReserva(string funcion, string asiento);
        Task<bool> ExisteUsuario(string correo);
        Task<List<Asientos>> GetAsientosDisponibles(string funcion);
        Task<List<Claim>> GetClaims(string correo);
        Task<IEnumerable<Funcion>> GetFunciones();
        Task<string> GetPasswordUsr(string corre);
        Task<string> GetPeliculaGUIDNombre(string nombre);
        Task<string> GetPeliculaNombreGUID(string guid);
        Task<IEnumerable<Pelicula>> GetPeliculas();
        Task<IEnumerable<PeliculasIdDTO>> GetPeliculasId();
        Task<string> GetPosterGuid(string guid);
        Task<IEnumerable<DetalleReservaDTO>> GetReservas();
        Task<IEnumerable<DetalleReservaDTO>> GetReservasPorUsuario(string correo);
        Task<string> GetSalaNombreGUID(int guid);
        Task<List<SalasConAsientosDTO>> GetSalasConAsientos();
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
