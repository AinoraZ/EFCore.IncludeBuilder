using System;
using System.Collections.Generic;

namespace Ainoraz.EFCore.IncludeBuilder.Tests.Common.Models;

public class User
{
    public Guid Id { get; set; }

    public List<Blog> FollowingBlogs { get; set; } = new List<Blog>();
    public List<Post> ReadHistory { get; set; } = new List<Post>();
    public Blog OwnedBlog { get; set; } = null!;
    public List<Post> Posts { get; set; } = new List<Post>();
}
