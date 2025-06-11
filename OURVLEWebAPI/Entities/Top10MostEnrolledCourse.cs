using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Top10MostEnrolledCourse
{
    public ulong CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public long NumEnrolled { get; set; }
}
