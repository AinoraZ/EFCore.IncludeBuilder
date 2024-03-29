﻿using System;
using System.Collections.Generic;

namespace Ainoraz.EFCore.IncludeBuilder.Tests.Common.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public List<User> Readers { get; set; } = new List<User>();
    public DateTime PostDate { get; set; }
}
