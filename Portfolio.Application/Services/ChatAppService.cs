using Portfolio.Application.DTOs.Chat;
using Portfolio.Application.Interface;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class ChatAppService : IChatAppService
    {
        private readonly IContatoService _contatoService;
        private readonly IHabilidadeService _habilidadeService;
        private readonly ITrabalhoService _trabalhoService;
        private readonly IProjetoCarouselService _projetoCarouselService;
        private readonly ILinkSocialService _linkSocialService;
        private readonly IPortfolioAssistantClient _assistant;
        private readonly ITranslationProvider _translations;

        public ChatAppService(
            IContatoService contatoService,
            IHabilidadeService habilidadeService,
            ITrabalhoService trabalhoService,
            IProjetoCarouselService projetoCarouselService,
            ILinkSocialService linkSocialService,
            IPortfolioAssistantClient assistant,
            ITranslationProvider translations)
        {
            _contatoService = contatoService;
            _habilidadeService = habilidadeService;
            _trabalhoService = trabalhoService;
            _projetoCarouselService = projetoCarouselService;
            _linkSocialService = linkSocialService;
            _assistant = assistant;
            _translations = translations;
        }

        public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var language = string.IsNullOrWhiteSpace(dto.Language) ? "pt" : dto.Language.Trim().ToLowerInvariant();

            var contato = await _contatoService.GetUnicoAsync(cancellationToken);
            var habilidades = (await _habilidadeService.GetAllAsync(cancellationToken)).ToList();
            var trabalhos = (await _trabalhoService.GetAllAsync(cancellationToken)).ToList();
            var projetos = (await _projetoCarouselService.GetAllAsync(cancellationToken)).ToList();
            var links = (await _linkSocialService.GetAllAsync(cancellationToken)).ToList();

            var translations = _translations.GetDictionary(language);
            var systemPrompt = PortfolioPromptBuilder.Build(
                contato, habilidades, trabalhos, projetos, links, translations, language);

            var history = dto.History
                .Where(m => !string.IsNullOrWhiteSpace(m.Content))
                .Select(m => (m.Role, m.Content))
                .ToList();

            var answer = await _assistant.AskAsync(systemPrompt, history, dto.Message, cancellationToken);

            return new ChatResponseDto { Answer = answer };
        }
    }
}
