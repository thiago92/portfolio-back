using System.Text;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Services
{
    public static class PortfolioPromptBuilder
    {
        private const string SystemInstructionsPt =
            "Você é um assistente de IA que representa o portfólio do Thiago Silva de Souza, " +
            "um desenvolvedor web full-stack. Responda perguntas dos visitantes com base nos dados fornecidos " +
            "abaixo (contato, habilidades, trabalhos, projetos freelance e redes sociais). " +
            "Tom: profissional mas caloroso, entusiasmado, criativo. " +
            "Seja sempre honesto — NÃO invente informações que não estão nos dados. " +
            "Se o usuário perguntar algo fora do escopo do portfólio, redirecione educadamente para o que você sabe. " +
            "Responda SEMPRE no mesmo idioma em que o usuário perguntou. " +
            "Sinta-se à vontade para ser criativo na forma de apresentar as informações, mas preservando a factualidade.";

        private const string SystemInstructionsEn =
            "You are an AI assistant representing Thiago Silva de Souza's portfolio — " +
            "a full-stack web developer. Answer visitor questions based on the data provided below " +
            "(contact info, skills, works, freelance projects, and social links). " +
            "Tone: professional yet warm, enthusiastic, creative. " +
            "Always be honest — DO NOT invent information that is not in the data. " +
            "If the user asks about something outside the portfolio scope, politely redirect to what you know. " +
            "ALWAYS respond in the same language the user asked in. " +
            "Feel free to be creative in how you present the information, but stay factual.";

        public static string Build(
            Contato? contato,
            IReadOnlyList<Habilidade> habilidades,
            IReadOnlyList<Trabalho> trabalhos,
            IReadOnlyList<ProjetoCarousel> projetos,
            IReadOnlyList<LinkSocial> links,
            IReadOnlyDictionary<string, string>? translations,
            string language)
        {
            var isEn = language.Equals("en", StringComparison.OrdinalIgnoreCase);
            var sb = new StringBuilder();

            sb.AppendLine(isEn ? SystemInstructionsEn : SystemInstructionsPt);
            sb.AppendLine();
            sb.AppendLine(isEn ? "## PORTFOLIO DATA" : "## DADOS DO PORTFÓLIO");
            sb.AppendLine();

            if (contato is not null)
            {
                sb.AppendLine(isEn ? "### Contact" : "### Contato");
                sb.AppendLine($"- {(isEn ? "Name" : "Nome")}: {contato.Nome}");
                sb.AppendLine($"- {(isEn ? "Location" : "Localização")}: {contato.Localizacao}");
                sb.AppendLine($"- {(isEn ? "Phone" : "Telefone")}: {contato.Telefone}");
                sb.AppendLine($"- E-mail: {contato.Email}");
                sb.AppendLine();
            }

            if (habilidades.Count > 0)
            {
                sb.AppendLine(isEn ? "### Technical Skills (0-100 proficiency)" : "### Habilidades Técnicas (proficiência 0-100)");
                foreach (var h in habilidades.OrderBy(x => x.Ordem))
                {
                    sb.AppendLine($"- {h.Label}: {h.Valor}%");
                }
                sb.AppendLine();
            }

            if (trabalhos.Count > 0)
            {
                sb.AppendLine(isEn ? "### Featured Works" : "### Trabalhos em Destaque");
                foreach (var t in trabalhos.OrderBy(x => x.Ordem))
                {
                    sb.AppendLine($"- **{Translate(translations, t.TituloSlug)}**");
                    sb.AppendLine($"  {Translate(translations, t.TextoSlug)}");
                    sb.AppendLine($"  Link: {t.Href}");
                    sb.AppendLine();
                }
            }

            if (projetos.Count > 0)
            {
                sb.AppendLine(isEn ? "### Freelance Projects (clients)" : "### Projetos Freelance (clientes)");
                foreach (var p in projetos.OrderBy(x => x.Ordem))
                {
                    sb.AppendLine($"- {Translate(translations, p.AltSlug)}: {p.Url}");
                }
                sb.AppendLine();
            }

            if (links.Count > 0)
            {
                sb.AppendLine(isEn ? "### Social Links & Quick Contact" : "### Redes Sociais e Contato Rápido");
                foreach (var l in links.OrderBy(x => x.Tipo).ThenBy(x => x.Ordem))
                {
                    var tipo = l.Tipo == ETipoLinkSocial.Social
                        ? (isEn ? "Social" : "Rede social")
                        : (isEn ? "Quick contact" : "Contato rápido");
                    sb.AppendLine($"- [{tipo}] {l.IconeSlug}: {l.Url}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string Translate(IReadOnlyDictionary<string, string>? dict, string slug)
        {
            if (dict is null || string.IsNullOrEmpty(slug)) return slug;
            return dict.TryGetValue(slug, out var translated) ? translated : slug;
        }
    }
}
