namespace GerenciadorProcessos.Domain.Exceptions;

public class ProcessoArquivadoException : DomainException
{
    public ProcessoArquivadoException(string message = "Não é possível alterar ou adicionar andamentos/partes a um processo arquivado ou finalizado.")
        : base(message)
    {
    }
}
