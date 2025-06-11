using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Top10HighestAverage
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public decimal? AverageGrade { get; set; }
}
