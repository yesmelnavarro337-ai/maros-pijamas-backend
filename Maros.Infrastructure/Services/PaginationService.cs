using Maros.Application.Common;
using Maros.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Services;

public class PaginationService : IPaginationService
{
    public async Task<PagedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 10 : pageSize;
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PagedResult<T>(items, page, size, totalCount);
    }
}