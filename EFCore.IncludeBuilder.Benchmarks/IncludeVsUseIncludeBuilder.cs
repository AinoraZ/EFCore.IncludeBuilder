using BenchmarkDotNet.Attributes;
using EFCore.IncludeBuilder.Extensions;
using EFCore.IncludeBuilder.Tests.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EFCore.IncludeBuilder.Benchmarks
{
    [MemoryDiagnoser]
    public class IncludeVsUseIncludeBuilder
    {
        private TestDbContext testDbContext;

        public IncludeVsUseIncludeBuilder()
        {
            testDbContext = new TestDbContext();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            testDbContext.Dispose();
        }

        [Benchmark]
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
}
