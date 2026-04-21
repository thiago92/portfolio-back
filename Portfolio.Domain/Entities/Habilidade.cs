namespace Portfolio.Domain.Entities
{
    public class Habilidade : Entity
    {
        public string Label { get; set; } = string.Empty;
        public int Valor { get; set; }
        public int Ordem { get; set; }
    }
}
