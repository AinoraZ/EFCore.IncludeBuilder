# EFCore.IncludeBuilder

[![NuGet](https://img.shields.io/nuget/v/Ainoraz.EFCore.IncludeBuilder)](https://www.nuget.org/packages/Ainoraz.EFCore.IncludeBuilder)
[![Build](https://github.com/Ainoraz/EFCore.IncludeBuilder/actions/workflows/build-ci.yml/badge.svg)](https://github.com/AinoraZ/EFCore.IncludeBuilder/actions/workflows/build-ci.yml)

Extension library for Entity Framework Core (6, 7) that tries to improve upon the ```Include(...).ThenInclude(...)``` syntax in order to better support the following scenarios:

- Loading multiple entities on the same level (siblings).
- Writing extension methods to include a whole set of entities at once.

## Usage - Sibling Data

EFCore.IncludeBuilder allows you to load sibling entities without going up the whole include structure.

### ```UseIncludeBuilder()``` syntax

```csharp
dbContext.Users
    .UseIncludeBuilder()
    .Include(u => u.OwnedBlog, builder => builder
        .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)), builder => builder
            .Include(p => p.Author)
            .Include(p => p.Readers, builder => builder
                .Include(r => r.ReadHistory)
                .Include(r => r.Posts)
            )
        )
    )
    .Build()
```

> ⚠️Caution: Autocomplete after List/Collection Includes is currently broken for VS and VSCode due to a bug in Roslyn. This is tracked by issue [#16](https://github.com/AinoraZ/EFCore.IncludeBuilder/issues/16). Regardless of autocomplete, **the code will still fully compile and work**.

### ```Include(...).ThenInclude(...)``` syntax

```csharp
dbContext.Users
    .Include(u => u.OwnedBlog)
        .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ThenInclude(p => p.Readers)
                .ThenInclude(p => p.ReadHistory)
    .Include(u => u.OwnedBlog)
        .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ThenInclude(p => p.Author)
    .Include(u => u.OwnedBlog)
        .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ThenInclude(p => p.Readers)
                .ThenInclude(p => p.Posts)

```

## Usage - Extension Methods

Since IncludeBuilder uses the same ```Include(...)``` method signature for all levels, it is possible to write extension methods without depending on whether the includes are root level ```Include(...)``` on ```IQueryable``` or nested ```ThenInclude(...)``` on ```IIncludableQueryable```.

### Extension method

```csharp
public static TBuilder IncludeBlogChildren<TBase, TBuilder>(this IIncludeBuilder<TBase, Blog, TBuilder> blogBuilder)
    where TBase : class
    where TBuilder : IIncludeBuilder<TBase, Blog, TBuilder>
    {
    return blogBuilder
        .Include(b => b.Author)
        .Include(b => b.Posts, builder => builder
            .Include(p => p.Readers)
        );
    }
```

### Root level extension usage

```csharp
dbContext.Blogs
    .UseIncludeBuilder()
    .IncludeBlogChildren()
    .Build();
```

### Nested level extension usage

```csharp
dbContext.Users
    .UseIncludeBuilder()
    .Include(u => u.OwnedBlog, builder => builder
        .IncludeBlogChildren()
        .Include(b => b.Followers)
    )
    .Build();
```

## How It Works

EFCore.IncludeBuilder converts the includes you give it back to ```Include(...).ThenInclude(...)``` calls, so it **_should_** support all the same providers and functionality as the original.

## Performance

```UseIncludeBuilder``` adds very little overhead both time and memory wise:

| Method            |     Mean |    Error |   StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------------|---------:|---------:|---------:|------:|--------:|----------:|------------:|
| Include           | 30.89 μs | 0.572 μs | 0.535 μs |  1.00 |    0.00 |  10.53 KB |        1.00 |
| UseIncludeBuilder | 33.50 μs | 0.163 μs | 0.136 μs |  1.08 |    0.02 |  11.91 KB |        1.13 |

For larger queries that duplicate the same filters, it can even be the faster option:

| Method                   |     Mean |    Error |   StdDev | Ratio | Allocated | Alloc Ratio |
|--------------------------|---------:|---------:|---------:|------:|----------:|------------:|
| Include                  | 69.77 μs | 0.140 μs | 0.109 μs |  1.00 |   23.8 KB |        1.00 |
| Include_DuplicatedFilter | 87.86 μs | 0.177 μs | 0.148 μs |  1.26 |  29.93 KB |        1.26 |
| UseIncludeBuilder        | 78.62 μs | 0.213 μs | 0.167 μs |  1.13 |  25.34 KB |        1.07 |

You can find the most up to date benchmarks in the build artifacts for each build.
