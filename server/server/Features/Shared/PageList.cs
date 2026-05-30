using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Server.Features.Shared;

public class PageList<T>
{
    public List<T> Items { get; }
    public int Count { get; }
    public int Page { get; }
    public int PageSize { get; }

    public bool HasNextPage => Page * PageSize < Count;
    public bool HasPrevious => Page > 1;

    public PageList(List<T> items, int count, int page, int pageSize)
    {
        Items = items;
        Count = count;
        Page = page;
        PageSize = pageSize;

    }

    public static async Task<PageList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
    {
        var count = await query.CountAsync();
        var items =await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
        return new PageList<T>(items, count, page, pageSize);
    }

    public static PageList<T> Create(IEnumerable<T> query, int page, int pageSize)
    {
        var count = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PageList<T>(items, count, page, pageSize);
    }
}
