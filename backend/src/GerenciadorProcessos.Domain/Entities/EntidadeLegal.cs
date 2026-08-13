using System;
using System.Linq;
using GerenciadorProcessos.Domain.Enums;
using GerenciadorProcessos.Domain.Exceptions;

namespace GerenciadorProcessos.Domain.Entities;

public class EntidadeLegal
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public TipoEntidade TipoEntidade { get; private set; }
    public string NumeroDocumento { get; private set; }

    protected EntidadeLegal() { }

    public EntidadeLegal(string nome, TipoEntidade tipoEntidade, string numeroDocumento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome não pode ser vazio.");

        Id = Guid.NewGuid();
        Nome = nome;
        TipoEntidade = tipoEntidade;
        NumeroDocumento = CleanDocumentNumber(numeroDocumento);

        ValidateDocument();
    }

    public void UpdateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome não pode ser vazio.");

        Nome = nome;
    }

    private string CleanDocumentNumber(string doc)
    {
        if (string.IsNullOrWhiteSpace(doc)) return string.Empty;
        return new string(doc.Where(char.IsDigit).ToArray());
    }

    private void ValidateDocument()
    {
        if (TipoEntidade == TipoEntidade.PessoaFisica)
        {
            if (!IsValidCpf(NumeroDocumento))
                throw new DomainException("CPF inválido.");
        }
        else if (TipoEntidade == TipoEntidade.PessoaJuridica)
        {
            if (!IsValidCnpj(NumeroDocumento))
                throw new DomainException("CNPJ inválido.");
        }
    }

    private bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11) return false;
        if (cpf.Distinct().Count() == 1) return false;

        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = (resto < 2) ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = (resto < 2) ? 0 : 11 - resto;
        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }

    private bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (cnpj.Distinct().Count() == 1) return false;

        int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = (soma % 11);
        resto = (resto < 2) ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = (soma % 11);
        resto = (resto < 2) ? 0 : 11 - resto;
        digito += resto.ToString();

        return cnpj.EndsWith(digito);
    }
}
