using Maros.Application.Common;

namespace Maros.Application.Interfaces;

public interface IPaginationService
{
    Task<PagedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize);
}