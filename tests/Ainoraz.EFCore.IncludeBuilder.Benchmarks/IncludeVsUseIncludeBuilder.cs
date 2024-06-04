using Ainoraz.EFCore.IncludeBuilder.Extensions;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Benchmarks;

[MemoryDiagnoser]
public class IncludeVsUseIncludeBuilder
{
    private readonly TestDbContext _testDbContext = new();

    [GlobalCleanup]
    public void GlobalCleanup() => _testDbContext.Dispose();

    [Benchmark(Baseline = true)]
    public string Include()
    {
        return _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.ReadHistory)
            .ToQueryString();
    }

    [Benchmark]
    public string UseIncludeBuilder()
    {
        return _testDbContext.Users
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
