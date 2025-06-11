using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Section
{
    public int SectionId { get; set; }

    public int? CourseId { get; set; }

    public string? SectionName { get; set; }

    public virtual ICollection<Sectionitem> Sectionitems { get; set; } = new List<Sectionitem>();
}
