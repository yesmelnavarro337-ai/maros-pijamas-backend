using Maros.Application.Common;
using Maros.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Maros.Infrastructure.Services;

public class PaginationService : IPaginationService
{
    public async Task<PagedResult<T>> PaginateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, pageNumber, pageSize, totalCount);
    }
}