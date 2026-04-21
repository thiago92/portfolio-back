using Mapster;
using Portfolio.Application.DTOs.LinksSociais;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings
{
    public sealed class LinkSocialMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<LinkSocial, LinkSocialDto>();

            config.NewConfig<LinkSocialDto, LinkSocial>()
                .Ignore(dest => dest.Id);
        }
    }
}
