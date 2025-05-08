using AutoMapper;
using Vemo.Gestion.Recursos.Data.DTOs;
using Vemo.Gestion.Recursos.Data.Entidades;

namespace Vemo.Gestion.Recursos.Helpers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Usuarios, UsuarioDTO>().ReverseMap();
            CreateMap<UsuarioCreacionDTO, Usuarios>().ForMember(x => x.UserName, o => o.MapFrom(MapUserName));
        }


        private string MapUserName(UsuarioCreacionDTO usuarioCreacionDTO, Usuarios usuario)
        {
            return usuarioCreacionDTO.Email!.Split('@')[0];
        }
    }
}