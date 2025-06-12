using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OURVLEWebAPI.Entities;

public partial class Student
{
    [Required(ErrorMessage = "UserId is required")]
    public int? UserId { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    public virtual ICollection<Submitassignment> Submitassignments { get; set; } = new List<Submitassignment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
