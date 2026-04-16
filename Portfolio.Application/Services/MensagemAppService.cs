using Portfolio.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Application.Services
{
    public class MensagemAppService : IMensagemAppService
    {
        private readonly AppDbContext _context
        public MensagemAppService(AppDbContext context)
        {
            _context = context;
        }

        public List<Mensagem> Search()
        {
            return _context.Mensagens.ToList();
        }

        public Mensagem Criar(Mensagem mensagem)
        {
            _context.Mensagens.Add(mensagem);
            _context.SaveChanges();
            return mensagem;
        }
    }
}
