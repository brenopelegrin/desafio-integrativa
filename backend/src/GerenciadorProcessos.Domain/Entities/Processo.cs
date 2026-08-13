using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Domain.Exceptions;
using GerenciadorProcessos.Domain.Constants;

namespace GerenciadorProcessos.Domain.Entities;

public class Processo
{
    public Guid Id { get; private set; }
    public string NumeroProcesso { get; private set; }
    public TipoProcesso TipoProcesso { get; private set; }
    public string Assunto { get; private set; }
    public DateTimeOffset DataCriacao { get; private set; }
    public StatusProcesso Status { get; private set; }
    public bool FlagDeleted { get; private set; }

    private readonly List<Parte> _partes = new();
    public IReadOnlyCollection<Parte> Partes => _partes.AsReadOnly();

    private readonly List<Andamento> _andamentos = new();
    public IReadOnlyCollection<Andamento> Andamentos => _andamentos.AsReadOnly();

    protected Processo() { }

    public Processo(string numeroProcesso, TipoProcesso tipoProcesso, string assunto, DateTimeOffset dataCriacao, IEnumerable<Parte> partes)
    {
        Id = Guid.NewGuid();
        TipoProcesso = tipoProcesso;
        NumeroProcesso = numeroProcesso;
        Assunto = assunto;
        DataCriacao = dataCriacao;
        Status = StatusProcesso.Ativo;
        FlagDeleted = false;

        ValidateNumeroProcesso();

        if (partes == null) throw new ArgumentNullException(nameof(partes));
        var partesList = partes.ToList();

        if (!partesList.Any(p => p.TipoPolo == TipoPolo.Ativo) || !partesList.Any(p => p.TipoPolo == TipoPolo.Passivo))
            throw new DomainException("Um processo deve ter ao menos uma parte no polo Ativo e uma parte no polo Passivo na sua criação.");

        foreach (var parte in partesList)
        {
            AddParte(parte);
        }
    }

    private void ValidateNumeroProcesso()
    {
        if (string.IsNullOrWhiteSpace(NumeroProcesso))
            throw new DomainException("O número do processo não pode ser vazio.");

        if (TipoProcesso == TipoProcesso.Judicial)
        {
            if (!Regex.IsMatch(NumeroProcesso, DomainConstants.CnjRegex))
                throw new DomainException("O número do processo judicial deve seguir o formato CNJ (NNNNNNN-DD.AAAA.J.TR.OOOO).");
        }
    }

    public void UpdateAssunto(string assunto)
    {
        if (Assunto == assunto) return;

        if (Status != StatusProcesso.Ativo)
            throw new ProcessoArquivadoException("Não é possível alterar o assunto de um processo inativo.");

        Assunto = assunto;
    }

    public void ChangeStatus(StatusProcesso newStatus)
    {
        if (Status == StatusProcesso.Finalizado && newStatus == StatusProcesso.Ativo)
            throw new DomainException("Um processo Finalizado não pode retornar diretamente para Ativo.");

        if (Status == StatusProcesso.Arquivado && newStatus == StatusProcesso.Finalizado)
            throw new DomainException("Um processo Arquivado não pode ser Finalizado diretamente. Ele deve ser ativado primeiro.");

        Status = newStatus;
    }

    public void Delete()
    {
        if (Status != StatusProcesso.Arquivado && Status != StatusProcesso.Finalizado)
            throw new DomainException("Apenas processos com status Arquivado ou Finalizado podem ser excluídos.");

        FlagDeleted = true;
    }

    public void AddParte(Parte parte)
    {
        if (Status != StatusProcesso.Ativo)
            throw new ProcessoArquivadoException("Não é possível adicionar partes a um processo inativo.");

        // Uniqueness check: same EntidadeLegal
        bool alreadyExists = _partes.Any(p => !p.FlagDeleted && p.EntidadeLegalId == parte.EntidadeLegalId);

        if (alreadyExists)
            throw new DomainException("Esta entidade legal já é uma parte neste processo.");

        _partes.Add(parte);
    }

    public void RemoveParte(Guid parteId)
    {
        if (Status != StatusProcesso.Ativo)
            throw new ProcessoArquivadoException("Não é possível remover partes de um processo inativo.");

        var parte = _partes.Find(p => p.Id == parteId && !p.FlagDeleted);
        if (parte == null)
            throw new ParteNotFoundException();

        var activePartes = _partes.Where(p => !p.FlagDeleted).ToList();

        if (parte.TipoPolo == TipoPolo.Ativo && activePartes.Count(p => p.TipoPolo == TipoPolo.Ativo) <= 1)
            throw new DomainException("Não é possível remover a parte, pois o processo deve ter ao menos 1 parte como Polo Ativo.");

        if (parte.TipoPolo == TipoPolo.Passivo && activePartes.Count(p => p.TipoPolo == TipoPolo.Passivo) <= 1)
            throw new DomainException("Não é possível remover a parte, pois o processo deve ter ao menos 1 parte como Polo Passivo.");

        parte.Delete();
    }

    public void AddAndamento(Andamento andamento, DateTimeOffset systemTimeNow)
    {
        if (Status != StatusProcesso.Ativo)
            throw new ProcessoArquivadoException();

        if (andamento.Data > systemTimeNow)
            throw new InvalidAndamentoDateException("A data do andamento não pode ser no futuro.");

        if (andamento.Data < DataCriacao)
            throw new InvalidAndamentoDateException("A data do andamento não pode ser anterior à data de criação do processo.");

        _andamentos.Add(andamento);
    }
}
