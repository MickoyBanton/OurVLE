using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Course
{
    public ulong CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Calendarevent> Calendarevents { get; set; } = new List<Calendarevent>();

    public virtual ICollection<Discussionforum> Discussionforums { get; set; } = new List<Discussionforum>();

    public virtual ICollection<Student> Users { get; set; } = new List<Student>();

    public virtual ICollection<Lecturer> UsersNavigation { get; set; } = new List<Lecturer>();
}
