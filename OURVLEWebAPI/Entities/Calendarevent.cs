using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Calendarevent
{
    public int EventId { get; set; }

    public ulong? CourseId { get; set; }

    public string? Title { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual Course? Course { get; set; }
}
