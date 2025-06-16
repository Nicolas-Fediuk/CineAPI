using AutoMapper;
using CineAPI.Entities;
using CineAPI.Entities.DTO;

namespace CineAPI.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //mapea CREDEN_USR como fuente
            CreateMap<NuevoUsuarioDTO, Credenciales>().ConstructUsing(src => src.CREDEN_USR);

            CreateMap<NuevoUsuarioDTO, Usuario>().ForMember(x => x.USER_NOMBRE, x =>x.MapFrom(x => x.USER_NOMBRE))
                                                 .ForMember(x => x.USER_APELLIDO, x => x.MapFrom(x => x.USER_APELLIDO))
                                                 .ForMember(dest => dest.USER_CORREO, opt => opt.MapFrom(src => src.CREDEN_USR.CREDEN_CORREO))
                                                 .ForMember(x => x.USER_FECNAC, x => x.MapFrom(x => x.USER_FECNAC))
                                                 .ForMember(x => x.USER_TELEFONO, x => x.MapFrom(x => x.USER_TELEFONO))
                                                 .ForMember(x => x.USER_GENERO, x => x.MapFrom(x => x.USER_GENERO))
                                                 .ForMember(x => x.USER_RECALERT, x => x.MapFrom(x => x.USER_RECALERT));

            CreateMap<NuevoUsuarioDTO, RolesUsuario>().ForMember(dest => dest.ROLUSER_ROLID, opt => opt.MapFrom(src =>
                src.ROLUSER_ROLES.Select(id => new Rol { ROL_ID = id }).ToList()
            ))
            .ForMember(dest => dest.ROLUSER_CORREO, opt => opt.MapFrom(src => src.CREDEN_USR.CREDEN_CORREO))
            .ForMember(dest => dest.ROLUSER_ID, opt => opt.Ignore());

            CreateMap<NuevaPeliculaDTO, Pelicula>();

            CreateMap<Pelicula, PeliculasDTO>();

            CreateMap<Usuario, UsuarioDTO>();

            CreateMap<Funcion, ListFuncionesDTO>();
        }

       
    }
}
