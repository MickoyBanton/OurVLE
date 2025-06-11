using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Account
{
    public int UserId { get; set; }

    public string Password { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public virtual ICollection<Discussionthread> Discussionthreads { get; set; } = new List<Discussionthread>();
}
