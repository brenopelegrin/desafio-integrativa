using System.Collections.Generic;

namespace GerenciadorProcessos.Application.DTOs.Responses;

public record PaginatedListDto<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize);
