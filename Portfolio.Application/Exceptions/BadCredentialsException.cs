namespace Portfolio.Application.Exceptions
{
    public sealed class BadCredentialsException : Exception
    {
        public BadCredentialsException()
            : base("Credenciais inválidas.")
        {
        }
    }
}
