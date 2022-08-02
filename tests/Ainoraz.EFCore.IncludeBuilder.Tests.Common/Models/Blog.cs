using System;
using System.Collections.Generic;

namespace Ainoraz.EFCore.IncludeBuilder.Tests.Common.Models;

public class Blog
{
    public Guid Id { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public List<User> Followers { get; set; } = new List<User>();
}
