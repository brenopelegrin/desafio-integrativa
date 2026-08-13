using System;

namespace GerenciadorProcessos.Domain.Entities;

public class Andamento
{
    public Guid Id { get; private set; }
    public DateTimeOffset Data { get; private set; }
    public string Descricao { get; private set; }

    public Guid ProcessoId { get; private set; }
    public Processo Processo { get; private set; }

    public bool FlagDeleted { get; private set; }

    protected Andamento() { }

    public Andamento(DateTimeOffset data, string descricao)
    {
        Data = data;
        Descricao = descricao;
        FlagDeleted = false;
    }
}
