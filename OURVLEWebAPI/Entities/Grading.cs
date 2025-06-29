using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OURVLEWebAPI.Entities;

public partial class Grading
{
    public int SubmissionId { get; set; }

    public decimal? Grade { get; set; }

    public virtual Submitassignment Submission { get; set; } = null!;
}
