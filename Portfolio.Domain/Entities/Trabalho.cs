namespace Portfolio.Domain.Entities
{
    public class Trabalho : Entity
    {
        public string TituloSlug { get; set; } = string.Empty;
        public string TextoSlug { get; set; } = string.Empty;
        public string ImgPath { get; set; } = string.Empty;
        public string TextoBotaoSlug { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
        public string TooltipSlug { get; set; } = string.Empty;
        public int Ordem { get; set; }
    }
}
