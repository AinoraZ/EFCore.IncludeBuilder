# EFCore.IncludeBuilder
Extension library for Entity Framework Core that replaces the ```Include(...).ThenInclude(...)``` syntax to better support loading multiple entities on the same level.

Normal efcore syntax:
```csharp
dbContext.Users
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

```

EFCore.IncludeBuilder syntax:
```csharp
dbContext.Users
    .UseIncludeBuilder()
    .Include(u => u.OwnedBlog, builder => builder
        .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)), builder => builder
            .Include(p => p.Readers, builder => builder
                .Include(r => r.ReadHistory)
                .Include(r => r.Posts)
            )
            .Include(p => p.Author)
        )
    )
    .Build()
```

## TODO
- [ ] Propose adoption to efcore OR publish package as third-party library.
