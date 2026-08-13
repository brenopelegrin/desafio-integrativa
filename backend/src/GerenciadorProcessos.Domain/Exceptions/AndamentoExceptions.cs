namespace GerenciadorProcessos.Domain.Exceptions;

public class InvalidAndamentoDateException : DomainException
{
    public InvalidAndamentoDateException(string message) : base(message)
    {
    }
}
