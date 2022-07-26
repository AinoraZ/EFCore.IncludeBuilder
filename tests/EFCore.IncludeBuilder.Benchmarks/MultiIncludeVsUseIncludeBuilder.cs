using Ainoraz.EFCore.IncludeBuilder.Common;
using Ainoraz.EFCore.IncludeBuilder.Extensions;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Benchmarks;

[MemoryDiagnoser]
public class MultiIncludeVsUseIncludeBuilder
{
    private readonly TestDbContext testDbContext;

    public MultiIncludeVsUseIncludeBuilder()
    {
        testDbContext = new TestDbContext();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        testDbContext.Dispose();
    }

    [Benchmark(Baseline = true)]
    public string Include()
    {
        return testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.ReadHistory)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Author)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.OwnedBlog)
            .ToQueryString();
    }

    [Benchmark]
    public string Include_DuplicatedFilter()
    {
        return testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.ReadHistory)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Author)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.OwnedBlog)
            .ToQueryString();
    }

    [Benchmark]
    public string UseIncludeBuilder()
    {
        return testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)), builder => builder
                    .Include(p => p.Readers, builder => builder
                        .Include(r => r.ReadHistory)
                        .Include(r => r.Posts)
                    )
                    .Include(p => p.Author)
                )
                .Include(b => b.Followers, builder => builder
                    .Include(f => f.OwnedBlog)
                )
            )
            .Build()
            .ToQueryString();
    }
}
