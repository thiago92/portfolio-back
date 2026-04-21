using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class MensagemRepository : GenericRepository<Mensagem>, IMensagemRepository
    {
        public MensagemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
