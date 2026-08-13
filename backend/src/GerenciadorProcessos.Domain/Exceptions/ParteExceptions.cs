using System;

namespace GerenciadorProcessos.Domain.Exceptions;

public class DuplicateParteException : DomainException
{
    public DuplicateParteException()
        : base("Esta parte já está vinculada ao processo (mesmo nome e tipo).")
    {
    }

    public DuplicateParteException(string message) : base(message)
    {
    }
}

public class ParteNotFoundException : DomainException
{
    public ParteNotFoundException()
        : base("A parte informada não foi encontrada neste processo.")
    {
    }

    public ParteNotFoundException(string message) : base(message)
    {
    }
}
