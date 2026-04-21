using System.ComponentModel;

namespace Portfolio.Domain.Enums
{
    public enum ERoleUsuario
    {
        [Description("Admin")]
        Admin = 1,

        [Description("Visitante")]
        Visitante = 2
    }
}
