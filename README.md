# EFCore.IncludeBuilder

Extension library for Entity Framework Core that tries to improve upon the ```Include(...).ThenInclude(...)``` syntax in order to better support the following scenarios:

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

|            Method |     Mean |    Error |   StdDev | Ratio |  Gen 0 |  Gen 1 | Allocated |
|------------------ |---------:|---------:|---------:|------:|-------:|-------:|----------:|
|           Include | 66.42 μs | 0.291 μs | 0.272 μs |  1.00 | 0.7324 | 0.2441 |     14 KB |
| UseIncludeBuilder | 69.22 μs | 0.243 μs | 0.227 μs |  1.04 | 0.7324 | 0.2441 |     15 KB |

For larger queries that duplicate the same filters, it can even be the faster option:

|                   Method |     Mean |   Error |  StdDev | Ratio |  Gen 0 |  Gen 1 | Allocated |
|------------------------- |---------:|--------:|--------:|------:|-------:|-------:|----------:|
|                  Include | 152.0 μs | 0.22 μs | 0.20 μs |  1.00 | 1.7090 | 0.4883 |     34 KB |
| Include_DuplicatedFilter | 186.7 μs | 0.70 μs | 0.65 μs |  1.23 | 2.1973 | 0.4883 |     41 KB |
|        UseIncludeBuilder | 164.3 μs | 0.39 μs | 0.36 μs |  1.08 | 1.7090 | 0.4883 |     35 KB |

You can find the most up to date benchmarks in the build artifacts for each build.
