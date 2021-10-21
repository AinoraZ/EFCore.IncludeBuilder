# EFCore.IncludeBuilder

Extension library for Entity Framework Core that tries to improve upon the ```Include(...).ThenInclude(...)``` syntax to better support the following scenarios:

- Loading multiple entities on the same level (siblings).
- Writing extension methods to include a whole set of entities at once.

## Usage - Sibling Data

EFCore.IncludeBuilder allows you to load sibling entities without going up the whole include structure.

EFCore.IncludeBuilder syntax:

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

Normal efcore syntax:

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

Since IncludeBuilder uses the same ```Include(...)``` method signature for all levels, it's possible to write extension methods without depending on whether it's a root level ```Include(...)``` on ```IQueryable``` or a nested ```ThenInclude(...)``` on ```IIncludableQueryable```.

Extension method:

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

Root level extension usage:

```csharp
dbContext.Blogs
    .UseIncludeBuilder()
    .IncludeBlogChildren()
    .Build();
```

Nested level extension usage:

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

IncludeBuilder converts the ```UseIncludeBuilder``` syntax back to ```Include(...).ThenInclude(...)``` calls, so it should support all the same providers and functionality as the original.

## TODO

This project is still in progress, so the public interfaces are subject to change.

- [X] Propose adoption to efcore (seems to be in consideration [#23110](https://github.com/dotnet/efcore/issues/23110)).
- [ ] Publish package as third-party library for the meanwhile.
