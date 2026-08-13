using GerenciadorProcessos.Domain.Enums;

namespace GerenciadorProcessos.Application.DTOs.Requests;

public record UpdateProcessoDto(string Assunto, StatusProcesso Status);
