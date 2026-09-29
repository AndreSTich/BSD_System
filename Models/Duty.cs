using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class Duty
{
    public int Id { get; set; }

    public int SubdivisionId { get; set; }

    public DateOnly Dates { get; set; }

    public virtual Subdivision Subdivision { get; set; } = null!;
}
