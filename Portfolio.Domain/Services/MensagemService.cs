using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class MensagemService : DomainService<Mensagem>, IMensagemService
    {
        public MensagemService(IMensagemRepository repository) : base(repository)
        {
        }
    }
}
