using Ainoraz.EFCore.IncludeBuilder.Common;
using Ainoraz.EFCore.IncludeBuilder.Extensions;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Benchmarks;

[MemoryDiagnoser]
public class IncludeVsUseIncludeBuilder
{
    private readonly TestDbContext testDbContext;

    public IncludeVsUseIncludeBuilder()
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
                    )
                )
            )
            .Build()
            .ToQueryString();
    }
}
