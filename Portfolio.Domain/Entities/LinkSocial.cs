using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities
{
    public class LinkSocial : Entity
    {
        public string IconeSlug { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public ETipoLinkSocial Tipo { get; set; }
        public int Ordem { get; set; }
    }
}
