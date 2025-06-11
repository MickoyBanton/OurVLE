using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Discussionthread
{
    public int ThreadId { get; set; }

    public int? ForumId { get; set; }

    public int? UserId { get; set; }

    public string? Message { get; set; }

    public int? ParentThreadId { get; set; }

    public virtual Discussionforum? Forum { get; set; }

    public virtual ICollection<Discussionthread> InverseParentThread { get; set; } = new List<Discussionthread>();

    public virtual Discussionthread? ParentThread { get; set; }

    public virtual Account? User { get; set; }
}
