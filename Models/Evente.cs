using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class Evente
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Ttype { get; set; }

    public DateOnly Dates { get; set; }

    public string Locations { get; set; } = null!;

    public TimeOnly Times { get; set; }

    public int CreatorId { get; set; }

    public virtual Employee Creator { get; set; } = null!;
}
