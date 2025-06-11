using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Grading
{
    public int SubmissionId { get; set; }

    public decimal? Grade { get; set; }

    public virtual Submitassignment Submission { get; set; } = null!;
}
