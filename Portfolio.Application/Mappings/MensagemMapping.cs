using Mapster;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Mappings
{
    public sealed class MensagemMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Mensagem, MensagemDto>();
            config.NewConfig<CreateMensagemDto, Mensagem>();
            config.NewConfig<UpdateMensagemDto, Mensagem>();
        }
    }
}
