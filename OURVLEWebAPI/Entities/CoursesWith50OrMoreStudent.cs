using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class CoursesWith50OrMoreStudent
{
    public ulong CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public long StudentCount { get; set; }
}
