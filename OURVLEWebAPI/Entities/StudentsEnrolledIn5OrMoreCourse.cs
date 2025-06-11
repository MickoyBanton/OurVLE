using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class StudentsEnrolledIn5OrMoreCourse
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public long CourseCount { get; set; }
}
