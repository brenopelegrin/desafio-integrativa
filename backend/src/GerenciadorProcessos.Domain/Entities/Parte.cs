using System;
using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Domain.Entities;

public class Parte
{
    public Guid Id { get; private set; }
    public TipoPolo TipoPolo { get; private set; }

    public Guid ProcessoId { get; private set; }
    public Processo Processo { get; private set; }

    public Guid EntidadeLegalId { get; private set; }
    public EntidadeLegal EntidadeLegal { get; private set; }

    public bool FlagDeleted { get; private set; }

    protected Parte() { }

    public Parte(TipoPolo tipoPolo, EntidadeLegal entidadeLegal)
    {
        TipoPolo = tipoPolo;
        EntidadeLegal = entidadeLegal ?? throw new ArgumentNullException(nameof(entidadeLegal));
        EntidadeLegalId = entidadeLegal.Id;
        FlagDeleted = false;
    }

    public void Delete()
    {
        FlagDeleted = true;
    }
}
