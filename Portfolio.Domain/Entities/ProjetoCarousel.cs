namespace Portfolio.Domain.Entities
{
    public class ProjetoCarousel : Entity
    {
        public string Url { get; set; } = string.Empty;
        public string ImgPath { get; set; } = string.Empty;
        public string AltSlug { get; set; } = string.Empty;
        public int Ordem { get; set; }
    }
}
