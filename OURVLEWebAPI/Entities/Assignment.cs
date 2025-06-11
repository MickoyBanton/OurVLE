using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public ulong? CourseId { get; set; }

    public string? AssignmentTitle { get; set; }

    public DateTime? Date { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Submitassignment> Submitassignments { get; set; } = new List<Submitassignment>();
}
