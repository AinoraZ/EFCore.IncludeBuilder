using System;
using System.Collections.Generic;

namespace Ainoraz.EFCore.IncludeBuilder.Common.Models;

public class User
{
    public Guid Id { get; set; }

    public IEnumerable<Blog> FollowingBlogs { get; set; } = new List<Blog>();
    public IEnumerable<Post> ReadHistory { get; set; } = new List<Post>();
    public Blog OwnedBlog { get; set; } = null!;
    public IEnumerable<Post> Posts { get; set; } = new List<Post>();
}
