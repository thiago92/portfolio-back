using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities
{
    public class User : EntityAudited
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ERoleUsuario Role { get; set; } = ERoleUsuario.Admin;
    }
}
