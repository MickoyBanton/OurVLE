using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Discussionforum
{
    public int ForumId { get; set; }

    public ulong? CourseId { get; set; }

    public string? Title { get; set; }

    public string? Question { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Discussionthread> Discussionthreads { get; set; } = new List<Discussionthread>();
}
