using System;
using System.Collections.Generic;

namespace OURVLEWebAPI.Entities;

public partial class Sectionitem
{
    public ulong ItemId { get; set; }

    public int? SectionId { get; set; }

    public string? SectionItem { get; set; }

    public string FileType { get; set; } = null!;

    public virtual Section? Section { get; set; }
}
