using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Submitassignment
{
    public int SubmissionId { get; set; }

    public int? UserId { get; set; }

    public int? AssignmentId { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual Grading? Grading { get; set; }

    public virtual Student? User { get; set; }
}
