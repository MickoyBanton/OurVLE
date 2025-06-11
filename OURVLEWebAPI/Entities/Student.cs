using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Student
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<Submitassignment> Submitassignments { get; set; } = new List<Submitassignment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
