using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Requests;

public class ProcessoFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? NumeroDocumentoParte { get; set; }
    public StatusProcesso? StatusProcesso { get; set; }
    public string? NumeroProcesso { get; set; }
}
